using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class InteractWith : MonoBehaviour
{
    public UnityEvent Interacts;

    public void DoInteract()
    {
        Interacts.Invoke();
    }

}
