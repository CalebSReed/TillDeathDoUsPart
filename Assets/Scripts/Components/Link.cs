using UnityEngine;
using UnityEngine.InputSystem;

public class Link : MonoBehaviour
{
    public static Link Instance { get; private set; }

    [SerializeField] GameObject hinge;
    [SerializeField] SpringJoint2D spring;
    Rigidbody2D hingeRb;
    [SerializeField] float restingMaxDistance;
    [SerializeField] float startSlingMaxDistance;
    [SerializeField] float slingingMaxDistance;
    [SerializeField] float slingingMinDistance;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float forceMult;
    [SerializeField] float slingCorrectionForceMult;
    [SerializeField] float slingForceMult;

    [SerializeField] float defaultLinearDamping;
    [SerializeField] float slingLinearDamping;
    [SerializeField] float maxSlingCooldownTimer;//in seconds
    private float slingCooldownTimer;
    public bool slinging { get; private set; }
    public bool slingCooldown;
    float slingForce;
    float maxSlingForce = 1f;
    bool rightSided;
    private float distance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hingeRb = hinge.GetComponent<Rigidbody2D>();
    }

    public void OnSlingInput(InputAction.CallbackContext context)
    {
        if (Player.Instance.intro)
        {
            return;
        }

        if (context.performed && !slingCooldown && Player.Instance.controlsEnabled && !Player.Instance.cutscene)
        {
            StartSling();
        }
        else if (context.canceled && slinging)
        {
            EndSling();
        }
    }

    private void StartSling()
    {
        if (transform.position.x > hinge.transform.position.x)
        {
            rightSided = true;
        }
        else
        {
            rightSided = false;
        }

        slinging = true;
        slingCooldown = true;
        Player.Instance.speedMult = .25f;
        //Camera_Behavior.Instance.follow = false;
        rb.linearDamping = slingLinearDamping;
        spring.distance = distance;
        spring.enabled = true;
        Player.Instance.tugging = true;
        Player.Instance.frontAnimator.SetBool("isTugging", true);
        Player.Instance.backAnimator.SetBool("isTugging", true);
        Player.Instance.leftAnimator.SetBool("isTugging", true);
        Player.Instance.rightAnimator.SetBool("isTugging", true);
    }
    
    public void EndSling(bool sling = true)
    {
        spring.enabled = false;
        slinging = false;
        if (sling)
        {
            Sling();
        }
        slingForce = 0;
        Player.Instance.speedMult = 1f;
        //Camera_Behavior.Instance.follow = true;
        slingCooldownTimer = maxSlingCooldownTimer;
        Camera_Behavior.Instance.FollowDeath = true;
        Player.Instance.tugging = false;
        Player.Instance.frontAnimator.SetBool("isTugging", false);
        Player.Instance.backAnimator.SetBool("isTugging", false);
        Player.Instance.leftAnimator.SetBool("isTugging", false);
        Player.Instance.rightAnimator.SetBool("isTugging", false);
    }

    private void Sling()
    {
        AudioManager.Instance.Play("Launch", transform.position, gameObject, true);

        Vector3 dir = hinge.transform.position - transform.position;
        /*var force = (Quaternion.AngleAxis(90f, Vector3.forward) * dir.normalized) * slingForceMult;

        if (!rightSided)
        {
            force *= -1f;
        }*/

        var force = Camera_Behavior.Instance.cam.ScreenToWorldPoint(Player.Instance.playerInput.PlayerDefault.MousePosition.ReadValue<Vector2>()) - transform.position;
        force.z = 0;
        force = force.normalized * slingForce * slingForceMult;

        rb.linearVelocity = force;
        Debug.Log(force);
        Player.Instance.frontAnimator.SetBool("Throwing", true);
        Player.Instance.backAnimator.SetBool("Throwing", true);
        Player.Instance.leftAnimator.SetBool("Throwing", true);
        Player.Instance.rightAnimator.SetBool("Throwing", true);
        Death.Instance.directionResolver.showSpecial = false;
    }

    private void Update()
    {
        distance = Vector3.Distance(hinge.transform.position, transform.position);

        if (!Player.Instance.intro && Player.Instance.playerInput.PlayerDefault.Sling.ReadValue<float>() > 0f && !slinging && !slingCooldown && Player.Instance.controlsEnabled && !Player.Instance.cutscene)
        {
            StartSling();
        }

        RunSlingCooldownTimer();
        BuildUpSlingForce();
        CloseInSlingDistance();
    }

    void FixedUpdate()
    {
        if (slinging)
        {
            Vector3 dir = hinge.transform.position - transform.position;
            //Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            var force = (Quaternion.AngleAxis(90f, Vector3.forward) * dir.normalized) * forceMult * 100 * slingForce;

            if (!rightSided)
            {
                force *= -1f;
            }

            rb.AddForce(force);

            //at start max distance will be 1 unit higher, after a full second will be normal. This is to avoid death from instantly going towards player at start of sling
            /*if (distance > slingingMaxDistance + (1f - slingForce))
            {
                rb.AddForce(dir * slingCorrectionForceMult * slingForce);
            }*/
            /*else if (dist < slingingMinDistance)
            {
                rb.AddForce(-dir.normalized * slingCorrectionForceMult * slingForce * (1/(dist/slingingMinDistance)));
            }*/
        }
        else
        {
            if (distance > restingMaxDistance)
            {
                var force = (hinge.transform.position - transform.position) * distance;
                rb.AddForce(force);
                hingeRb.AddForce(-force * 3);
                //Debug.Log($"{force} is force, hinge: {hinge.transform.position}, and self: {transform.position}");
            }
        }
    }

    private void RunSlingCooldownTimer()
    {
        if (slingCooldownTimer > 0f)
        {
            slingCooldownTimer -= Time.deltaTime;
            if (slingCooldownTimer < 0f)//end timer
            {
                rb.linearDamping = defaultLinearDamping;
                slingCooldown = false;
                Camera_Behavior.Instance.FollowDeath = false;
                Death.Instance.directionResolver.showSpecial = true;
            }
        }
    }

    private void BuildUpSlingForce()
    {
        if (slinging && slingForce < maxSlingForce)
        {
            slingForce += Time.deltaTime * 2;
            if (slingForce > maxSlingForce)
            {
                slingForce = maxSlingForce;
            }
        }
    }

    private void CloseInSlingDistance()
    {    
        if (slinging && distance > slingingMinDistance)
        {
            spring.distance -= Time.deltaTime * 10;

            if (spring.distance < slingingMinDistance)
            {
                spring.distance = slingingMinDistance;
            }
        }
        else if (slinging && distance < slingingMinDistance)
        {
            spring.distance += Time.deltaTime * 2;

            if (spring.distance < slingingMinDistance)
            {
                spring.distance = slingingMinDistance;
            }
        }
    }
}
