﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Chess.PieceActions;

namespace Assets.Scripts.Chess.Pieces
{
    public interface IActionHandler
    {
        List<IPieceAction> Actions { get; set; }
        Action Callback { get; set; }

        void AddAction(IPieceAction action);
        void ExecuteActions();
    }
}