using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] GameObject topRoom;
    [SerializeField] GameObject bottomRoom;
    [SerializeField] GameObject leftRoom;
    [SerializeField] GameObject rightRoom;
    [SerializeField] GameObject doorLock;

    [SerializeField] float timer;
    [SerializeField] float timerGoal;
    [SerializeField] GameObject physicalCollider;
    [SerializeField] Animator animator;
    public bool locked;
    public bool keyLocked;
    bool cooldown;
    Vector3 movement;

    private void FixedUpdate()
    {
        if (cooldown && timer < timerGoal)
        {
            timer += Time.fixedDeltaTime;
            Player.Instance.movement = movement;
            var newPos = Player.Instance.transform.position - movement * 2.5f;
            Death.Instance.transform.position = newPos;
            if (timer > timerGoal)
            {
                timer = 0f;
                cooldown = false;
                Player.Instance.cutscene = false;
                physicalCollider.SetActive(true);
            }
        }
    }

    public void DoorCoolDown()
    {
        cooldown = true;
    }

    private void ForceCutScene(string side)
    {
        Player.Instance.cutscene = true;
        Link.Instance.EndSling();
        physicalCollider.SetActive(false);

        if (side == "DoorTop")
        {
            movement = new Vector2(0,-1);
        }
        else if (side == "DoorLeft")
        {
            movement = new Vector2(1, 0);
        }
        else if (side == "DoorRight")
        {
            movement = new Vector2(-1, 0);
        }
        else if (side == "DoorBottom")
        {
            movement = new Vector2(0, 1);
        }
    }

    public void OnDoorEntered(string side)
    {
        if (cooldown || locked || keyLocked)
        {
            return;
        }

        if (side == "DoorTop")
        {
            bottomRoom.SetActive(true);
            topRoom.SetActive(false);
            bottomRoom.GetComponent<RoomBehavior>().EnterRoom();
        }
        else if (side == "DoorLeft")
        {
            rightRoom.SetActive(true);
            leftRoom.SetActive(false);
            rightRoom.GetComponent<RoomBehavior>().EnterRoom();
        }
        else if (side == "DoorRight")
        {
            leftRoom.SetActive(true);
            rightRoom.SetActive(false);
            leftRoom.GetComponent<RoomBehavior>().EnterRoom();
        }
        else if (side == "DoorBottom")
        {
            topRoom.SetActive(true);
            bottomRoom.SetActive(false);
            topRoom.GetComponent<RoomBehavior>().EnterRoom();
        }
        else
        {
            Debug.LogError("Incorrect tag name for door collider!");
            return;
        }
        DoorCoolDown();
        ForceCutScene(side);
    }

    public void KeyUnlock()
    {
        locked = false;
        keyLocked = false;
        animator.SetBool("isLocked", false);
    }

    public void LockDoor()
    {
        if (keyLocked)
        {
            return;
        }

        locked = true;
        if (animator != null)
        {
            animator.SetBool("isLocked", true);
        }
    }

    public void UnlockDoor()
    {
        if (keyLocked)
        {
            return;
        }

        locked = false;
        if (animator != null)
        {
            animator.SetBool("isLocked", false);
        }
    }
}
