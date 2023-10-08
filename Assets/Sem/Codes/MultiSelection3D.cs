using UnityEngine;
using UnityEngine.UI;

public class MultiSelection3D : MonoBehaviour
{
    public Camera cam;
    public LayerMask selectableMasks;

    public Image selectionBox;
    private Vector2 startPos;
    private Vector2 endPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BoxStart();
        }
        if (Input.GetMouseButton(0))
        {
            BoxWhile();
        }
        if (Input.GetMouseButtonUp(0))
        {
            BoxEnd();
        }
    }

    void BoxStart()
    {
        startPos = Input.mousePosition;
        selectionBox.gameObject.SetActive(true);
        selectionBox.rectTransform.sizeDelta = Vector2.zero;
    }

    void BoxWhile()
    {
        endPos = Input.mousePosition;
        Vector2 boxSize = new Vector2(Mathf.Abs(endPos.x - startPos.x), Mathf.Abs(endPos.y - startPos.y));
        Vector2 boxCenter = (startPos + endPos) / 2f;

        selectionBox.rectTransform.position = new Vector3(boxCenter.x, boxCenter.y, cam.nearClipPlane);
        selectionBox.rectTransform.sizeDelta = boxSize;
    }

    void BoxEnd()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 center = (hit.point + new Vector3(startPos.x, startPos.y, cam.nearClipPlane)) / 2f;
            Vector3 size = new Vector3(Mathf.Abs(hit.point.x - startPos.x), Mathf.Abs(hit.point.y - startPos.y), 0f);

            Collider[] colliders = Physics.OverlapBox(center, size / 2f, Quaternion.identity, selectableMasks);

            foreach (Collider collider in colliders)
            {
                GameObject objectInBox = collider.gameObject;
                objectInBox.GetComponent<InteractWith>().DoInteract();
                Debug.Log(objectInBox.name);
            }
        }

        selectionBox.gameObject.SetActive(false);
    }
}