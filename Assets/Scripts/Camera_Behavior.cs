using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Camera_Behavior : MonoBehaviour
{
    public static Camera_Behavior Instance { get; private set; }
    public Camera cam;
    [SerializeField] float camSize;
    [SerializeField] float maxDist;

    public bool controlsEnabled = true;
    public bool follow = true;

    [SerializeField] private float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;
    private float zoomVel;
    private float zoomVel2;

    [SerializeField] private Transform player;
    [SerializeField] private Transform death;
    [SerializeField] private float deadZoneDist;
    private Vector3 targetPos;

    public bool FollowDeath;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        cam.orthographicSize = camSize;
    }

    public void OnToggleFollow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            follow = !follow;
        }
    }

    public void LockCursor(bool lockOn)
    {
        if (lockOn)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            controlsEnabled = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            controlsEnabled = false;
        } 
    }

    void FixedUpdate()//which to choose.....
    {
        if (player != null && death != null && follow)
        {
            if (FollowDeath)
            {
                targetPos = (player.position + death.position) / 2;
                transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPos.x, targetPos.y, -10f), ref velocity, smoothTime);
            }
            else//follow player normal, when slinging we 
            {
                /*var screenPos = cam.WorldToScreenPoint(player.position);
                screenPos.z = 0;
                var centerScreen = Vector2.zero;
                centerScreen.x = Screen.width / 2;
                centerScreen.y = Screen.height / 2;
                var dist = Vector3.Distance(centerScreen, screenPos);
                if (dist > deadZoneDist && Player.Instance.movement != Vector3.zero)
                {
                    targetPos = player.position;
                }*/

                targetPos = player.position;

                transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPos.x, targetPos.y, -10f), ref velocity, smoothTime);
                //Debug.Log($"distance: {dist}, player: {screenPos}");
            }
        }
        else if (player != null && follow)
        {
            Vector3 targetPosition = player.position;
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPosition.x, targetPosition.y, -10f), ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPos.x, targetPos.y, -10f), ref velocity, smoothTime);
        }
    }
}
