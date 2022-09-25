//------------------------------------------------------------------------------
// BaseGlobal.cs
// Copyright 2022 2022/9/25 
// Created by CYM on 2022/9/25
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM.AStar2D;
namespace CYM
{
    public partial class BaseGlobal : BaseCoreMono
    {
        Plugin AStar2D = new Plugin
        {
            OnInstall = (g) => {
                AStar2DMgr = g.AddComponent<BaseAStar2DMgr>();
            }
        };
        public static BaseAStar2DMgr AStar2DMgr { get; protected set; }
    }
}