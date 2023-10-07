using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;



public class CameraMovement : MonoBehaviour
{
    private NewControls cameraActions;
    private InputAction movement;
    private Transform cameraTransform;
    private Transform cameraTransform2;


    [Header("Horizontal Translation")]

    [SerializeField]
    private float maxSpeed = 5f;
    private float speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float damping = 15f;


    [Header("Vertical Translation")]
    [SerializeField]
    private float stepSize = 2f;
    [SerializeField]
    private float zoomDampening = 7.5f;
    [SerializeField]
    private float maxHeight = 10f;
    [SerializeField]
    private float minHeight = 10f;
    [SerializeField]
    private float zoomSpeed = 2f;


    [Header("Edge Tolerance")]
    [SerializeField]
    [Range(0f, 0.1f)]
    private float edgeTolarence = 0.01f;
    private bool useScreeEdge = true;
    private Vector3 targetPosition;
    private float zoomHeight;
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;
    Vector3 startDrag;

    private void Awake()
    {
        cameraActions = new NewControls();
        cameraTransform = this.GetComponentInChildren<Camera>().transform;
        cameraTransform2 = this.transform;
    }

    private void OnEnable()
    {
        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(this.transform);


        lastPosition = this.transform.position;

        movement = cameraActions.Gameplay.Move;
        cameraActions.Gameplay.ZoomCamera.performed += ZoomCamera;
        cameraActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        cameraActions.Gameplay.ZoomCamera.performed -= ZoomCamera;
        cameraActions.Gameplay.Disable();

    }
    void Update()
    {
        GetKeyboardMovement();
        if (useScreeEdge)
            CheckMouseAtScreenEdge();
        DragCamera();

        UpdateVelocity();
        UpdateBasePosition();
        UpdateCameraPosition();
    }


    private Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform2.forward;
        return forward;
    }
    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y = 0f;
        return right;
    }
    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * Time.deltaTime * speed;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }
        targetPosition = Vector3.zero;
    }


    private void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = this.transform.position;
    }
    private void ZoomCamera(InputAction.CallbackContext inputValue)
    {
        float value = -inputValue.ReadValue<Vector2>().y / 100f;
        if (Mathf.Abs(value) > 0.1f)
        {
            zoomHeight = cameraTransform.localPosition.y + value * stepSize;
            if (zoomHeight < minHeight)
                zoomHeight = minHeight;
            else if (zoomHeight > maxHeight)
                zoomHeight = maxHeight;
        }

    }
    private void UpdateCameraPosition()
    {

        Vector3 zoomTarget = new Vector3(cameraTransform.localPosition.x, zoomHeight, cameraTransform.localPosition.z);
        zoomTarget -= zoomSpeed * (zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);
        cameraTransform.LookAt(this.transform);
    }
    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight() + movement.ReadValue<Vector2>().y * GetCameraForward();
        inputValue = inputValue.normalized;
        if (inputValue.sqrMagnitude > 0.1f)
        {
            targetPosition += inputValue;
        }
    }

    private void CheckMouseAtScreenEdge()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x < edgeTolarence * Screen.width)
        {

            moveDirection += -GetCameraRight();
        }
        else if (mousePosition.x > (1f - edgeTolarence) * Screen.width)
        {

            moveDirection += GetCameraRight();
        }

        if (mousePosition.y < edgeTolarence * Screen.height)
        {

            moveDirection += -GetCameraForward();
        }
        else if (mousePosition.y > (1f - edgeTolarence) * Screen.height)
        {

            moveDirection += GetCameraForward();
        }

        targetPosition += moveDirection;
    }

    private void DragCamera()
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            return;
        }

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (plane.Raycast(ray, out float distance))
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                startDrag = ray.GetPoint(distance);
            else
                targetPosition += startDrag - ray.GetPoint(distance);
        }
    }


}
