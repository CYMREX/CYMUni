using System;
using System.IO;
using UnityEngine;
//------------------------------------------------------------------------------
// BuildConfig.cs
// Created by CYM on 2018/9/5
// 填写类的描述...
//------------------------------------------------------------------------------
namespace CYM
{
    public sealed class BuildConfig : ScriptableObjectConfig<BuildConfig> 
    {
        public override bool IsHideInBuildWindow => true;
        #region Inspector
        [HideInInspector]
        public string CompanyName = "CYM";
        [HideInInspector]
        public string NameSpace = "NationWar";
        [HideInInspector]
        public string MainUIView = "MainUIView";
        public int Major;
        public int Minor;
        public int Data;
        public int Suffix = 1;
        public int Prefs = 0;
        public VersionTag Tag = VersionTag.Preview;
        public bool IsUnityDevelopmentBuild;
        public bool IsShowWinClose = true;
        public bool IsObfuse = true;
        public bool IsShowConsoleBnt = false;
        public bool IgnoreChecker;
        public BuildType BuildType = BuildType.Develop;
        public string LastBuildTime;
        public Platform Platform = Platform.Windows64;
        public string Name = "MainTitle";
        public int Distribution;
        public int TouchDPI = 1;
        public int DragDPI = 800;

        //将所有的配置资源打成AssetBundle来读取，适合移动平台
        [SerializeField]
        public bool IsAssetBundleConfig = true;
        [SerializeField]
        public bool IsDiscreteShared = true; //是否为离散的共享包
        [SerializeField]
        public bool IsForceBuild = false;
        [SerializeField]
        public bool IsCompresse = true; //是否为压缩格式
        [SerializeField]
        public bool IsSimulationEditor = true;
        //是否初始化的时候加载所有的Directroy Bundle
        [SerializeField]
        public bool IsInitLoadDirBundle = true;
        //是否初始化的时候加载所有Shared Bundle
        [SerializeField]
        public bool IsInitLoadSharedBundle = true;
        [SerializeField]
        public bool IsWarmupAllShaders = true;
        #endregion

        public static bool IsForceEditorMode { get; set; } = false;

        #region prop
        public string GameVersion => ToString();
        public string FullVersion => string.Format("{0} {1} {2}", FullName, ToString(), Platform);
        public string DirPath
        {
            get
            {
                if (IsPublic)
                {
                    if (IsTrial)
                    {
                        return Path.Combine(Const.Path_Build, Platform.ToString());//xxxx/Windows_x64 Trail
                    }
                    else
                        return Path.Combine(Const.Path_Build, Platform.ToString());//xxxx/Windows_x64
                }
                else
                {
                    return Path.Combine(Const.Path_Build, FullVersion);//xxxx/BloodyMary v0.0 Preview1 Windows_x64 Steam
                }
            }
        }
        public string ExePath
        {
            get
            {
                if(Platform == Platform.Windows64)
                    return Path.Combine(DirPath, FullName + ".exe");
                else if(Platform == Platform.Android)
                    return Path.Combine(DirPath, FullName + ".apk");
                else if(Platform == Platform.IOS)
                    return Path.Combine(DirPath, FullName + ".ipa");
                throw new Exception();
            }
        }
        public string FullName => Name;
        public override string ToString()
        {
            string str = string.Format("v{0}.{1} {2}{3} {4}", Major, Minor, Tag, Suffix,BaseGlobal.GetDistributionName());
            if (IsDevelop)
                str += " Dev";
            return str;
        }
        #endregion

        #region is
        public bool IsDevelop => BuildType == BuildType.Develop;
        public bool IsPublic => BuildType == BuildType.Public;
        public bool IsTrial => BaseGlobal.GetDistributionName() == nameof(BaseGlobal.Trial);
        public static bool IsWindows => 
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor;
        // 数据库版本是否兼容
        public bool IsInData(int data) => Data == data;
        //是否为编辑器模式
        public bool IsEditorMode
        {
            get
            {
                if (IsForceEditorMode) return true;
                if (!Application.isEditor) return false;
                if (Application.isEditor && IsSimulationEditor) return true;
                return false;
            }
        }
        //编辑器模式或者纯配置模式
        public bool IsEditorOrConfigMode => IsEditorMode || !IsAssetBundleConfig;
        //编辑器模式或者AB配置模式
        public bool IsEditorOrAssetBundleMode => IsEditorMode || IsAssetBundleConfig;
        #endregion
    }
}