using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 movementDir;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementDir * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DamageArgs args = new();
            args.sender = transform;
            collision.GetComponent<HealthManager>().TakeDamage(1, args);
        }
        Destroy(gameObject);
    }
}
