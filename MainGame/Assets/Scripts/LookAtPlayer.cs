using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public float speed;
    public float returnSpeed;
    private Transform _player;
    private Vector3 _initRotation;
    private void Start()
    {
        _initRotation = transform.eulerAngles;
        _player = Camera.main.transform;
    }
    private void Update()
    {
        transform.DOLookAt(_player.position, Time.deltaTime * speed);
    }

    private void OnDisable()
    {
        transform.DORotate(_initRotation, returnSpeed);
    }
}
