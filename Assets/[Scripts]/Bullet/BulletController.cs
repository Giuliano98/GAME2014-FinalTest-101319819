using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Properties")]
    public Vector2 direction;
    public Rigidbody2D rb2D;
    [Range(1.0f, 30.0f)]
    public float force;
    public Vector3 offset;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void Activate()
    {
        Vector3 playerPosition = FindObjectOfType<PlayerBehaviour>().transform.position + offset;
        direction = (playerPosition - transform.position).normalized;
        Rotate();
        Move();
        Invoke("DestroyYourself", 2.0f);
    }

    // Update is called once per frame
    private void Rotate()
    {
        rb2D.AddTorque(Random.Range(5.0f, 15.0f) * direction.x * -1.0f, ForceMode2D.Impulse);
    }

    private void Move()
    {
        rb2D.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void DestroyYourself()
    {
        if (gameObject.activeInHierarchy)
        {
            BulletManager.Instance().ReturnBullet(this.gameObject);
        }
    }

    public void ResetAllPhysics()
    {
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0;
        direction = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")
            || other.gameObject.CompareTag("Ground")
            || other.gameObject.CompareTag("Prop")
            || other.gameObject.CompareTag("Platform"))
        {
            DestroyYourself();
        }
    }
}
