
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    // The camera should be the child of rig object.
    // y:50 rotation x :45
    [SerializeField] private Camera rigCamera;
    public Transform cameraTransform;
    
    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    
    public Vector3 zoomAmount = new(0,10,-10);
    public Vector3 newZoom;
    public Vector3 newPosition;
    public Quaternion newRotation;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }


    void LateUpdate()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    private void HandleMouseInput()
    {
        MouseZoom();
        
        if (MouseMovement()) return;

        MouseRotation();
    }

    private void MouseRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 deltaPosition = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-deltaPosition.x / 5f));
        }
    }

    private bool MouseMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var plane = new Plane(Vector3.up, Vector3.zero);

            var ray = rigCamera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            {
                var plane = new Plane(Vector3.up, Vector3.zero);

                var ray = rigCamera.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (!plane.Raycast(ray, out entry)) return true;
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        return false;
    }

    private void MouseZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
    }

    private void HandleKeyboardInput()
    {
        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation*=Quaternion.Euler(Vector3.up*rotationAmount);
        }
        
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation*=Quaternion.Euler(Vector3.up*-rotationAmount);
        }
        
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition =
            Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
