using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField, Min(1)] private float sensitivity = 100;

    private float mouseX, mouseY;

    private float distanceToTarget;

    public float smoothing = 5;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            Vector3 offset = transform.position - target.transform.position;
            distanceToTarget = offset.magnitude;

            Vector3 xzDirection = Vector3.ProjectOnPlane(offset, Vector3.up);
            mouseY = Vector3.Angle(offset, xzDirection);
        }
        
    }

    private void Update()
    {
        if (target != null)
        {
            mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            mouseY -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            mouseY = Mathf.Clamp(mouseY, 20, 70);
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Quaternion desiredRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(mouseY, mouseX, 0), smoothing * Time.deltaTime);

            transform.position = target.transform.position + desiredRotation * -Vector3.forward * distanceToTarget;
            transform.LookAt(target.transform);
        }
    }
}
