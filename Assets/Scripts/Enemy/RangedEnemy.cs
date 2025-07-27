using UnityEngine;
using System.Collections.Generic;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float sightRange;
    [SerializeField] float maxSightRange;
    [SerializeField] float maxShootingFrequency;//in seconds
    [SerializeField] GameObject projectile;
    [SerializeField] Animator animator;
    public bool dropHealth;
    public bool dropKey;
    [SerializeField] List<GameObject> gibsList = new();

    float shootingProg;
    float shootingGoal;
    bool playerFound;
    Rigidbody2D rb;
    HealthManager hp;
    Vector3 movementDir;
    public bool canMove;

    [SerializeField] float checkDist;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            movementDir.x = 1;
            movementDir.y = 1;
        }
        else
        {
            movementDir.x = -1;
            movementDir.y = -1;
        }


        hp = GetComponent<HealthManager>();
        hp.OnDeath += Die;

        ResetShootProg();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSides();
        if (!playerFound)
        {
            FindPlayer();
        }
        else
        {
            CheckToLosePlayer();
        }
        
        if (shootingProg < shootingGoal)
        {
            shootingProg += Time.deltaTime;
            if (shootingProg > shootingGoal)
            {
                shootingProg -= shootingGoal;
                if (playerFound)
                {
                    animator.Play("Shoot");
                }
                ResetShootProg();
            }
        }
    }

    private void FixedUpdate()
    {
        MoveDiagonally();
    }

    private void MoveDiagonally()
    {
        if (canMove)
        {
            Vector3 newPos = transform.position + (movementDir * speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    private void FindPlayer()
    {
        if (Vector3.Distance(transform.position, Player.Instance.transform.position) < sightRange)
        {
            playerFound = true;
        }
    }

    private void CheckToLosePlayer()
    {
        if (Vector3.Distance(transform.position, Player.Instance.transform.position) > maxSightRange)
        {
            playerFound = false;
        }
    }
    
    private void ResetShootProg()
    {
        shootingGoal = maxShootingFrequency;
        float rand = Random.Range(-1f, 1f);
        shootingGoal += rand;
    }

    public void Shoot()
    {
        var proj = Instantiate(projectile, transform.position, Quaternion.identity);
        var dir = Player.Instance.transform.position - transform.position;
        proj.GetComponent<Projectile>().movementDir = dir.normalized * 20f;
        AudioManager.Instance.Play("Shoot", transform.position, gameObject);
    }

    private void CheckSides()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, checkDist);

        foreach(var hit in hits)
        {
            if (hit.transform.gameObject.layer == 6)
            {
                movementDir.y *= -1f;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.down, checkDist);

        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.layer == 6)
            {
                movementDir.y *= -1f;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.right, checkDist);

        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.layer == 6)
            {
                movementDir.x *= -1f;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.left, checkDist);

        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.layer == 6)
            {
                movementDir.x *= -1f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DamageArgs args = new();
            args.sender = transform;
            collision.GetComponent<HealthManager>().TakeDamage(1, args);
        }
    }

    private void Die(object sender, System.EventArgs e)
    {
        GameManager.Instance.AddKill();

        if (dropKey)
        {
            Instantiate(GameManager.Instance.keyObj, transform.position, Quaternion.identity);
        }

        if (dropHealth)
        {
            Instantiate(GameManager.Instance.hpObj, transform.position, Quaternion.identity);
        }

        foreach (var gib in gibsList)
        {
            Instantiate(gib, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
