using UnityEngine;

public class Death : MonoBehaviour
{
    public static Death Instance { get; private set; }
    [SerializeField] GameObject player;
    [SerializeField] float knockbackPower;
    [SerializeField] Transform bodyRotator;
    public DirectionResolver directionResolver;
    public Rigidbody2D rb;
    public Animator specialAnimator;
    SpringJoint2D springJoint;
    Link link;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        link = GetComponent<Link>();
    }

    private void Update()
    {
        RotateBody();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void RotateBody()
    {
        bodyRotator.eulerAngles = new Vector3(0, 0, Vector3.SignedAngle(rb.linearVelocity, Vector3.up, Vector3.forward));
    }

    private void StartKnockback(Vector3 dir, bool normalized = true)
    {
        if (normalized)
        {
            rb.linearVelocity = dir.normalized * knockbackPower;
        }
        else
        {
            rb.linearVelocity = dir * knockbackPower;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        DamageArgs e = new DamageArgs();
        e.sender = transform;
        var hp = collision.transform.GetComponent<HealthManager>();

        if (collision.CompareTag("Enemy"))
        {
            var knockBack = player.transform.position - transform.position;
            if (!link.slinging)
            {
                StartKnockback(knockBack.normalized * 2, false);
            }
            else
            {
                StartKnockback(knockBack);
            }
            link.EndSling(false);
            if (Link.Instance.slingCooldown)
            {
                AudioManager.Instance.Play("Hit", transform.position, gameObject);
                specialAnimator.Play("Hit");
            }
        }

        if (hp != null)
        {
            hp.TakeDamage(1, e);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Link.Instance.slingCooldown)
        {
            AudioManager.Instance.Play("Hit", transform.position, gameObject);
            specialAnimator.Play("Hit");
        }
    }
}
