using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Board mBoard;
    public PieceManager mPieceManager;

    void Start()
    {
        // Creating board
        mBoard.Create();

        // Creating pieces
        mPieceManager.Setup(mBoard);
    }
}
