using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

public class InteractablesManager : MonoBehaviour
{
    public Camera mainCamera;
    public UnityAction<Interactable> OnHoveredInteractableChanged; 
    public float raycastWidth = 0.25f;
    private float maxDistance = 2f;
    private Ray _ray;
    private RaycastHit _hit;
    private Interactable _currentInteractable;
    
    private void Update()
    {
        _ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(_ray.origin, _ray.direction, Color.green);

        if(Physics.SphereCast(_ray, raycastWidth, out _hit, maxDistance))
        {
            Interactable interactable = _hit.collider.GetComponent<Interactable>();
            if(!interactable) interactable = _hit.collider.GetComponentInParent<Interactable>();
            
            if(interactable)
            {
                if(interactable != _currentInteractable || _currentInteractable == null)
                {
                    _currentInteractable = interactable;
                    OnHoveredInteractableChanged.Invoke(_currentInteractable);
                }

                _currentInteractable.OnPlayerHover(_ray, _hit);
            }
        }
        else
        {
            _currentInteractable = null;
        }

        // Make sure the current interactable still exists
        if(!_currentInteractable)
        {
            OnHoveredInteractableChanged.Invoke(null);
        }
    }

}
