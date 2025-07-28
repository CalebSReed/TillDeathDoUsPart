using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public PlayerInputActions playerInput;
    public Vector3 movement;
    public float speed;
    public float speedMult;
    [SerializeField] int maxHealth;
    [SerializeField] Rigidbody2D rb;
    public HealthManager hp;
    [SerializeField] TextMeshProUGUI hpText;
    public Animator specialAnimator;
    [SerializeField] Transform bodyRotator;
    [SerializeField] Transform spriteHolder;
    public bool controlsEnabled;
    public bool cutscene;
    public bool tugging;
    public bool throwing;
    public bool intro;

    [SerializeField] DirectionResolver directionResolver;
    public Animator frontAnimator;
    public Animator backAnimator;
    public Animator leftAnimator;
    public Animator rightAnimator;

    [SerializeField] GameObject keyUI;
    public bool hasKey;

    Vector3 knockbackDir;
    float knockbackProg;
    bool knockback;
    bool dead;
    [SerializeField] float maxInvincibilityTimer;
    float invincibilityTimer;
    bool flicker;

    private void Awake()
    {
        Instance = this;
        playerInput = new PlayerInputActions();
        playerInput.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp.SetHealth(maxHealth);
        hp.OnDamageTaken += OnDamaged;
        hp.OnHealed += UpdateHealth;
        if (!intro)
        {
            hpText.text = $"Health: {hp.currentHealth}/{hp.maxHealth}";
        }
    }

    void UpdateHealth(object sender, System.EventArgs e)
    {
        hpText.text = $"Health: {hp.currentHealth}/{hp.maxHealth}";
    }

    // Update is called once per frame
    void Update()
    {
        ReadMovement();
        RotateBody();
        RunInvincibilityTimer();
    }

    private void FixedUpdate()
    {
        DoMovement();

        if (knockback)
        {
            DoKnockBack();
        }
    }

    void ReadMovement()
    {
        if (!controlsEnabled)
        {
            return;
        }

        if (!cutscene)
        {
            movement = playerInput.PlayerDefault.Movement.ReadValue<Vector2>();
        }

        if (movement.x != 0 || movement.y != 0)
        {
            frontAnimator.SetBool("isRunning", true);
            backAnimator.SetBool("isRunning", true);
            leftAnimator.SetBool("isRunning", true);
            rightAnimator.SetBool("isRunning", true);
        }
        else
        {
            frontAnimator.SetBool("isRunning", false);
            backAnimator.SetBool("isRunning", false);
            leftAnimator.SetBool("isRunning", false);
            rightAnimator.SetBool("isRunning", false);
        }
    }

    private void RotateBody()
    {
        if (tugging || throwing)
        {
            var pos = Death.Instance.transform.position - transform.position;
            var angle = Vector3.SignedAngle(pos.normalized, Vector3.up, Vector3.forward);
            bodyRotator.eulerAngles = new Vector3(0, 0, angle);
        }
        else if (!knockback && movement.x != 0 || !knockback && movement.y != 0)
        {
            var angle = Vector3.SignedAngle(movement, Vector3.up, Vector3.forward);
            bodyRotator.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    void DoMovement()
    {   
        if (controlsEnabled)
        {
            Vector3 newPos = transform.position + movement * speed * speedMult * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    private void DoKnockBack()
    {
        knockbackProg -= Time.fixedDeltaTime * 4f;
        if (knockbackProg <= 0f)
        {
            knockback = false;
        }
        rb.MovePosition(transform.position + knockbackDir * knockbackProg);
        bodyRotator.eulerAngles = new Vector3(0, 0, Vector3.SignedAngle(knockbackDir.normalized, Vector3.up, Vector3.forward));
    }

    private void KnockBack(Vector3 dir)
    {
        knockbackDir = dir.normalized;
        knockbackProg = 1f;
        knockback = true;
    }

    private void OnDamaged(object sender, DamageArgs e)
    {
        if (dead)
        {
            return;
        }

        if (hp.currentHealth <= 0)
        {
            dead = true;
            playerInput.PlayerDefault.Disable();
            AudioManager.Instance.Play("Die", transform.position, gameObject, true, false, false);
            AudioManager.Instance.Stop("Level");
            specialAnimator.Play("Die");
        }
        else
        {
            AudioManager.Instance.Play("Hurt", transform.position, gameObject, true, false, false);
            specialAnimator.Play("PlayerDamaged");
        }
 
        var dir = transform.position - e.sender.position;
        KnockBack(dir);
        hp.isInvincible = true;
        hp.invincibilityOnTimer = true;
        hpText.text = $"Health: {hp.currentHealth}/{hp.maxHealth}";
    }

    private void RunInvincibilityTimer()
    {
        if (hp.invincibilityOnTimer && !dead)
        {
            if (invincibilityTimer >= maxInvincibilityTimer)
            {
                hp.invincibilityOnTimer = false;
                hp.isInvincible = false;
                invincibilityTimer = 0f;
                spriteHolder.transform.localScale = new Vector3(1, 1, 1);
                return;
            }
            else
            {
                invincibilityTimer += Time.deltaTime;
            }
            flicker = !flicker;
            if (flicker)
            {
                spriteHolder.transform.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                spriteHolder.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Health") && !hp.cooldown)
        {
            AudioManager.Instance.Play("Heal", transform.position, gameObject, true, false, true);
            hp.RestoreHealth(1);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Key"))
        {
            AudioManager.Instance.Play("Key", transform.position, gameObject, true, false, true);
            hasKey = true;
            keyUI.SetActive(true);
            Destroy(collision.gameObject);
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasKey && collision.collider.CompareTag("KeyDoor"))
        {
            if (collision.transform.GetComponentInParent<DoorBehavior>().keyLocked)
            {
                collision.transform.GetComponentInParent<DoorBehavior>().KeyUnlock();
                collision.transform.GetComponentInParent<DoorBehavior>().OnDoorEntered("DoorBottom");

                hasKey = false;
                keyUI.SetActive(false);
            }
        }
    }
}
