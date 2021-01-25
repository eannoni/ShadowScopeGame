using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public PlayerController player;
    public float speed;
    public Camera cam;
    public Vector3 zoomedOut;
    public Vector3 zoomedIn;

    private void LateUpdate()
    {
        Vector3 desiredPosition;
        if (player.crouching)
        {
            desiredPosition = player.transform.position + zoomedOut;
        }
        else
        {
            desiredPosition = player.transform.position + zoomedIn;
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, speed);
        transform.position = smoothedPosition;
    }
}
