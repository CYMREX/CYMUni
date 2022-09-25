//------------------------------------------------------------------------------
// BuildSteamData.cs
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
using System.IO;

namespace CYM.Steam
{
    #region Steam Data
    [Serializable]
    public class BuildSteamData : BuildData
    {
        public override void PostBuilder()
        {
            base.PostBuilder();
            FileUtil.WriteFile(Path.Combine(BuildConfig.DirPath, Const.File_SteamAppID), GameConfig.Ins.SteamAppID.ToString());
        }
    }
    #endregion
}