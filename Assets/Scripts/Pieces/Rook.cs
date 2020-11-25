﻿using UnityEngine;
using UnityEngine.UI;

public class Rook : BasePiece
{
    [HideInInspector]
    public Cell mCastleTriggerCell = null;
    private Cell mCastleCell = null;

    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Load sprite
        mMovement = new Vector3Int(7, 7, 0);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Rook");
    }

    public override void Place(Cell newCell)
    {
        base.Place(newCell);

        // Trigger cell
        int triggerOffset = mCurrentCell.mBoardPosition.x < 4 ? 2 : -1;
        mCastleTriggerCell = SetCell(triggerOffset);

        // Castle cell
        int castleOffset = mCurrentCell.mBoardPosition.x < 4 ? 3 : -2;
        mCastleCell = SetCell(castleOffset);
    }

    public void Castle()
    {
        // Set new target cell
        mTargetCell = mCastleCell;

        // Move target cell
        Move();
    }

    private Cell SetCell(int offset)
    {
        // Create new position
        Vector2Int newPosition = mCurrentCell.mBoardPosition;
        newPosition.x += offset;

        // Returning cell
        return mCurrentCell.mBoard.mAllCells[newPosition.x, newPosition.y];
    }
}
