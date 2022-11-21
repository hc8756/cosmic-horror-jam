using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawPuzzle : MonoBehaviour
{
    private GameObject[] puzzlePieces;
    private bool puzzleSolved;
    // Start is called before the first frame update
    void Start()
    {
        puzzlePieces = GameObject.FindGameObjectsWithTag("PuzzlePiece");
        puzzleSolved = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (puzzleSolved) { gameObject.GetComponent<Canvas>().enabled = false; }
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
        puzzleSolved = returnVal;
    }
}
