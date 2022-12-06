using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JigsawPuzzle : MonoBehaviour
{
    public GameObject clueItem;
    public GameObject solvedPictureInverse;
    private Vector3 solvedPicturePos;
    private GameObject[] puzzlePieces;
    private bool puzzleSolved;
    // Start is called before the first frame update
    void Start()
    {
        puzzlePieces = GameObject.FindGameObjectsWithTag("PuzzlePiece");
        //Get initial pos of inverse image so that it doesn't move w/ mask
        solvedPicturePos = solvedPictureInverse.GetComponent<RectTransform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (puzzleSolved) {
            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                puzzlePieces[i].GetComponent<PuzzlePiece>().enabled = false;
            }
            
        }
        solvedPictureInverse.GetComponent<RectTransform>().position = solvedPicturePos;
    }

    public void ResetPuzzle()
    {
        if (!puzzleSolved) {
            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                puzzlePieces[i].GetComponent<PuzzlePiece>().Randomize();
                puzzlePieces[i].GetComponent<PuzzlePiece>().InRightPosition=false;
            }
        }
    }

    public void CheckSolved() {
        bool returnVal = true;
        for (int i = 0; i < puzzlePieces.Length; i++) {
            if (!puzzlePieces[i].GetComponent<PuzzlePiece>().InRightPosition) {
            returnVal = false;
            }
        }
        if (returnVal) {clueItem.SetActive(true); }
        
        puzzleSolved = returnVal;
    }
}
