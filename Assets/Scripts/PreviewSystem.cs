using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPreview;
    private Material previewMaterialIntance;
    private Renderer cellIndicatorRenderer;

    private Color red = new Color(1, 0, 0, 0.1f);
    private Color white = new Color(1, 1, 1, 0.1f);

    void Start()
    {
        previewMaterialIntance = new Material(previewMaterialPreview);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreavie(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PreparePreavie(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialIntance;
            }
            renderer.materials = materials;
        }
    }
    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 && size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    public void StopShowingPlacementPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        MovePreview(position);
        MoveCursor(position);
        ApplyFeeedBack(validity);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewOffset, position.z);
    }

    private void MoveCursor(Vector3 position)
    {
        if(cellIndicator.transform.localScale!=new Vector3(3,1,3))
        cellIndicator.transform.position = position + new Vector3(0, -0.99f, 0f);
        else
        cellIndicator.transform.position = position + new Vector3(1, -0.99f, 1f);
    }

    private void ApplyFeeedBack(bool validity)
    {

        Color c = validity ? white : red;
        previewMaterialPreview.color = c;
        cellIndicatorRenderer.material.color = c;


    }
}
