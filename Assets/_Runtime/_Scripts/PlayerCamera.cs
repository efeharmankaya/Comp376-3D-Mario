// ------------------------------------------------------------------------------
// Quiz
// Written by: Efe Harmankaya - 40077277
// For COMP 376 – Fall 2021
// Controls the 3rd person player camera along with additional repositioning logic to avoid obstacles w/ raycasting
// -----------------------------------------------------------------------------

using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField, Min(1)] private float sensitivity = 100;

    private float mouseX, mouseY;

    private float distanceToTarget;

    public float smoothing = 5;

    public Transform newRaycastPosition;

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

        checkRayCast();
    }

    // Reference ideas
    // https://forum.unity.com/threads/raycast-coming-from-center-of-camera.321510/
    // https://forum.unity.com/threads/navmesh-and-raycast-obstacle-avoidance.135090/
    private void checkRayCast(){
        RaycastHit HitInfo;

        Transform cameraTransform = Camera.main.transform;

        // center raycast
        if(Physics.Raycast(cameraTransform.position,cameraTransform.forward, out HitInfo, 100.0f)){
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100.0f, Color.yellow);
            Transform objectHit = HitInfo.transform;
            if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // center raycast hit is not the player
                mouseX += 2f;
            }
        }


        // left center raycast
        newRaycastPosition = cameraTransform;
        newRaycastPosition.Translate(-1f, 0f, 0f);
        Vector3 newDir = Vector3.RotateTowards(newRaycastPosition.forward, target.transform.position - newRaycastPosition.position, 1f, 0f);
        if(Physics.Raycast(newRaycastPosition.position, newDir, out HitInfo, 100.0f)){
            Debug.DrawRay(newRaycastPosition.position, newDir * 100.0f, Color.red);
            Transform objectHit = HitInfo.transform;
            if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
                mouseX -= 1f;
            }
        }
        
        // right center raycast
        newRaycastPosition = cameraTransform;
        newRaycastPosition.Translate(2f, 0f, 0f);
        newDir = Vector3.RotateTowards(newRaycastPosition.forward, target.transform.position - newRaycastPosition.position, 1f, 0f);
        if(Physics.Raycast(newRaycastPosition.position, newDir, out HitInfo, 100.0f)){
            Debug.DrawRay(newRaycastPosition.position, newDir * 100.0f, Color.red);
            Transform objectHit = HitInfo.transform;
            if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
                mouseX += 1f;
            }
        }

        // top center raycast
        newRaycastPosition = cameraTransform;
        newRaycastPosition.Translate(-1f, 1f, 0f);
        newDir = Vector3.RotateTowards(newRaycastPosition.forward, target.transform.position - newRaycastPosition.position, 1f, 0f);
        if(Physics.Raycast(newRaycastPosition.position, newDir, out HitInfo, 100.0f)){
            Debug.DrawRay(newRaycastPosition.position, newDir * 100.0f, Color.red);
            Transform objectHit = HitInfo.transform;
            if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
                mouseY -= 1f;
            }
        }

        // // bottom center raycast
        // newRaycastPosition = cameraTransform;
        // newRaycastPosition.Translate(0f, -2f, 0f);
        // newDir = Vector3.RotateTowards(newRaycastPosition.forward, target.transform.position - newRaycastPosition.position, 1f, 0f);
        // if(Physics.Raycast(newRaycastPosition.position, newDir, out HitInfo, 100.0f)){
        //     Debug.DrawRay(newRaycastPosition.position, newDir * 100.0f, Color.red);
        //     Transform objectHit = HitInfo.transform;
        //     if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
        //         mouseY += 1f;
        //     }
        // }


        // rightCenter = cameraTransform;
        // rightCenter.Translate(0.5f, 0f, 0f);
        // rightCenter.RotateTowards(target.transform);
        // // right center raycast
        // if(Physics.Raycast(rightCenter.position,rightCenter.forward, out HitInfo, 100.0f)){
        //     Debug.DrawRay(rightCenter.position, rightCenter.forward * 100.0f, Color.red);
        //     Transform objectHit = HitInfo.transform;
        //     if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
        //         mouseX += 1f;
        //     }
        // }

        // // top center raycast
        // if(Physics.Raycast(topCenter.position,topCenter.forward, out HitInfo, 100.0f)){
        //     Debug.DrawRay(topCenter.position, topCenter.forward * 100.0f, Color.red);
        //     Transform objectHit = HitInfo.transform;
        //     if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
        //         mouseY -= 1f;
        //     }
        // }

        // // bottom center raycast
        // if(Physics.Raycast(bottomCenter.position,bottomCenter.forward, out HitInfo, 100.0f)){
        //     Debug.DrawRay(bottomCenter.position, bottomCenter.forward * 100.0f, Color.red);
        //     Transform objectHit = HitInfo.transform;
        //     if(!HitInfo.collider.gameObject.tag.Equals("Player")){ // raycast hit is not the player
        //         mouseY += 1f;
        //     }
        // }
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
