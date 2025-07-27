using UnityEngine;

public class DoorCollision : MonoBehaviour
{
    [SerializeField] DoorBehavior door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            door.OnDoorEntered(gameObject.tag);
        }
    }
}
