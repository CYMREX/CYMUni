using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEditor.SceneManagement;
using Sirenix.OdinInspector;
using System.ComponentModel;
namespace CYM
{
    public class BuildWindow : EditorWindow
    {

        public static EditorWindow Ins { get; private set; }

        [MenuItem("Tools/Build  &`")]
        public static void ShowBuildWindow()
        {
            Ins = ShowWindow<BuildWindow>();
            Ins.minSize = new Vector2(300,500);
            RefreshData();
            Save();
        }

        #region prop
        static bool foldKey = false;
        static string curEnterBuildKey = "";
        static GUIStyle TitleStyle = new GUIStyle(); 
        static BuildConfig BuildConfig => BuildConfig.Ins;
        static LocalConfig LocalConfig => LocalConfig.Ins;
        static DLCConfig DLCConfig => DLCConfig.Ins;
        static UIConfig UIConfig => UIConfig.Ins;
        static LogConfig LogConfig => LogConfig.Ins;
        protected static Dictionary<string, string> SceneNames { get; private set; } = new Dictionary<string, string>();
        protected static string VerticalStyle = "HelpBox";
        protected static string ButtonStyle = "minibutton";
        protected static string FoldStyle = "AnimItemBackground";
        protected static string SceneButtonStyle = "ButtonMid;";
        static GUIStyle FoldStyleData;
        #endregion

        #region life
        void OnEnable()
        {
            Ins = this;
            RefreshData();
            AssetDatabase.DisallowAutoRefresh();
        }
        #endregion

        #region set
        public static void RefreshData()
        {
            if (Application.isPlaying)
                return;
            TitleStyle.fixedWidth = 100;
            EnsureProjectFiles();
            RefreshSceneNames();
            BaseGlobal.RefreshInternalConfig();
            BaseGlobal.RefreshDistribution();
            if (Ins != null)
            {
                Ins.titleContent = new GUIContent("Build");
                Ins.Repaint();
            }
            CLog.Info("打开开发者界面");
        }
        public void DrawGUI()
        {
            Present_Info();
            Present_Version();
            Present_Settings();
            Present_DLC();
            Present_Build();
            Present_Explorer();
            Present_SubWindow();
            Present_ConfigWindow();
            Present_LevelList();
            Present_Other();
        }
        #endregion

        #region info
        public void Present_Info()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldInfo = EditorGUILayout.Foldout(LocalConfig.Ins.FoldInfo, "Info", true, FoldStyleData))
            {
                EditorGUILayout.LabelField(string.Format("作者:{0}", "CYM"));
                if (!BuildConfig.LastBuildTime.IsInv())
                    EditorGUILayout.LabelField("BuildTime:" + BuildConfig.LastBuildTime);
                EditorGUILayout.LabelField(string.Format("版本号预览:{0}", BuildConfig));
                EditorGUILayout.LabelField(string.Format("完整版本号预览:{0}", BuildConfig.FullVersion));
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Version
        public void Present_Version()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldVersion = EditorGUILayout.Foldout(LocalConfig.Ins.FoldVersion, "版本", true, FoldStyleData))
            {
                BuildConfig.Platform = (Platform)EditorGUILayout.Popup("目标", (int)BuildConfig.Platform, Enum.GetNames(typeof(Platform)));
                BuildConfig.Distribution = EditorGUILayout.Popup("发布渠道", BuildConfig.Distribution, BaseGlobal.DistributionOptions);
                BuildConfig.BuildType = (BuildType)EditorGUILayout.EnumPopup("打包版本", BuildConfig.BuildType);

                BuildConfig.Name = EditorGUILayout.TextField("名称", BuildConfig.Name);
                if (PlayerSettings.productName != BuildConfig.Name)
                {
                    PlayerSettings.productName = BuildConfig.Name;
                    RefreshAppIdentifier();
                }
                if (PlayerSettings.companyName != BuildConfig.CompanyName)
                {
                    PlayerSettings.companyName = BuildConfig.Name;
                    RefreshAppIdentifier();
                }

                BuildConfig.Major = EditorGUILayout.IntField("主版本", BuildConfig.Major);
                BuildConfig.Minor = EditorGUILayout.IntField("副版本", BuildConfig.Minor);
                BuildConfig.Data = EditorGUILayout.IntField("存档标", BuildConfig.Data);
                BuildConfig.Prefs = EditorGUILayout.IntField("Prefs", BuildConfig.Prefs);

                EditorGUILayout.BeginHorizontal();
                BuildConfig.Tag = (VersionTag)EditorGUILayout.EnumPopup("后缀", BuildConfig.Tag);
                BuildConfig.Suffix = EditorGUILayout.IntField(BuildConfig.Suffix);
                EditorGUILayout.EndHorizontal();


                if (PlayerSettings.bundleVersion != BuildConfig.ToString())
                    PlayerSettings.bundleVersion = BuildConfig.ToString();

                if (PlayerSettings.productName != BuildConfig.Name)
                    PlayerSettings.productName = BuildConfig.Name;

                if (PlayerSettings.companyName != BuildConfig.CompanyName)
                    PlayerSettings.companyName = BuildConfig.CompanyName;

                EditorGUILayout.BeginVertical();
                OnDrawSettings();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            void RefreshAppIdentifier()
            {
                PlayerSettings.applicationIdentifier = "com." + BuildConfig.CompanyName + "." + BuildConfig.Name;
            }
        }
        #endregion

        #region setting
        public void Present_Settings()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.FoldSetting = EditorGUILayout.Foldout(LocalConfig.FoldSetting, "设置", true, FoldStyleData))
            {
                //EditorGUILayout.BeginVertical();
                //if (LocalConfig.FoldSettingBuild = EditorGUILayout.Foldout(LocalConfig.FoldSettingBuild, "Build"))
                //{
                    BuildConfig.TouchDPI = EditorGUILayout.IntField("Touch DPI", BuildConfig.TouchDPI);
                    BuildConfig.DragDPI = EditorGUILayout.IntField("Drag DPI", BuildConfig.DragDPI);
                    UIConfig.Width = EditorGUILayout.IntField("Width",UIConfig.Width);
                    UIConfig.Height = EditorGUILayout.IntField("Height",UIConfig.Height);
                    LogConfig.Enable = EditorGUILayout.Toggle("Is Log", LogConfig.Enable);
                    UIConfig.IsShowLogo = EditorGUILayout.Toggle("Is Show Logo", UIConfig.IsShowLogo);
                    BuildConfig.IgnoreChecker = EditorGUILayout.Toggle("Is Ignore Checker", BuildConfig.IgnoreChecker);
                    bool preDevelopmentBuild = BuildConfig.IsUnityDevelopmentBuild;
                    BuildConfig.IsUnityDevelopmentBuild = EditorGUILayout.Toggle("Is Debug Build", BuildConfig.IsUnityDevelopmentBuild);
                    if (preDevelopmentBuild != BuildConfig.IsUnityDevelopmentBuild)
                    {
                        EditorUserBuildSettings.development = BuildConfig.IsUnityDevelopmentBuild;
                    }
                    BuildConfig.IsShowWinClose = EditorGUILayout.Toggle("Is Show WinClose", BuildConfig.IsShowWinClose);
                    BuildConfig.IsShowConsoleBnt = EditorGUILayout.Toggle("Is Show ConsoleBnt", BuildConfig.IsShowConsoleBnt);

                    BuildConfig.IsSimulationEditor = EditorGUILayout.Toggle("Is Simulation Editor", BuildConfig.IsSimulationEditor);
                    BuildConfig.IsInitLoadDirBundle = EditorGUILayout.Toggle("Is Init Load Dir Bundle", BuildConfig.IsInitLoadDirBundle);
                    BuildConfig.IsInitLoadSharedBundle = EditorGUILayout.Toggle("Is Init Load Shared Bundle", BuildConfig.IsInitLoadSharedBundle);
                    BuildConfig.IsWarmupAllShaders = EditorGUILayout.Toggle("Is Warmup All Shaders", BuildConfig.IsWarmupAllShaders);
                    BuildConfig.IsAssetBundleConfig = EditorGUILayout.Toggle("Is Asset Bundle Config", BuildConfig.IsAssetBundleConfig);
                    BuildConfig.IsDiscreteShared = EditorGUILayout.Toggle("Is Discrete Shared", BuildConfig.IsDiscreteShared);
                    BuildConfig.IsForceBuild = EditorGUILayout.Toggle("Is Force Build", BuildConfig.IsForceBuild);
                    BuildConfig.IsCompresse = EditorGUILayout.Toggle("Is Compresse", BuildConfig.IsCompresse);
                //}

                //if (LocalConfig.FoldSettingSteam = EditorGUILayout.Foldout(LocalConfig.FoldSettingSteam, "Steam", true))
                //{
                //    BuildConfig.SteamAppID = (uint)EditorGUILayout.IntField("Steam App ID", (int)BuildConfig.SteamAppID);
                //    BuildConfig.SteamWebAPI = EditorGUILayout.TextField("Steam Web API", BuildConfig.SteamWebAPI);
                //}

                OnDrawSettings();
                //EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region DLC
        static string newDlcName="NewDlc";
        public void Present_DLC()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.FoldDLC = EditorGUILayout.Foldout(LocalConfig.FoldDLC, "DLC", true, FoldStyleData))
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                newDlcName = EditorGUILayout.TextField("名称", newDlcName);
                if (GUILayout.Button("Add"))
                {
                    if (!newDlcName.IsInv() && EditorUtility.DisplayDialog("警告!", "您确定要添加此DLC吗？", "确定要添加", "取消"))
                    {
                        DLCConfig.AddDLC(newDlcName);
                        Save();
                    }
                }
                EditorGUILayout.EndHorizontal();

                foreach (var item in DLCConfig.EditorExtend)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(item.Name, GUILayout.MaxWidth(100));

                    if (BuildConfig.IsSimulationEditor)
                    {
                        if (GUILayout.Button("Build Config"))
                        {
                            Builder.BuildDLCConfig(item);
                            AssetDatabase.Refresh();
                        }
                        if (GUILayout.Button("Build Bundle"))
                        {
                            Builder.BuildBundle(item);
                            EditorUtility.DisplayDialog("提示!", $"恭喜! {item.Name} 已经打包完成!!", "确定");
                            CLog.Green($"恭喜! {item.Name} 已经打包完成!!");
                        }
                        if (GUILayout.Button("Delete"))
                        {
                            if (EditorUtility.DisplayDialog("警告!", "您确定要删除此DLC吗？", "确定要删除", "取消"))
                            {
                                DLCConfig.RemoveDLC(item.Name);
                                FileUtil.DeleteDir(item.AbsRootPath);
                                Save();
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("请勾选编辑器模式");
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("RrfreshDLC"))
                {
                    DLCConfig.RefreshDLC();
                }
                if (GUILayout.Button("RecreateDLC"))
                {
                    DLCConfig.RecreateDLC();
                    DLCConfig.RefreshDLC();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 构建
        public void Present_Build()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.FoldBuild = EditorGUILayout.Foldout(LocalConfig.FoldBuild, "构建", true, FoldStyleData))
            {
                EditorGUILayout.BeginVertical();
                foreach (var item in DLCConfig.EditorInner)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(item.Name, GUILayout.MaxWidth(100));

                    if (BuildConfig.IsSimulationEditor)
                    {
                        if (GUILayout.Button("Build Config"))
                        {
                            Builder.BuildDLCConfig(item);
                            AssetDatabase.Refresh();
                        }
                        if (GUILayout.Button("Build Bundle"))
                        {
                            if (!CheckKey()) return;
                            Builder.BuildBundle(item);
                            EditorUtility.DisplayDialog("提示!", $"恭喜! {item.Name} 已经打包完成!!", "确定");
                            CLog.Green($"恭喜! {item.Name} 已经打包完成!!");
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("请勾选编辑器模式");
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (BuildConfig.IsSimulationEditor)
                {
                    if (GUILayout.Button("Refresh Config"))
                    {
                        BaseGlobal.RefreshInternalConfig();
                    }
                    if (GUILayout.Button("One click build"))
                    {
                        if (CheckEorr()) return;
                        if (!CheckDevBuildWarring()) return;
                        if (!CheckAuthority()) return;
                        if (!CheckKey()) return;
                        RefreshData();
                        foreach (var item in DLCConfig.EditorAll)
                        {
                            Builder.BuildBundle(item);
                        }
                        Builder.BuildEXE();
                        EditorUtility.DisplayDialog("提示!", $"恭喜! 一键打包已经打包完成!!", "确定");
                        CLog.Green($"恭喜! 一键打包已经打包完成!!");
                        EditorApplication.Beep();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Build"))
                {
                    if (CheckEorr()) return;
                    if (!CheckDevBuildWarring()) return;
                    if (!CheckAuthority()) return;
                    if (!CheckKey()) return;
                    RefreshData();
                    Builder.BuildEXE();
                    EditorUtility.DisplayDialog("提示!", $"恭喜! 程序构建已经打包完成!!", "确定");
                    CLog.Green($"恭喜! 程序构建已经打包完成!!");
                }

                if (GUILayout.Button("Run game"))
                {
                    FileUtil.OpenExplorer(BuildConfig.ExePath);
                    CLog.Info("Run:{0}", BuildConfig.ExePath);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 资源管理器
        public void Present_Explorer()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldExplorer = EditorGUILayout.Foldout(LocalConfig.Ins.FoldExplorer, "链接", true, FoldStyleData))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Persistent"))
                {
                    FileUtil.OpenExplorer(Application.persistentDataPath);
                }
                else if (GUILayout.Button("删除 Persistent"))
                {
                    FileUtil.DeleteDir(Application.persistentDataPath);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Project File"))
                {
                    FileUtil.OpenExplorer(Const.Path_Project);
                }
                else if (GUILayout.Button("Bin"))
                {
                    FileUtil.OpenExplorer(Const.Path_Build);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                OnDrawPresentExplorer();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 关卡列表
        [HideInInspector]
        static Vector2 scrollSceneList = Vector2.zero;
        public void Present_LevelList()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldSceneList = EditorGUILayout.Foldout(LocalConfig.Ins.FoldSceneList, "场景", true, FoldStyleData))
            {
                if (SceneNames.Count > 5)
                    scrollSceneList = EditorGUILayout.BeginScrollView(scrollSceneList, GUILayout.ExpandHeight(false), GUILayout.MinHeight(300));
                //else
                    //scrollSceneList = EditorGUILayout.BeginScrollView(scrollSceneList, GUILayout.ExpandHeight(false), GUILayout.MinHeight(SceneNames.Count * 15));

                EditorGUILayout.BeginHorizontal();
                DrawGoToBundleSystemSceneButton(Const.SCE_Start);
                DrawGoToBundleSystemSceneButton(Const.SCE_Preview);
                DrawGoToBundleSystemSceneButton(Const.SCE_Test);
                EditorGUILayout.EndHorizontal();
                if (SceneNames != null)
                {
                    foreach (var item in SceneNames)
                    {
                        if (item.Key == Const.SCE_Preview ||
                            item.Key == Const.SCE_Start ||
                            item.Key == Const.SCE_Test)
                            continue;
                        DrawGoToBundleSceneButton(item.Key, item.Value);
                    }
                }
                if (SceneNames.Count > 5)
                    EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 其他
        public void Present_Other()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldOther = EditorGUILayout.Foldout(LocalConfig.Ins.FoldOther, "其他", true, FoldStyleData))
            {
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("保存"))
                {
                    Save();
                }
                else if (GUILayout.Button("拍照"))
                {
                    ScreenCapture.CaptureScreenshot(Const.Path_Screenshot + "/Screenshot.png", ScreenCapture.StereoScreenCaptureMode.BothEyes);
                    FileUtil.OpenFile(Const.Path_Screenshot);
                }
                else if (GUILayout.Button("编译"))
                {
                    AssetDatabase.Refresh();
                }
                else if (GUILayout.Button("刷新"))
                {
                    RefreshData();
                }
                else if (GUILayout.Button("运行"))
                {
                    AssetDatabase.Refresh();
                    GoToScene(GetSystemScenesPath(Const.SCE_Start), OpenSceneMode.Single);
                    EditorApplication.isPlaying = true;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                OnDrawPresentScriptTemplate();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region sub Window
        public void Present_SubWindow()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldSubWindow = EditorGUILayout.Foldout(LocalConfig.Ins.FoldSubWindow, "窗口", true, FoldStyleData))
            {
                if (GUILayout.Button("GUIStyle")) GUIStyleWindow.ShowWindow();
                else if (GUILayout.Button("ColorPicker")) ColorPickerWindow.ShowWindow();
                else if (GUILayout.Button("Dependencies")) DependencyWindow.ShowWindow();
                else if (GUILayout.Button("ParticleScaler")) ParticleScalerWindow.ShowWindow();
                else if (GUILayout.Button("UnityTexture")) UnityTextureWindow.ShowWindow();
                else if (GUILayout.Button("RampTexture")) RampTexGenWindow.ShowWindow();
                else if (GUILayout.Button("Screenshot")) ScreenshotWindow.ShowWindow();
                else if (GUILayout.Button("TerrainHeight")) TerrainHeightWindow.ShowWindow();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region config windown
        public void Present_ConfigWindow()
        {
            EditorGUILayout.BeginVertical(VerticalStyle);
            if (LocalConfig.Ins.FoldConfigWindow = EditorGUILayout.Foldout(LocalConfig.Ins.FoldConfigWindow, "配置", true, FoldStyleData))
            {
                foreach (var item in BaseGlobal.ConfigWindows)
                {
                    if (item.Key is IScriptableObjectConfig inter && !inter.IsHideInBuildWindow)
                    {
                        if (GUILayout.Button(item.Key.name))
                            ScriptableObjectConfigWindow.ShowConfigWindow(item.Key);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region utile
        public static void Save()
        {
            EditorUtility.SetDirty(LogConfig);
            EditorUtility.SetDirty(UIConfig);
            EditorUtility.SetDirty(DLCConfig);
            EditorUtility.SetDirty(LocalConfig);
            EditorUtility.SetDirty(BuildConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public bool CheckDevBuildWarring()
        {
            if (BuildConfig.IsDevelop)
            {
                return EditorUtility.DisplayDialog("警告!", "您确定要构建吗?因为当前是Dev版本", "确定要构建Dev版本", "取消");
            }
            return true;
        }
        protected bool CheckAuthority()
        {
            CLog.Info("打包:"+SystemInfo.deviceName);
            return true;
        }
        protected bool CheckEorr()
        {
            if (BuildConfig.IgnoreChecker)
                return false;

            if (CheckIsHaveError())
                return true;
            return false;
        }
        protected bool CheckKey()
        {
            return true;
        }
        protected bool DoCheckWindow<T>() where T : CheckerWindow
        {
            T window = GetWindow<T>();
            window.CheckAll();
            window.Close();
            return window.IsHaveError();
        }
        protected static EditorWindow ShowWindow<T>() where T : EditorWindow
        {
            var ret = GetWindow<T>();
            ret.ShowPopup();
            return ret;
        }
        protected string GetScenesPath(string name)
        {
            return string.Format(Const.Format_BundleScenesPath, name);
        }
        protected string GetSystemScenesPath(string name)
        {
            return string.Format(Const.Format_BundleSystemScenesPath, name);
        }
        protected void DrawGoToBundleSystemSceneButton(string name)
        {
            if (GUILayout.Button(name))
            {
                GoToScene(GetSystemScenesPath(name));
            }
        }
        protected void DrawGoToBundleSceneButton(string name, string fullPath)
        {
            if (GUILayout.Button(name))
            {
                GoToSceneByFullPath(fullPath);
            }
        }
        protected void DrawButton(string name, Callback doAction)
        {
            if (GUILayout.Button(name))
            {
                doAction?.Invoke();
            }
        }
        protected void GoToScene(string path, OpenSceneMode mode = OpenSceneMode.Single)
        {
            GoToSceneByFullPath(Application.dataPath + path, mode);
        }
        protected void GoToSceneByFullPath(string path, OpenSceneMode mode = OpenSceneMode.Single)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path, mode);
        }
        protected void LookAtPos(Vector3 pos)
        {
            SceneView view = SceneView.lastActiveSceneView;
            view.LookAt(pos, view.rotation, 20);
        }
        protected void SafeOpenJsonFile<T>(string path, T data) where T : class
        {
            FileUtil.UpdateFile(path, data);
            FileUtil.OpenFile(path);
        }
        protected static void RefreshSceneNames()
        {
            var paths = Directory.GetFiles(Path.Combine(Application.dataPath,Const.Dir_Art, "Scene"),"*.unity", SearchOption.AllDirectories);
            SceneNames.Clear();
            foreach (var item in paths)
            {
                string tempName = Path.GetFileNameWithoutExtension(item);
                if (tempName == Const.SCE_Preview ||
                    tempName == Const.SCE_Start ||
                    tempName == Const.SCE_Test)
                    continue;
                if (!SceneNames.ContainsKey(tempName))
                    SceneNames.Add(tempName, item);
            }
        }
        /// <summary>
        /// 确保标准项目文件夹存在
        /// </summary>
        public static void EnsureProjectFiles()
        {
            //if (BuildConfig.ShowType == BuildWindowShowType.Dlc)
            //{
            //    FileUtil.EnsureDirectory(Const.Path_Arts);
            //    FileUtil.EnsureDirectory(Const.Path_Resources);
            //    FileUtil.EnsureDirectory(Const.Path_ResourcesConfig);
            //    FileUtil.EnsureDirectory(Const.Path_ResourcesConst);
            //    FileUtil.EnsureDirectory(Const.Path_StreamingAssets);
            //}
            //else if (BuildConfig.ShowType == BuildWindowShowType.Build)
            //{
            //}

            FileUtil.EnsureDirectory(Const.Path_Arts);
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Arts, "Scene"));

            FileUtil.EnsureDirectory(Const.Path_Bundles);

            FileUtil.EnsureDirectory(Const.Path_Resources);
            FileUtil.EnsureDirectory(Const.Path_ResourcesConfig);
            FileUtil.EnsureDirectory(Const.Path_ResourcesTemp);
            FileUtil.EnsureDirectory(Const.Path_ResourcesConst);

            FileUtil.EnsureDirectory(Const.Path_Funcs);
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "GlobalMgr"));
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "Main"));
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "Table"));
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "UI"));
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "UIMgr"));
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "UnitMgr"));
            FileUtil.EnsureDirectory(Path.Combine(Const.Path_Funcs, "UnitMono"));
            FileUtil.EnsureDirectory(Const.Path_StreamingAssets);
        }
        #endregion

        #region Override
        [HideInInspector]
        public Vector2 scrollPosition = Vector2.zero;
        protected void OnGUI()
        {
            if (BuildConfig == null)
                return;
            if (FoldStyleData==null)
            {
                FoldStyleData = new GUIStyle("FoldoutHeader");
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawGUI();
            EditorGUILayout.EndScrollView();
        }
        protected virtual void OnDrawPresentScriptTemplate()
        {
        }
        protected virtual void OnDrawPresentExplorer()
        {
        }
        protected virtual void OnDrawSubwindow()
        {

        }
        protected virtual void OnDrawSettings()
        {

        }
        protected virtual bool CheckIsHaveError()
        {
            //e.x.
            return DoCheckWindow<CheckerWindow>();
        }
        #endregion
    }
}