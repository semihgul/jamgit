using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Team
{
    Blue,
    Red
}

public class AI_Piyade : MonoBehaviour
{
    public Team team;
    public float detectionRadius = 10f;

    private NavMeshAgent agent;
    private Camera cam;
    private bool isSelected;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = FindObjectOfType<Camera>();
    }

    public void Interaction()
    {
        isSelected = true;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isSelected)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cam.transform.position.y; // Fare pozisyonunu kameranın yüksekliği seviyesine getirin
            Vector3 targetPosition = cam.ScreenToWorldPoint(mousePosition);

            agent.SetDestination(targetPosition);
        }

        // Hedefe doğru hareket etmek için NavMeshAgent kullanılıyor

        // Belirli bir yarıçap içindeki düşmanları algılama
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in colliders)
        {
            AI_Piyade enemySoldier = collider.GetComponent<AI_Piyade>();
            if (enemySoldier != null && enemySoldier.team != team)
            {
                // Düşman askerleri tespit edildiğinde saldırı yapabilirsiniz
                Attack(collider.gameObject);
            }
        }
    }

    private void Attack(GameObject enemy)
    {
        // Düşmana saldırma kodunu buraya yazabilirsiniz
        Debug.Log("Attacking enemy: " + enemy.name);
    }
}