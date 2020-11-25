using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    // Initializing variables
    public Image mOutlineImage;

    // Cell Position
    [HideInInspector]
    public Vector2Int mBoardPosition = Vector2Int.zero;

    // Board
    [HideInInspector]
    public Board mBoard = null;

    // Cell Size
    [HideInInspector]
    public RectTransform mRectTransform = null;

    [HideInInspector]
    public BasePiece mCurrentPiece = null;

    //[HideInInspector]
    //public BasePiece mCurrentPiece = null;

    // Cell constructor
    public void Setup(Vector2Int newBoardPosition, Board newBoard)
    {
        // Set board
        mBoardPosition = newBoardPosition;
        mBoard = newBoard;

        // Set position
        mRectTransform = GetComponent<RectTransform>();
    }

    // Remove if no piece
    public void RemovePiece()
    {
        if (mCurrentPiece != null)
        {
            mCurrentPiece.Kill();
        }
    }
}
