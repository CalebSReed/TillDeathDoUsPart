using UnityEngine;

public class Giblet : MonoBehaviour
{
    [SerializeField] float forceMult;

    private void Start()
    {
        Vector3 dir = Vector3.zero;
        dir.x = Random.Range(-1f, 1f);
        dir.y = Random.Range(-1f, 1f);
        GetComponent<Rigidbody2D>().AddForce(dir.normalized * forceMult, ForceMode2D.Impulse);
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
