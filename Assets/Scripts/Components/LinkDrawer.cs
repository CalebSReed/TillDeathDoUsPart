using UnityEngine;

public class LinkDrawer : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform death;
    LineRenderer lineRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, player.position);
        lineRenderer.SetPosition(1, death.position);
    }
}
