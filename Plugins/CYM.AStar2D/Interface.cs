//------------------------------------------------------------------------------
// Interface.cs
// Copyright 2022 2022/9/25 
// Created by CYM on 2022/9/25
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using UnityEngine;
using CYM;
using CYM.UI;
using CYM.AI;
using System;

namespace CYM.AStar2D
{
    #region AStar 2D
    public enum BasicMoveType2D
    {
        None,
        MoveToNode,
        MoveToUnit,
        MoveIntoUnit,
    }
    public enum AgentState2D
    {
        //空闲
        Idle = 0,
        //跟随路径移动
        FollowingPath,
        //等待移动
        AwaitingFollowing,
    }

    [Flags]
    public enum AgentDirection2D
    {
        Forward = 1,
        Backward = 2,
        Left = 4,
        Right = 8,
        ForwardLeft = Forward | Left,
        ForwardtRight = Forward | Right,
        BackwardLeft = Backward | Left,
        BackwardRight = Backward | Right,
        Default = Forward,
    }
    #endregion
}