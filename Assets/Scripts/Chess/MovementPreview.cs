﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Chess.Pieces;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class MovementPreview : MonoBehaviour
{
    [SerializeField] private Piece movementExecutor;
    [SerializeField] private Piece selectedPiece;

    [SerializeField] private GameObject UI_Panel;
    [SerializeField] private ChessCamera mainCamera;

    [SerializeField] private Material dummyMaterial;

    private bool moving;
    private int accumulatedCost;

    public bool previewing;

    private Cell OnMouseCell;

    public Piece SelectedPiece { get => selectedPiece; set => selectedPiece = value; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ExitPreview();
    }

    private void FixedUpdate()
    {
        if (previewing && !moving) ShowCells();
    }

    private void ShowCells()
    {
        if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0) return;

        var temp = PieceSelector.GetFromRay<Cell>("Cell");

        if (temp && temp != SelectedPiece.boardPosition)
        {
            if (OnMouseCell)
            {
                OnMouseCell.availableCell.color = Color.white;
                OnMouseCell.availableCell.enabled = false;
            }

            OnMouseCell = temp;

            Color colorToChange = movementExecutor.MovePositions.Contains(OnMouseCell) ? Color.green : Color.red;

            OnMouseCell.availableCell.enabled = true;
            OnMouseCell.availableCell.color = colorToChange;
        }
        else if (OnMouseCell)
        {
            OnMouseCell.availableCell.enabled = false;
            OnMouseCell.availableCell.color = Color.white;
        }
    }

    private void StartPreview()
    {
        SelectedPiece.ActionHandler.Actions.Clear();
        SelectedPiece.boardPosition.GetComponent<Renderer>().material.color = Color.blue;

        mainCamera.ChangeState(ChessCamera.State.FOLLOWING);

        movementExecutor.GetPossibleMoves(true);

        previewing = true;
        UI_Panel.SetActive(true);
    }

    private void ExecutePreview()
    {
        moving = false;

        if (movementExecutor.boardPosition.type == Cell.CellType.Void) ExitPreview();
        else movementExecutor.GetPossibleMoves(movementExecutor.omnidirectional);
    }

    public void ExitPreview()
    {
        Destroy(movementExecutor.gameObject);

        SelectedPiece.boardPosition.GetComponent<Renderer>().material.color = Color.white;
        SelectedPiece.ActionHandler.ExecuteActions();
        SelectedPiece.Moved = true;

        mainCamera.ChangeState(ChessCamera.State.LOCKED);

        SelectedPiece = null;
        movementExecutor = null;

        moving = false;
        previewing = false;

        if (OnMouseCell) OnMouseCell.availableCell.enabled = false;

        UI_Panel.SetActive(false);
    }

    public void CancelPreview()
    {
        Destroy(movementExecutor.gameObject);
        movementExecutor = null;

        SelectedPiece.boardPosition.GetComponent<Renderer>().material.color = Color.white;
        SelectedPiece.player.movements += accumulatedCost;
        SelectedPiece = null;

        moving = false;
        previewing = false;
    }

    private void MoveDummy(Cell cell)
    {
        if (!SelectedPiece || moving) return;

        PieceAction actionToDo = null;

        bool lastAction = false;

        if (cell.piecePlaced)
        {
            actionToDo = new MovementAction(cell, 0, Mathf.Infinity, movementExecutor);
            SelectedPiece.ActionHandler.Actions.Add(new PushAction(cell.piecePlaced, 5, SelectedPiece));
            lastAction = true;
        }
        else
        {
            switch (cell.type)
            {
                case Cell.CellType.Normal:
                    actionToDo = new MovementAction(cell, 0, Mathf.Infinity, movementExecutor);
                    SelectedPiece.ActionHandler.Actions.Add(new MovementAction(cell, 0, 5, SelectedPiece));

                    lastAction = true;

                    break;

                case Cell.CellType.Portal:
                    actionToDo = new TeleportAction(cell, 0, Mathf.Infinity, movementExecutor);
                    SelectedPiece.ActionHandler.Actions.Add(new TeleportAction(cell, 0, 5, SelectedPiece));

                    break;

                case Cell.CellType.Jumper:
                    actionToDo = new JumpAction(cell, Mathf.Infinity, movementExecutor);
                    SelectedPiece.ActionHandler.Actions.Add(new JumpAction(cell, 5, SelectedPiece));

                    break;

                case Cell.CellType.DestructibleWall:
                    int cost = movementExecutor.CalculateCost(cell);

                    actionToDo = new DestroyAction(cell, cost, Mathf.Infinity, movementExecutor);
                    SelectedPiece.ActionHandler.Actions.Add(new DestroyAction(cell, cost, 5, SelectedPiece));
                    lastAction = true;


                    break;
            }
        }

        if (actionToDo == null) return;

        StartCoroutine(lastAction ? actionToDo.DoAction(ExitPreview) : actionToDo.DoAction(ExecutePreview));

        moving = true;
    }

    public void SelectPositionToMove()
    {
        if (!movementExecutor) return;

        var selectedCell = PieceSelector.GetFromRay<Cell>("Cell");

        if (selectedCell == null) return;

        if (movementExecutor.MovePositions.Contains(selectedCell))
        {
            accumulatedCost += movementExecutor.CalculateCost(selectedCell);
            movementExecutor.player.movements -= movementExecutor.CalculateCost(selectedCell);

            MoveDummy(selectedCell);
        }
    }

    public void Select(Piece piece)
    {
        if (movementExecutor) return;

        SelectedPiece = piece ? piece : null;
        movementExecutor = piece ? CreateDummy(piece) : null;

        if (movementExecutor && SelectedPiece) StartPreview();
    }

    public Piece CreateDummy(Piece original)
    {
        var clone = Instantiate(original.gameObject).GetComponent<Piece>();

        clone.dummy = true;
        clone.MoveToCell(original.boardPosition);
        clone.GetComponentInChildren<Renderer>().material = dummyMaterial;

        return clone;
    }
}
