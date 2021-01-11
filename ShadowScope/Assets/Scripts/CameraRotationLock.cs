using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationLock : MonoBehaviour
{
    private Quaternion my_rotation;
    // Start is called before the first frame update
    private void Start()
    {
        my_rotation = this.transform.rotation;
    }

    private void LateUpdate()
    {
        this.transform.rotation = my_rotation;
    }
}
