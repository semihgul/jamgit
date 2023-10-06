using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class CameraMovement : MonoBehaviour
{
    private NewControls cameraActions;
    private InputAction movement;
    private Transform cameraTransform;

    [Header("Horizontal Translation")]

    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
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
    private float edgeTolarence = 0.1f;
    private Vector3 targetPosition;
    private float zoomHeight;
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;
    Vector3 startDrag;

    private void Awake()
    {
        cameraActions = new NewControls();
        cameraTransform = this.transform;
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

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        float inputValue= -obj.ReadValue<Vector2>().y/100f;
        if(Mathf.Abs(inputValue)>0.1f)
        {
            zoomHeight = cameraTransform.localPosition.y + inputValue * stepSize;
            if(zoomHeight<minHeight)
            zoomHeight=minHeight;
            else if(zoomHeight>maxHeight)
            zoomHeight=maxHeight;
        }
       
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        return forward;
    }
    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y=0f;
        return right;
    }

    private void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position-lastPosition)/Time.deltaTime;
        horizontalVelocity.y=0f;
        lastPosition = this.transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x*GetCameraRight()+movement.ReadValue<Vector2>().y*GetCameraForward();
        inputValue= inputValue.normalized;
        if(inputValue.sqrMagnitude>0.1f)
        {
            targetPosition+=inputValue;
        }   
    }

    private void UpdateBasePosition()
    {
        if(targetPosition.sqrMagnitude>0.1f)
        {
            speed=Mathf.Lerp(speed,maxSpeed,Time.deltaTime*acceleration)    ;
            transform.position+=targetPosition*Time.deltaTime*speed;
        }
        else
        {
            horizontalVelocity=Vector3.Lerp(horizontalVelocity,Vector3.zero,Time.deltaTime*damping);
            transform.position+=horizontalVelocity*Time.deltaTime;
        }
        targetPosition=Vector3.zero;
    }

    private void UpdateCameraPosition()
    {
        Vector3 zoomTarget= new Vector3 (cameraTransform.localPosition.x,zoomHeight,cameraTransform.localPosition.z);
        zoomTarget -= zoomSpeed*(zoomHeight-cameraTransform.localPosition.y)*Vector3.forward;
        cameraTransform.localPosition=Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime*zoomDampening);
        cameraTransform.LookAt(this.transform);
    }
    void Update()
    {
        GetKeyboardMovement();
        UpdateVelocity();
        UpdateBasePosition();
        UpdateCameraPosition();
    }
}
