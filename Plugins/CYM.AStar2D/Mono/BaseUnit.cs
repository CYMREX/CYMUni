//------------------------------------------------------------------------------
// BaseUnit.cs
// Copyright 2022 2022/9/25 
// Created by CYM on 2022/9/25
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM.AStar2D;

namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        public BaseAStarMove2DMgr Move2DMgr { get; protected set; }
    }
}