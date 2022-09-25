//------------------------------------------------------------------------------
// DLCDownloader.cs
// Created by CYM on 2022/9/15
// 填写类的描述...
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
namespace CYM.DLC
{
    public class DLCDownloader : BaseCoreMono
    {
        #region prop
        static Dictionary<string, string> ServerAllBundles = new Dictionary<string, string>();
        static Dictionary<string, string> LocalAllBundles = new Dictionary<string, string>();
        const string AssetBundles = nameof(AssetBundles);
        const string HashDataFile = "hashdata.txt";
        static string LocalCachePath;
        static string FullNetPath;
        #endregion

        #region inspector
        static bool IsHotFix = false;
        static string LocalIP = "http://127.0.0.1:8001";
        static string PublicIP = "http://123.56.90.148:8001";
        static string AssetBundlePath = "StreamingAssets/";
        #endregion

        #region life
        public override void OnAffterAwake()
        {
            base.OnAffterAwake();
            LocalCachePath = Path.Combine(Application.persistentDataPath, AssetBundles);
        }
        #endregion

        #region get path
        string GetCachePath(string name)=> LocalCachePath + "/" + name;
        static string GetNetBundlePath(string filename) => FullNetPath + filename;
        #endregion

        #region get
        public AssetBundle GetAssetBundle(string name)
        {
            AssetBundle ab = AssetBundle.LoadFromFile(GetCachePath(name));
            return ab;
        }
        public async Task<AssetBundle> GetAssetBundleAsnyc(string name)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(GetCachePath(name));
            while (!request.isDone)
                await Task.Yield();
            return request.assetBundle;
        }
        #endregion

        #region set
        static AssetBundleManifest Manifest = null;

        public static IEnumerator Download(DLCItemConfig config)
        {
            if (IsHotFix)
            {
                string dlcName = config.Name;
                ServerAllBundles.Clear();
                LocalAllBundles.Clear();
                string ip = LocalIP;
                if (BuildConfig.Ins.IsPublic)
                {
                    ip = PublicIP;
                }
                FullNetPath = ip + "/" + AssetBundlePath;
                yield return EM_Download(dlcName, true);
                yield return EM_DownloadAllAssetBundle();
            }
        }
        static IEnumerator EM_DownloadAllAssetBundle()
        {
            //读取本地hash文件
            var hashdatapath = LocalCachePath + "/" + HashDataFile;
            if (File.Exists(hashdatapath))
            {
                LocalAllBundles.Clear();
                var deserdata = File.ReadAllText(hashdatapath);
                LocalAllBundles = JsonConvert.DeserializeObject<Dictionary<string, string>>(deserdata);
            }
            //获取所有的AssetBundle
            foreach (var item in Manifest.GetAllAssetBundles())
            {
                ServerAllBundles.Add(item, Manifest.GetAssetBundleHash(item).ToString());
            }
            //下载所有资源文件
            foreach (var item in ServerAllBundles.Values)
            {
                yield return EM_Download(item);
            }
            Debug.Log("远端资源下载完成");

            //写入最新的清单文件
            var serdata = JsonConvert.SerializeObject(ServerAllBundles);
            File.WriteAllText(hashdatapath, serdata);
            yield break;
        }
        static IEnumerator EM_Download(string item, bool isManifest = false)
        {
            UnityWebRequest request;
            string uri = GetNetBundlePath(item);
            string savepath = LocalCachePath + "/" + item;
            if (!isManifest)
            {
                //已经存在就不再下载
                if (File.Exists(savepath) && LocalAllBundles != null)
                {
                    if (ServerAllBundles.ContainsKey(item) &&
                        LocalAllBundles.ContainsKey(item) &&
                        LocalAllBundles[item] == ServerAllBundles[item])
                        yield break;
                }
                request = UnityWebRequest.Get(uri);
            }
            else
            {
                request = UnityWebRequest.Get(uri);
            }
            yield return request.SendWebRequest();
            if (!request.isNetworkError && !request.isHttpError)
            {
                if (Application.isEditor)
                    Debug.Log("下载成功:" + item);

                if (!Directory.Exists(LocalCachePath))
                {
                    Directory.CreateDirectory(LocalCachePath);
                }
                try
                {
                    File.WriteAllBytes(savepath, request.downloadHandler.data);
                    Debug.LogFormat("文件写入成功:{0}", savepath);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogErrorFormat("文件写入失败:{0}", savepath);
                }

                if (isManifest)
                {
                    var bundle = AssetBundle.LoadFromFile(item);
                    Manifest = bundle.LoadAsset<AssetBundleManifest>(Const.STR_ABManifest);
                }
            }
            else
            {
                Debug.LogError(request.error);
            }
        }
        #endregion
    }
}