//**********************************************
// Class Name	: CYMBase
// Discription	：None
// Author	：CYM
// Team		：MoBaGame
// Date		：2015-11-1
// Copyright ©1995 [CYMCmmon] Powered By [CYM] Version 1.0.0 
//**********************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CYM.Excel;
using CYM.UI;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.ComponentModel;
using UnityEngine.Rendering;

namespace CYM
{
    #region UI
    public interface IUIDirty
    {
        bool IsDirtyShow { get; }
        bool IsDirtyData { get; }
        bool IsDirtyCell { get; }
        bool IsDirtyRefresh { get; }

        void SetDirtyAll(float delay);
        void SetDirtyAll();
        void SetDirtyShow();
        void SetDirtyData();
        void SetDirtyRefresh();
        void SetDirtyCell();

        void RefreshAll();
        void RefreshShow();
        void RefreshCell();
        void RefreshData();
        void Refresh();

        void OnFixedUpdate();
    }
    public interface ICheckBoxContainer
    {
        bool GetIsToggleGroup();
        bool GetIsAutoSelect();
        void SelectItem(UControl arg1);
    }
    #endregion

    #region battle
    public interface IBattleMgr<out TDataOut>
    {
        #region prop
        TDataOut CurData { get; }
        Scene Scene { get; }
        Scene SceneStart { get; }
        Scene SceneSelf { get; }
        string SceneName { get; }
        string BattleID { get; }
        Timer PlayTimer { get; }
        int LoadBattleCount { get; }
        #endregion

        #region set
        void StartNewGame(string battleId = "");
        void StartTutorial(string battleId = "");
        void ContinueGame();
        void LoadGame(string dbKey);
        void GoToStart();
        void LoadBattle(string tdid);
        void LockBattleStartFlow(bool b);
        void UnPauseLoadingView();
        #endregion

        #region is
        bool IsInPauseLoadingView { get; }
        bool IsInBattle { get; }
        bool IsLoadingBattle { get; }
        bool IsGameStartOver { get; }
        bool IsLoadBattleEnd { get; }
        bool IsLockBattleStartFlow { get; }
        bool IsLoadedScene { get; }
        bool IsFirstLoad { get; }
        bool IsInCustomLoading { get; }
        bool IsInReloading { get; }
        #endregion

        #region Callback
        event Callback Callback_OnStartNewGame;
        event Callback Callback_OnCloseLoadingView;
        event Callback Callback_OnBackToStart;
        event Callback Callback_OnLoad;
        event Callback Callback_OnLoaded;
        event Callback Callback_OnLoadedScene;
        event Callback Callback_OnReadDataEnd;
        event Callback Callback_OnUnLoad;
        event Callback Callback_OnUnLoaded;
        event Callback Callback_OnGameStart;
        event Callback Callback_OnGameStarted;
        event Callback Callback_OnGameStartOver;
        event Callback Callback_OnLoadStart;
        event Callback<string, float> Callback_OnLoadingProgress;
        event Callback Callback_OnStartCustomFlow;
        event Callback Callback_OnEndCustomFlow;
        event Callback Callback_OnRandTip;
        event Callback Callback_OnInPauseLoadingView;
        #endregion
    }
    public interface ILevelMgr<out TDataOut>
    {
        #region prop
        TDataOut CurData { get; }
        #endregion

        #region set
        void Load(string battleID = "");
        void UnLoad();
        #endregion

        #region is
        bool IsInLevel { get; }
        string SceneName { get; }
        string LevelID { get; }
        bool IsLoadingLevel { get; }
        bool IsLoadLevelEnd { get; }
        #endregion

        #region Callback
        event Callback Callback_OnLoad;
        event Callback Callback_OnLoaded;
        event Callback Callback_OnLoadedScene;
        event Callback Callback_OnUnLoad;
        event Callback Callback_OnUnLoaded;
        event Callback Callback_OnLoadStart;
        event Callback Callback_OnGameStart;
        event Callback<string, float> Callback_OnLoadingProgress;
        event Callback Callback_OnRandTip;
        #endregion
    }
    #endregion

    #region DB Convert
    public interface IDBListConverMgr<T> where T : DBBase
    {
        void LoadDBData(ref List<T> data);
        void SaveDBData(ref List<T> data);
    }
    public interface IDBConverMgr<T> where T : DBBase
    {
        void LoadDBData(ref T data);
        void SaveDBData(ref T data);
    }
    public interface IDBSingleConverMgr<TData>
        where TData : TDBaseData
    {
        void LoadDBData<TDBData>(ref TDBData dbData, Callback<TData, TDBData> action) where TDBData : DBBase, new();
        void SaveDBData<TDBData>(ref TDBData dbData, Callback<TData, TDBData> action) where TDBData : DBBase, new();
    }
    public interface IDBListConvertMgr<TData>
        where TData : TDBaseData
    {
        void LoadDBData<TDBData>(ref List<TDBData> dbData, Callback<TData, TDBData> action) where TDBData : DBBase, new();
        void SaveDBData<TDBData>(ref List<TDBData> dbData, Callback<TData, TDBData> action) where TDBData : DBBase, new();
    }
    public interface IDBDicConvertMgr<TData>
    where TData : TDBaseData
    {
        void LoadDBData<TDBData>(ref Dictionary<int,TDBData> dbData, Callback<int,TData, TDBData> action) where TDBData : DBBase, new();
        void SaveDBData<TDBData>(ref Dictionary<int,TDBData> dbData, Callback<int,TData, TDBData> action) where TDBData : DBBase, new();
    }
    #endregion

    #region DB
    public interface IDBMono
    {
        void OnRead1(DBBaseGame data);
        void OnRead2(DBBaseGame data);
        void OnRead3(DBBaseGame data);
        void OnReadEnd(DBBaseGame data);

        void OnWrite(DBBaseGame data);
    }
    public interface IArchiveFile
    {
        string Name { get; }
        DateTime SaveTime { get; }
        bool IsBroken { get; }
        ArchiveHeader Header { get; }
        DateTime FileTime { get; }
        // 未损坏且版本为最新
        // 则认为可以读取
        bool IsLoadble { get; }
        // 存档版本是否兼容
        bool IsCompatible { get; }
        TimeSpan PlayTime { get; }
        DBBaseGame BaseGameDatas { get; }
    }
    public interface IArchiveMgr
    {
        List<IArchiveFile> GetAllBaseArchives(bool isRefresh = false);

        // 存档是否可以载入
        bool IsArchiveValid(string id);
        // 是否存在相同的存档
        bool IsHaveArchive(string ID);
        bool IsHaveArchive();
    }
    #endregion

    public interface ITDConfig
    {
        Type DataType { get; }
        void OnLuaParseStart();
        void OnLuaParseEnd();
        void OnExcelParseStart();
        void OnExcelParseEnd();
        void OnAllLoadEnd1();
        void OnAllLoadEnd2();
        Dictionary<string, TDBaseData> BaseDatas { get; }
        T Get<T>(string key) where T : TDBaseData;
        IList GetRawGroup(string group);
        bool Contains(string key);
        List<object> ListObjValues { get; }
        List<string> ListKeys { get; }
        TableMapper TableMapper { get;}
        void AddAlterRangeFromObj(IEnumerable<object> data);
    }
    public interface IUnitSpawnMgr<out TUnitOut>
    {
        IList IListData { get; }
        bool IsAddToGlobalSpawnerMgr { get; }
        Type UnitType { get; }
        TUnitOut GetUnit(long rtid);
        TUnitOut GetUnit(string tdid);
        void Despawn(BaseUnit data,float delay=0);
        bool IsHave(BaseUnit unit);
    }
    public interface ITDSpawnMgr<out TDataOut>
    {
        bool IsAddToGlobalSpawnerMgr { get; }
        Type UnitType { get; }
        TDataOut GetUnit(long rtid);
        TDataOut GetUnit(string tdid);
        void Despawn(TDBaseData data, float delay = 0);
        bool IsHave(TDBaseData unit);
    }
    public interface IPlotMgr
    {
        #region set
        int PushIndex(int? index=null);
        bool Start(string id,int? index=null);
        void RunTemp(IEnumerator<float> enumerator, string flag = null);
        void RunMain();
        void Stop();
        void EnableAI(bool b);
        void SetPlotPause(bool b, int type = 0);
        #endregion

        #region get
        CoroutineHandle CustomStartBattleCoroutine();
        #endregion

        #region is
        bool IsInPlotPause();
        bool IsInPlot();
        bool IsInPlot(params string[] tdid);
        bool IsEnableAI { get;}
        int CurPlotIndex { get; }
        #endregion

        #region ghost
        void AddToGhostSelUnits(params BaseUnit[] unit);
        void AddToGhostMoveUnits(params BaseUnit[] unit);
        void RemoveFromGhostMoveUnits(params BaseUnit[] unit);
        void AddToGhostAIUnits(params BaseUnit[] unit);
        void RemoveFromGhostSelUnits(params BaseUnit[] unit);
        void RemoveFromGhostAIUnits(params BaseUnit[] unit);
        void AddToGhostAnimUnits(params BaseUnit[] unit);
        void RemoveFromGhostAnimUnits(params BaseUnit[] unit);
        bool IsGhostSel(BaseUnit unit);
        bool IsGhostMove(BaseUnit unit);
        bool IsGhostAI(BaseUnit unit);
        bool IsGhostAnim(BaseUnit unit);
        #endregion

        #region blocker ui
        void BlockClick(bool b);
        void AddIgnoreBlockClickOnce(UControl control);
        void RemIgnoreBlockClickOnce(UControl control);
        bool IsIgnoreBlockClickOnce(UControl control);
        void AddIgnoreBlockClick(UControl control);
        void RemIgnoreBlockClick(UControl control);
        bool IsInIgnoreBlockClick(UControl control);
        bool IsInIgnoreBlockClickView(UView view);
        bool IsBlockClick { get; }
        #endregion

        #region blocker unit
        void BlockSelectUnit(bool b);
        bool IsBlockerUnit(BaseUnit unit);
        #endregion
    }
    public interface IDBMgr<out TDataOut>
    {
        #region Callback
        event Callback<bool> Callback_OnSaveState;
        event Callback<bool, DBBaseGame> Callback_OnLoadState;
        #endregion

        #region save and load
        void Load(string ID, bool isAsyn, Callback<bool, DBBaseGame> callback);
        void SaveAs(string ID, bool isSnapshot = false, bool isAsyn = true, bool isDirtyList = true, bool isHide = false);
        void AutoSave(bool isSnapshot = false, bool isForce = false);
        void SaveTemp(bool useSnapshot = false,bool isAsyn = false);
        #endregion

        #region set
        TDataOut CurGameData { get; }
        TDataOut StartNewGame();
        TDataOut Snapshot(bool isSnapshot = true);
        void DeleteArchives(string ID);
        void ReadGameDBData();
        void WriteGameDBData();
        #endregion

        #region get
        IArchiveMgr GetAchieveMgr();
        string GetDefaultSaveName();
        string GetTempSavePath();
        #endregion

        #region is
        // 是否存在当前的存档
        bool IsHaveSameArchives(string ID);
        // 是否有游戏数据
        bool IsHaveGameData();
        // 是否可以继续游戏
        bool IsCanContinueGame();
        // 是否存储游戏中
        bool IsHolding { get; }
        #endregion
    }
    public interface ISettingsMgr<out TDataOut>
    {
        #region get
        TDataOut Settings { get; }
        string[] GetResolutionStrs();
        #endregion

        #region set
        void Revert();
        void Save();
        void SetResolution(int index);
        void SetWindowType(WindowType type);
        void SetQuality(int index);
        void SetTerrainAccuracy(bool b);
        void RefreshScreenSettings();
        #endregion
    }
    public interface IDiffMgr<out TDataOut>
    {
        #region set
        void SetDiffType(GameDiffType type);
        void SetGMMod(bool b);
        void SetAnalytics(bool b);
        void SetHavePlot(bool b);
        #endregion

        #region get
        GameDiffType GetDiffType();
        bool IsGMMod { get;  }
        TDataOut Setting { get; }
        #endregion

        #region is
        bool IsAnalytics();
        bool IsGMMode();
        bool IsSettedGMMod();
        bool IsHavePlot();
        #endregion
    }
    public interface IScreenMgr<out TUnitOut>
    {
        TUnitOut TempPlayer { get; }
        TUnitOut Player { get; }
        TUnitOut PrePlayer { get; }

        string SelectedChara { get; }
        void SelectChara(string tdid);
        void SetCurSelectPlayer();

        string SelectedDrama { get; }
        void SelectDrama(string tdid);

        TUnitOut GetUnit(string id);
        TUnitOut GetUnit(long id);

        event Callback<TUnitOut, TUnitOut> Callback_OnSetPlayer;
    }
    public interface IUnitMgr
    {
        Type UnitType { get; }

        #region set
        BaseUnit Add(string tdid);
        BaseUnit Add(int rtid);
        BaseUnit SpawnNew(string id, [DefaultValue("UnitSpawnParam.Default")] UnitSpawnParam param);
        void Despawn(BaseUnit legion);
        void Occupied(BaseUnit unit);
        Vector3 CalcAveragePos();
        void SortByScore();
        #endregion

        #region is
        bool IsHave();
        bool IsHave(BaseUnit unit);
        bool IsHave(string tdid);
        #endregion
    }
    public interface ITDMgr
    {
        Type DataType { get; }

        #region set
        TDBaseData Spawn(string id);
        void Despawn(TDBaseData data);
        #endregion
    }
    public interface IAttrMgr
    {
        void DoCost<TCostType>(List<Cost<TCostType>> datas, bool isReverse = false) where TCostType : Enum;
        void DoReward(List<BaseReward> rewards, bool isReverse = false);
        float GetVal(int type);
    }
    public interface IBuffMgr
    {
        ITDBuffData AddBase(string buffName);
        void Add(List<string> buffName);
        void Remove(List<string> buffName);
        void Remove(string buffName, RemoveBuffType type = RemoveBuffType.Once);

        string GetTableDesc(List<string> ids, bool newLine = false, string split = Const.STR_Indent, float? anticipationFaction = null, bool appendHeadInfo = false);
        // 拼接所有传入的buff addtion 的字符窜
        string GetTableDesc(string id, bool newLine = false, string split = Const.STR_Indent, float? inputVal = null, bool appendHeadInfo = false);
    }
    public interface IHUDMgr 
    {
        THUD SpawnDurableHUD<THUD>(string prefabName, BaseUnit target = null) where THUD : UHUDBar;
        UHUDText JumpChatBubbleStr(string str);
        UHUDText JumpChatBubble(string key);
    }
    public interface ICameraMgr
    {
        event Callback<Camera> Callback_OnFetchCamera;
        float ScrollVal { get; }
        float GetCustomScrollVal(float maxVal);
        Camera MainCamera { get; }
        Transform MainCameraTrans { get; }
        void FetchCamera();
        void Enable(bool b);
        T GetPostSetting<T>() where T : PostProcessEffectSettings;
    }
    #region other
    public interface IAttrAdditon
    {
        AttrOpType AddType { get; set; }
        AttrFactionType FactionType { get; set; }
        float Val { get; set; }
        float Faction { get; set; }
        float Step { get; set; }
        float InputValStart { get; set; }
        float RealVal { get; }
        float InputVal { get; }
        float Min { get; }
        float Max { get; }
    }
    public interface IUpFactionData
    {
        UpFactionType FactionType { get; set; }
        float AnticipationVal(float? inputVal);
        float Val { get; }
        float RealVal { get; }
        float InputValStart { get; set; }
        float InputVal { get; }
        float Faction { get; set; }
        float Add { get; set; }
        float Percent { get; set; }
        string GetName();
        Sprite GetIcon();
        string GetDesc(bool isHaveSign = false, bool isHaveColor = false, bool isHeveAttrName = true);
    }
    public interface IMono
    {
        void OnEnable();
        void OnSetNeedFlag();
        void Awake();
        void OnAffterAwake();
        void Start();
        void OnAffterStart();
        void OnUpdate();
        void OnFixedUpdate();
        void OnJobUpdate();
        void OnDisable();
        void OnDestroy();
        T AddComponent<T>() where T : BaseMgr, new();
        void RemoveComponent(BaseMgr component);
    }
    public interface IUnit
    {
        bool IsInited { get; }

        #region Life
        // 角色第一次创建，逻辑初始化的时候
        void OnInit();
        // 角色复活后触发
        void OnReBirth();
        // 角色第一次创建或者复活都会触发
        void OnBirth();
        // 角色第一次创建或者复活都会触发
        void OnBirth2();
        // 角色第一次创建或者复活都会触发
        void OnBirth3();
        // 角色假死亡
        void OnDeath();
        // 角色真的死亡
        void OnRealDeath();
        // 溶解
        void OnDissolve();
        void OnGameStart1();
        void OnGameStart2();
        // 游戏开始后触发
        void OnGameStarted1();
        void OnGameStarted2();
        void OnGameStartOver();
        #endregion

        #region Turn
        // 战旗回合
        void OnTurnbase(bool day, bool month, bool year);
        // 帧回合
        void OnTurnframe(int gameFramesPerSecond);
        #endregion

        #region Login
        void OnLoginInit1(object data);
        void OnLoginInit2(object data);
        void OnLoginInit3(object data);
        #endregion
    }
    public interface IRsCacher
    {
        bool IsHave(string name);
        void RemoveNull();
    }
    public interface IBuffGroup
    {
        int Layer { get; }
        int MaxLayer { get; set; }
        ITDBuffData IBuff { get; }
        Sprite GetIcon();
    }
    public interface ITDBaseData:IBase
    {
        #region life
        void OnBeAdded(BaseCoreMono selfMono, params object[] obj);
        void OnBeRemoved();
        #endregion

        #region base get
        string GetTDID();
        string GetName();
        string GetDesc(params object[] ps);
        string GetCont();
        // 获取icon
        Sprite GetIcon();
        // 获得禁用的图标,有可能没有
        Sprite GetDisIcon();
        Sprite GetSelIcon();
        // prefab
        GameObject GetPrefab();
        string GetBuff();
        // 获得animator
        RuntimeAnimatorController GetAnimator();
        //获得SFX
        AudioClip GetSFX();
        #endregion
    }
    public interface ITDBuffData: ITDBaseData
    {
        #region config
        int MaxLayer { get; }
        bool IsHide { get; }
        float MaxTime { get; }
        int IntervalTime { get; }
        ImmuneGainType Immune { get; }
        float InputValStart { get;}
        //buff 合并类型
        BuffMergeType MergeType { get;} 
        //自定义buff组的id,优先级高于BuffMergeType
        string BuffGroupID { get;}//有配置Buff组的会叠加,没有配置Buff组的会合并
        List<string> Performs { get; }
        #endregion

        #region life
        void OnMerge(ITDBuffData newBuff, params object[] obj);
        #endregion

        #region runtime
        int MergeLayer { get; }
        float CurTime { get; }
        float Input { get; }
        float CurInterval { get; }
        bool Valid { get; }
        float PercentCD { get; }
        string PercentCDStr { get; }
        //运行时的MaxTime,RunTime MaxTime,这个值不是配置值,是可以实时变化的
        float RTMaxTime { get; }
        #endregion

        #region is
        // buff时间是否结束
        bool IsTimeOver { get; }
        // 是否永久
        public bool IsForever { get; }
        public bool IsHaveRTMaxTime { get; }
        public bool IsHaveMaxTime { get; }
        #endregion

        #region set
        // 设置属性修改因子
        void SetInput(float input);
        void SetValid(bool b);
        void SetCD(float cd);
        void SetRTMaxTime(float maxTime);
        #endregion

        #region get
        // 获取buff的加成描述列表 翻译
        List<string> GetAdtStrs(float? inputVal = null);
        // 通过Layer 获得加成列表
        List<string> GetAdtStrsByLayer(int layer = 1);
        // 获得buff的加成描述字符串组合
        string GetAdtDesc(bool isNewLine = true, string splite = Const.STR_Indent);
        string GetAdtDescH();
        string GetAdtDescByLayer(int layer = 1, bool isNewLine = false, string splite = Const.STR_Indent);
        string GetTipInfo();
        #endregion
    }
    public interface IClear
    {
        void Clear();
    }
    public interface IOnAnimTrigger
    {
        void OnAnimTrigger(int param);
    }
    #endregion
}
