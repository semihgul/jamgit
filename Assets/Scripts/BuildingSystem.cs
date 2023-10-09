using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSystem : MonoBehaviour
{
    public InputManager inputManager;
    public PlacementSystem PS;

    public int ID;
    void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        PS = GameObject.FindGameObjectWithTag("Building").GetComponent<PlacementSystem>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            inputManager.OpenCreateMenu();
    }

}
