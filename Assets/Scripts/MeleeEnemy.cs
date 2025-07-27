using UnityEngine;
using System.Collections.Generic;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float detectionDistance;
    [SerializeField] float followDistance;
    [SerializeField] Animator animator;
    public bool dropHealth;
    public bool dropKey;
    [SerializeField] List<GameObject> gibsList = new();

    bool playerFound;
    Rigidbody2D rb;
    HealthManager hp;
    Vector3 movement;
    bool waitToMove;
    float waitTimer;
    float waitGoal;

    bool patrolling;
    float patrolTimer;
    float patrolGoal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = GetComponent<HealthManager>();

        hp.OnDeath += Die;

        waitGoal = Random.Range(1f, 2f);
        waitToMove = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerFound)
        {
            MoveTowardsPlayer();
            KeepPursuit();
        }
        else
        {
            SearchForPlayer();
            if (patrolling)
            {
                Patrolling();
            }

            if (patrolling && patrolTimer < patrolGoal)
            {
                patrolTimer += Time.fixedDeltaTime;
                if (patrolTimer >= patrolGoal)
                {
                    Wait();
                    patrolling = false;
                    animator.SetBool("isMoving", false);
                }
            }

            if (waitToMove && waitTimer < waitGoal)
            {
                waitTimer += Time.fixedDeltaTime;
                if (waitTimer >= waitGoal)
                {
                    Patrol();
                    waitToMove = false;
                }
            }
        }

        if (movement.x >= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Patrolling()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }

    private void MoveTowardsPlayer()
    {
        var newPos = Player.Instance.transform.position - transform.position;
        rb.MovePosition(transform.position + newPos.normalized * speed * Time.fixedDeltaTime);
        movement = newPos;
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

    private void KeepPursuit()
    {
        if (Vector3.Distance(Player.Instance.transform.position, transform.position) > followDistance)
        {
            playerFound = false;
            waitToMove = true;
            animator.SetBool("isMoving", false);
        }
    }

    private void SearchForPlayer()
    {
        if (Vector3.Distance(Player.Instance.transform.position, transform.position) <= detectionDistance)
        {
            playerFound = true;
            ResetTimers();
            animator.SetBool("isMoving", true);
        }
    }

    private void ResetTimers()
    {
        waitTimer = 0f;
        waitToMove = false;
        patrolling = false;
        animator.SetBool("isMoving", false);
        patrolTimer = 0f;
    }

    private void Wait()
    {
        waitToMove = true;
        waitTimer = 0f;
        waitGoal = Random.Range(1f, 2f);
    }

    private void Patrol()
    {
        patrolling = true;
        animator.SetBool("isMoving", true);
        movement.x = Random.Range(-1f, 1f);
        movement.y = Random.Range(-1f, 1f);
        movement.Normalize();

        patrolTimer = 0f;
        patrolGoal = Random.Range(1f, 2f);
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
