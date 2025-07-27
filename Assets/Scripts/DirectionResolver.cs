using UnityEngine;

public class DirectionResolver : MonoBehaviour
{
    [SerializeField] Transform special;
    [SerializeField] Transform front;
    [SerializeField] Transform back;
    [SerializeField] Transform left;
    [SerializeField] Transform right;

    public bool showSpecial;

    public bool upSwing;

    void Update()
    {
        if (showSpecial)
        {
            special.localScale = Vector3.one;
            back.localScale = Vector3.zero;
            left.localScale = Vector3.zero;
            right.localScale = Vector3.zero;
            front.localScale = Vector3.zero;

            if (transform.rotation.eulerAngles.z < 180)
            {
                special.localScale = new Vector3(-1, 1, 1);
            }

            return;
        }
        special.localScale = Vector3.zero;

        var angle = transform.rotation.eulerAngles.z;

        // 315-44 is back, 45-134 is side 135-180 is front
        if (angle < 45 || angle >= 315)
        {
            if (!upSwing)
            {
                AudioManager.Instance.Play("Swing", transform.position, gameObject, true);
            }
            upSwing = true;
            back.localScale = Vector3.one;
            left.localScale = Vector3.zero;
            right.localScale = Vector3.zero;
            front.localScale = Vector3.zero;
        }
        else if (angle >= 45 && angle < 135)//45 - 135
        {
            right.localScale = Vector3.one;
            back.localScale = Vector3.zero;
            left.localScale = Vector3.zero;
            front.localScale = Vector3.zero;
        }
        else if (angle >= 135 && angle < 225)//135 - 225
        {
            upSwing = false;
            front.localScale = Vector3.one;
            back.localScale = Vector3.zero;
            left.localScale = Vector3.zero;
            right.localScale = Vector3.zero;
        }
        else if (angle >= 225 && angle < 315)
        {
            left.localScale = Vector3.one;
            back.localScale = Vector3.zero;
            right.localScale = Vector3.zero;
            front.localScale = Vector3.zero;
        }
    }
}
