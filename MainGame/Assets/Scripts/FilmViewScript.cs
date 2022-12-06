using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilmViewScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //Old script for world space
        /*
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        transform.position = mousePosition;*/

        //New script for canvas space & keeping child still
        Vector3 mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }
}
