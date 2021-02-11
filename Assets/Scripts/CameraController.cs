using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform ObjectToFollow;

    // Camera will be updated on LateUpdate instead of Update or FixedUpdate in order
    // to avoid jitter, flicker or weird character distance
    void LateUpdate()
    {
        transform.position = new Vector3(0.0f, 0.31f, ObjectToFollow.position.z - 0.45f);
    }
}
