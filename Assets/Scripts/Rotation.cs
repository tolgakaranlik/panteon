using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float RotationSpeed = 0.5f;
    public Vector3 RotationVec = Vector3.one;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(RotationVec.x * RotationSpeed * Time.deltaTime, RotationVec.y * RotationSpeed * Time.deltaTime, RotationVec.z * RotationSpeed * Time.deltaTime);
    }
}
