using UnityEngine;

public class UITracking : MonoBehaviour
{
    Transform cameraTransform;
    void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.forward);
    }
}
