using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsInteractable : Interactable
{
    private Rigidbody _rigidbody;
    private RigidbodyInterpolation _initialInterp;
    // Unused but useful for rotation
    private Vector3 _rotationDifferenceEuler;
    private bool _active;
    private float _currentGrabDistance;

    private void Start()
    {
        _rigidbody = gameObject.GetComponentOrThrow<Rigidbody>();
    }
    
    public override void OnPlayerHover(Ray ray, RaycastHit hit)
    {
        if(_active) return;
        // No support for throwing yet but just put this somewhere in response to an input action and it will work
        // rigidbody.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

        if(!Input.GetMouseButtonDown(0)) return;
        _active = true;

        Transform camera = Camera.main.transform;

        // Track rigidbody's initial information
        _initialInterp = _rigidbody.interpolation;
        _rotationDifferenceEuler = transform.rotation.eulerAngles - camera.rotation.eulerAngles;

        // hitOffsetLocal = transform.InverseTransformVector(hit.point - hit.transform.position);
        _currentGrabDistance = Vector3.Distance(ray.origin, hit.point);

        // Set rigidbody's interpolation for proper collision detection when being moved by the player
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        // _rigidbody.isKinematic = true;
        _rigidbody.gameObject.layer
    }

    private void Update()
    {
        if(!_active) return;
        
        if(Input.GetMouseButtonUp(0))
        {
            _active = false;
            _rigidbody.interpolation = _initialInterp;
            // _rigidbody.isKinematic = false;
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 holdPoint = ray.GetPoint(_currentGrabDistance);

       _rigidbody.DOMove(holdPoint, Time.deltaTime);
    }
}
