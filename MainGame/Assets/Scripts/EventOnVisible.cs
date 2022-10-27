using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOnVisible : MonoBehaviour
{
    public GameEvent onVisible;
    public GameEvent onHidden;
    public MeshRenderer meshRenderer;
    private bool _visible;
    private float _visTimer;

    public void Update()
    {
        if(meshRenderer.isVisible && !_visible && _visTimer >= 0.25f)
        {
            if(onVisible)
                onVisible.Raise();
            _visTimer = 0f;
        }

        if(!meshRenderer.isVisible && _visible && _visTimer >= 0.25f)
        {
            if(onHidden)
                onHidden.Raise();
            _visTimer = 0f;
        }

        _visible = meshRenderer.isVisible;

        _visTimer += Time.deltaTime;
    }
}
