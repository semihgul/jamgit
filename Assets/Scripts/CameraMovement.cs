using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;
using Unity.VisualScripting;
using Photon.Pun;

using Photon.Realtime;


public class CameraMovement : MonoBehaviour
{
    private NewControls cameraActions;
    private InputAction movement;
    private Transform cameraTransform;
    private Transform cameraTransform2;
    public Transform target;
    private float _camSize;
    private Camera _cam;
    public AnimationCurve zoomCurve;




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
    private float maxHeight = 11f;
    [SerializeField]
    private float minHeight = 6f;
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

    bool zooming;

    private void Awake()
    {
        cameraActions = new NewControls();
        cameraTransform = this.GetComponentInChildren<Camera>().transform;
        cameraTransform2 = this.transform;
        _cam = this.GetComponentInChildren<Camera>();
        _camSize = this.GetComponentInChildren<Camera>().orthographicSize;
         /*if(!this.GetComponent<PhotonView>().IsMine)
        {
            this.GetComponent<CameraMovement>().enabled = false;
            cameraTransform.gameObject.SetActive(false);
        }*/
    }

    private void OnEnable()
    {
        zoomHeight = this.GetComponentInChildren<Camera>().orthographicSize;
        cameraTransform.LookAt(target);


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
            zoomHeight = _camSize + value * stepSize;
            if (zoomHeight < minHeight)
            {
                zoomHeight = minHeight;
            }

            else if (zoomHeight > maxHeight)
            {
                zoomHeight = maxHeight;
                
            }
        }

    }
    private void UpdateCameraPosition()
    {
        if (zooming)
        {
            return;
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0.1f)
        {
            float targetZoom = _cam.orthographicSize - (scrollDelta * zoomSpeed);
            targetZoom = Mathf.Clamp(targetZoom, minHeight, maxHeight);
            StartCoroutine(ZoomCamera(targetZoom));
        }
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

    public IEnumerator ZoomCamera(float targetZoom)
    {
        zooming = true;
        float startZoom = _cam.orthographicSize;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * zoomSpeed;
            float zoomAmount = Mathf.Lerp(startZoom, targetZoom, zoomCurve.Evaluate(timer));
            _cam.orthographicSize = zoomAmount;
            yield return null;
        }
        zooming = false;
    }
}
