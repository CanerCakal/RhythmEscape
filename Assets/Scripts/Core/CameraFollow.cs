using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 8, -8);

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}