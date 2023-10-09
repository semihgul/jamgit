using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDataBaseSO database;
    private int selecetedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GameObject buildMenu;

    private GridData floorData,
                     furnitureData;

    private List<GameObject> placedGameObject = new();
    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetecedGridPosition = Vector3Int.zero;
    public bool isBuilding;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        InstanteTownCenter();

    }
    public void OpenBuildMenu()
    {
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        isBuilding = true;
        selecetedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selecetedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(database.objectData[selecetedObjectIndex].prefab,
                                             database.objectData[selecetedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    public void InstanteTownCenter()
    {
        Vector3Int gridPosition = grid.WorldToCell(new Vector3(-2, 0, -2));
        GameObject newObject = Instantiate(database.objectData[0].prefab);
        placedGameObject.Add(newObject);
        GridData selectedData = furnitureData;
        selectedData.AddObjectAt(
        gridPosition,
        database.objectData[0].Size,
        database.objectData[0].ID,1
                                );
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
        newObject.transform.position = grid.CellToWorld(gridPosition);


        placedGameObject.Add(newObject);
        GridData selectedData = database.objectData[selecetedObjectIndex].ID == 4 ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            database.objectData[selecetedObjectIndex].Size,
            database.objectData[selecetedObjectIndex].ID,
            placedGameObject.Count - 1
        );
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    bool CheckPlacementValidity(Vector3Int gridPosition, int selecetedObjectIndex)
    {
        GridData selectedData = database.objectData[selecetedObjectIndex].ID == 0 ? floorData : furnitureData;

        return selectedData.CanPlaceObjectAt(
            gridPosition,
            database.objectData[selecetedObjectIndex].Size
        );
    }

    void StopPlacement()
    {
        selecetedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPlacementPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetecedGridPosition = Vector3Int.zero;
        isBuilding = false;
    }

    void Update()
    {
        if (selecetedObjectIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetecedGridPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selecetedObjectIndex);
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetecedGridPosition = gridPosition;

        }
    }
}
