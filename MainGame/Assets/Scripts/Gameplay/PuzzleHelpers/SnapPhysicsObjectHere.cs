using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For snapping a PhysicsInteractable to a position and raising an event
public class SnapPhysicsObjectHere : MonoBehaviour
{
    public GameEvent eventOnPlaced;
    public PhysicsInteractable interactableThatGoesHere;
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        PhysicsInteractable i = go.GetComponent<PhysicsInteractable>();
        if(!i) i = go.GetComponentInParent<PhysicsInteractable>();
        if(!i) return;

        if(i == interactableThatGoesHere)
        {
            Rigidbody rb = i.GetComponent<Rigidbody>();

            rb.isKinematic = true;

            i.transform.position = transform.position + offsetPosition;
            i.transform.rotation = transform.rotation * Quaternion.Euler(offsetRotation);

            eventOnPlaced.Raise();

            // Object is no longer interactable after it's placed here
            Destroy(i);
        }
    }
}
