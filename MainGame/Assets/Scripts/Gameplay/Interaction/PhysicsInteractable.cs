using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.FPS.Gameplay;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsInteractable : Interactable
{
    public float releaseDelay = 1f;
    private Rigidbody _rigidbody;
    private RigidbodyInterpolation _initialInterp;
    // Unused but useful for rotation
    private Vector3 _rotationDifferenceEuler;
    private bool _active;
    private float _currentGrabDistance;
    private FixedJoint _fixedJoint;
    
    private Rigidbody _playerCameraRB;
    private void Start()
    {
        PlayerCharacterController _player = UnityHelper.FindObjectOfTypeOrThrow<PlayerCharacterController>();
        _playerCameraRB = _player.WeaponCamera.gameObject.GetComponentOrThrow<Rigidbody>();

        _rigidbody = gameObject.GetComponentOrThrow<Rigidbody>();
    }
    
    public override void OnPlayerHover(Ray ray, RaycastHit hit)
    {
        if(_active) return;

        // No support for throwing yet but just put this somewhere in response to an input action and it will work
        // rigidbody.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

        if(!Input.GetMouseButtonDown(0)) return;
        _active = true;

        _fixedJoint = gameObject.AddComponent<FixedJoint>();
        _fixedJoint.connectedBody = _playerCameraRB;

        Transform camera = Camera.main.transform;

        // Set rigidbody's interpolation for proper collision detection when being moved by the player
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        // Very important so the player can't glitch and use this to fly
        _rigidbody.gameObject.layer = LayerMask.NameToLayer("DoesNotCollideWithPlayer");
    }

    private void Update()
    {
        if(!_active) return;
        
        if(Input.GetMouseButtonUp(0))
        {
            _active = false;
            _rigidbody.interpolation = _initialInterp;
            Destroy(_fixedJoint);
            _rigidbody.gameObject.layer = 0;

            return;
        }
    }
}
