using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class BasePiece : EventTrigger
{
    // Initializing variables
    [HideInInspector]
    public Color mColor = Color.clear;
    public bool mIsFirstMove = true;

    // Original/Current cells
    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;

    // References
    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    // Hover cell
    protected Cell mTargetCell = null;

    // Cell movement
    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHighlightedCells = new List<Cell>();

    // Piece constructor
    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        mPieceManager = newPieceManager;

        mColor = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        mRectTransform = GetComponent<RectTransform>();
    }

    // Placing piece
    public virtual void Place(Cell newCell)
    {
        // Set type
        mCurrentCell = newCell;
        mOriginalCell = newCell;
        mCurrentCell.mCurrentPiece = this;

        // Set position and attribute
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }

    // Resetting cell to default
    public void Reset()
    {
        Kill();

        mIsFirstMove = true;

        Place(mOriginalCell);
    }

    // Removing piece
    public virtual void Kill()
    {
        // Clear current cell
        mCurrentCell.mCurrentPiece = null;

        // Remove piece
        gameObject.SetActive(false);
    }

    // Estimate cell path of  piece
    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {
        // Target position
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        for (int i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;

            // Get current state of target cell
            CellState cellState = CellState.None;
            cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, this);

            // If enemy, add to list
            if (cellState == CellState.Enemy)
            {
                mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
                break;
            }

            if (cellState != CellState.Free)
                break;

            // Add to highlighted list
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
        }
    }

    // Generate possible paths of piece
    protected virtual void CheckPathing()
    {
        // Horizontal
        CreateCellPath(1, 0, mMovement.x);
        CreateCellPath(-1, 0, mMovement.x);

        // Vertical
        CreateCellPath(0, 1, mMovement.y);
        CreateCellPath(0, -1, mMovement.y);

        // Upper diagonal
        CreateCellPath(1, 1, mMovement.z);
        CreateCellPath(-1, 1, mMovement.z);

        // Lower diagonal
        CreateCellPath(-1, -1, mMovement.z);
        CreateCellPath(1, -1, mMovement.z);
    }

    // Highlighting cells
    protected void ShowCells()
    {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = true;
    }

    // De-highlight cells
    protected void ClearCells()
    {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = false;

        mHighlightedCells.Clear();
    }

    // Movement
    protected virtual void Move()
    {
        mIsFirstMove = false;

        // Remove piece if override
        mTargetCell.RemovePiece();

        // Clear current
        mCurrentCell.mCurrentPiece = null;

        // Switch cells
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = this;

        // Move on board
        transform.position = mCurrentCell.transform.position;
        mTargetCell = null;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        // Test for cells
        CheckPathing();

        // Show valid possibilities
        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        // Follow pointer
        transform.position += (Vector3)eventData.delta;

        // Check for overlapping available squares
        foreach (Cell cell in mHighlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
            {
                // Set if mouse is within highlighted cell
                mTargetCell = cell;
                break;
            }

            // Clear if mouse is not in highlighted cell
            mTargetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // Stop cursor and hide
        ClearCells();

        // Return to original position
        if (!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }

        // Move to new cell
        Move();

        // End turn
        mPieceManager.SwitchSides(mColor);
    }
}
