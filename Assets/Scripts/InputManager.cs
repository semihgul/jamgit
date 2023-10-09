using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;
    [SerializeField]
    private LayerMask buildMasks;
    public GameObject[] createMenu;
    public event Action OnClicked, OnExit;
    [SerializeField]
    private ObjectsDataBaseSO database;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointOverUI()
    => EventSystem.current.IsPointerOverGameObject();
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
    
    public void OpenCreateMenu()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        //GridData selectedData = database.objectData[selecetedObjectIndex].ID;
        if (Physics.Raycast(ray, out hit, 100, buildMasks))
        {
            EnableUI(0);
        }
    }

    private void EnableUI(int iD)
    {
        switch (iD)
        {
            case 0:
            createMenu[0].SetActive(true);
            break;
            case 1:
            createMenu[1].SetActive(true);
            break;
            default:
            break;
        }
    }
}
