using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractablesManager : MonoBehaviour
{
    public Camera mainCamera;
    public UnityAction<Interactable> OnHoveredInteractableChanged; 
    private float raycastWidth = 0.5f;
    private Ray _ray;
    private RaycastHit _hit;
    private Interactable _currentInteractable;

    private void Start()
    {
        _ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
    }

    private void Update()
    {
        if(!Physics.SphereCast(_ray, raycastWidth, out _hit)) return;
        Interactable interactable = _hit.collider.GetComponent<Interactable>();

        if(!interactable)
        {
            // Changed to no interactable
            if(_currentInteractable) OnHoveredInteractableChanged.Invoke(null);
            
            _currentInteractable = null;
            return;
        }

        if(interactable != _currentInteractable)
        {
            _currentInteractable = interactable;
            OnHoveredInteractableChanged.Invoke(_currentInteractable);
        }

        _currentInteractable.OnPlayerHover(_ray, _hit);
    }
}
