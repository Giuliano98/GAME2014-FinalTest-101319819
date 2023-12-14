using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class PlayerBehaviour : MonoBehaviour
{
    public InputAction move;
    public float maxYVelocity = 10f;

    [Header("Movement Properties")]
    public float horizontalForce;
    public float horizontalSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint; // the origin of the circle
    public float groundRadius; // the size of the circle
    public LayerMask groundLayerMask; // the stuff we can collide with
    public bool isGrounded;

    [Header("Animations")]
    public Animator animator;
    public PlayerAnimationState playerAnimationState;

    [Header("Dust Trail Effect")]
    public ParticleSystem dustTrail;
    public Color dustTrailColour;

    [Header("Screen Shake Properties")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeIntensity;
    public float shakeDuration;
    public float shakeTimer;
    public bool isCameraShaking;

    [Header("Health System")]
    public HealthBarController health;
    public LifeCounterController life;
    public DeathPlaneController deathPlane;

    [Header("Controls")]
    public Joystick leftStick;
    [Range(0.1f, 1.0f)]
    public float verticalThreshold;

    private Rigidbody2D rb2D;
    private SoundManager soundManager;

    void OnEnable()
    {
        move.Enable();
    }
    void OnDisable()
    {
        move.Disable();
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = FindObjectOfType<PlayerHealth>().GetComponent<HealthBarController>();
        life = FindObjectOfType<LifeCounterController>();
        deathPlane = FindObjectOfType<DeathPlaneController>();
        soundManager = FindObjectOfType<SoundManager>();
        //leftStick = (Application.isMobilePlatform) ? GameObject.Find("LeftStick").GetComponent<Joystick>() : null;
        //leftStick = GameObject.Find("LeftStick").GetComponent<Joystick>();

        dustTrail = GetComponentInChildren<ParticleSystem>();

        playerCamera = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        perlin = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        isCameraShaking = false;
        shakeTimer = shakeDuration;
    }

    void Update()
    {
        if (health.value <= 0)
        {
            life.LoseLife();

            if (life.value > 0)
            {
                health.ResetHealth();
                deathPlane.ReSpawn(gameObject);
                soundManager.PlaySoundFX(Sound.DEATH, Channel.PLAYER_DEATH_FX);
            }
        }

        if (life.value <= 0)
        {
            BulletManager.Instance().DestroyPool();
            SceneManager.LoadScene("End");
        }

        CapYVelocity();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var hit = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
        isGrounded = hit;

        Move();
        Jump();
        AirCheck();

        // Camera Shake Control
        if (isCameraShaking)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0.0f) // timed out
            {
                perlin.m_AmplitudeGain = 0.0f;
                shakeTimer = shakeDuration;
                isCameraShaking = false;
            }
        }
    }

    private void Move()
    {
        //var x = Input.GetAxisRaw("Horizontal") + ((Application.isMobilePlatform) ? leftStick.Horizontal : 0.0f);
        //var x = Input.GetAxisRaw("Horizontal") + (leftStick.Horizontal);
        var temp = move.ReadValue<Vector2>();
        var x = temp.x;
        if (x != 0.0f)
        {
            Flip(x);

            x = (x > 0.0) ? 1.0f : -1.0f; // sanitizing x

            rb2D.AddForce(Vector2.right * x * horizontalForce * ((isGrounded) ? 1.0f : airFactor));

            var clampedXVelocity = Mathf.Clamp(rb2D.velocity.x, -horizontalSpeed, horizontalSpeed);
            rb2D.velocity = new Vector2(clampedXVelocity, rb2D.velocity.y);

            ChangeAnimation(PlayerAnimationState.RUN);

            if (isGrounded)
            {
                CreateDustTrail();
            }
        }

        if ((isGrounded) && (x == 0.0f))
        {
            ChangeAnimation(PlayerAnimationState.IDLE);
        }
    }

    private void CreateDustTrail()
    {
        dustTrail.GetComponent<Renderer>().material.SetColor("_Color", dustTrailColour);
        dustTrail.Play();
    }

    private void ShakeCamera()
    {
        perlin.m_AmplitudeGain = shakeIntensity;
        isCameraShaking = true;
    }

    private void Jump()
    {
        //var y = Input.GetAxis("Jump") + ((Application.isMobilePlatform) ? leftStick.Vertical : 0.0f);
        //var y = Input.GetAxis("Jump") + (leftStick.Vertical);
        var temp = move.ReadValue<Vector2>();
        var y = temp.y;
        if ((isGrounded) && (y > verticalThreshold))
        {
            rb2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
            soundManager.PlaySoundFX(Sound.JUMP, Channel.PLAYER_SOUND_FX);
        }
    }

    private void AirCheck()
    {
        if (!isGrounded)
        {
            ChangeAnimation(PlayerAnimationState.JUMP);
        }
    }

    public void Flip(float x)
    {
        if (x != 0.0f)
        {
            transform.localScale = new Vector3((x > 0.0f) ? 1.0f : -1.0f, 1.0f, 1.0f);
        }
    }

    private void ChangeAnimation(PlayerAnimationState animationState)
    {
        playerAnimationState = animationState;
        animator.SetInteger("AnimationState", (int)playerAnimationState);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(20);

            soundManager.PlaySoundFX(Sound.HURT, Channel.PLAYER_HURT_FX);
            ShakeCamera();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            health.TakeDamage(30);

            soundManager.PlaySoundFX(Sound.HURT, Channel.PLAYER_HURT_FX);
            ShakeCamera();
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            health.TakeDamage(10);

            soundManager.PlaySoundFX(Sound.HURT, Channel.PLAYER_HURT_FX);
            ShakeCamera();
        }
    }

    void CapYVelocity()
    {
        Vector2 velocity = rb2D.velocity;
        velocity.y = Mathf.Clamp(velocity.y, -maxYVelocity, maxYVelocity);
        rb2D.velocity = velocity;
    }
}
