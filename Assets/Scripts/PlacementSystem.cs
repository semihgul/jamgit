using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject cellIndicator;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDataBaseSO database;
    private int selecetedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData,
                     furnitureData; 
    private Renderer previewRenderer;
    private List<GameObject> placedGameObject = new();

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selecetedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selecetedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    void PlaceStructure()
    {
        if (inputManager.IsPointOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selecetedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }
        
        GameObject newObject = Instantiate(database.objectData[selecetedObjectIndex].prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition) + new Vector3(0f, -0.1f, 2f);
        placedGameObject.Add(newObject);
        GridData selectedData = database.objectData[selecetedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            database.objectData[selecetedObjectIndex].Size,
            database.objectData[selecetedObjectIndex].ID,
            placedGameObject.Count - 1
        );
    }

    bool CheckPlacementValidity(Vector3Int gridPosition, int selecetedObjectIndex)
    {
        GridData selectedData = database.objectData[selecetedObjectIndex].ID == 0? floorData:furnitureData;
            
        return selectedData.CanPlaceObjectAt(
            gridPosition,
            database.objectData[selecetedObjectIndex].Size
        );
    }

    void StopPlacement()
    {
        selecetedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    void Update()
    {
        if (selecetedObjectIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        bool placementValidity = CheckPlacementValidity(gridPosition, selecetedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        cellIndicator.transform.position =
            grid.CellToWorld(gridPosition) + new Vector3(1f, -0.99f, 1f);
    }
}
