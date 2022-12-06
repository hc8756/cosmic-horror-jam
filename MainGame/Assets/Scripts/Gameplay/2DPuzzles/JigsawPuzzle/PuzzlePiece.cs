using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour
{
    private Vector3 RightPosition;
    public bool InRightPosition;
    [SerializeField] private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        RightPosition = transform.position;
        InRightPosition = false;
        Randomize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, RightPosition) < 3f)
        {
            if (InRightPosition == false)
            {
                transform.position = RightPosition;
                InRightPosition = true; 
                canvas.GetComponent<JigsawPuzzle>().CheckSolved();
            }
        }
    }

    public void Randomize()
    {
        int LeftOrRight = Random.Range(1, 3);
        float newX;
        if (LeftOrRight == 1) { newX = Random.Range(-350, -200); }
        else { newX = Random.Range(200, 350); }
        float newY = Random.Range(-150, 110);
        transform.position = canvas.transform.TransformPoint(new Vector3(newX, newY));
    }

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position
            );
        if (!InRightPosition) {
            transform.position = canvas.transform.TransformPoint(position);
        }
    }
}
