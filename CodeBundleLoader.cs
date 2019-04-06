using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace CodeBundle
{
    /// <summary>
    ///     Loads assets from bundles and causes events upon success or failure.
    /// </summary>
    public class CodeBundleLoader : MonoBehaviour
    {
        public bool ForceUseAssetBundle;
        public bool DebugMode;

        public UnityEvent OnLoaded;
        public UnityEvent OnError;

        public CodeBundleState State;

        
        public void StartLoading()
        {
            StartCoroutine(Loading());
        }

        private IEnumerator Loading()
        {
            if ((State = CodeBundleState.CheckAndLoad(State)) == null)
            {
                OnError.Invoke();
                yield break;
            }
            
            var platformPrefix = Application.platform.ToString();

            if (!Application.isEditor || ForceUseAssetBundle)
            {
                var assetBundleRequests = new Queue<string>();
                assetBundleRequests.Enqueue(State.MainAssetBundleName);

                while (assetBundleRequests.Any())
                {
                    var bundleName = assetBundleRequests.Dequeue();
                    Debug.Log("Loading asset bundle " + bundleName);
                    using (var abRequest =
                        UnityWebRequestAssetBundle.GetAssetBundle($"{State.RemoteUrl}{platformPrefix}/{bundleName}"))
                    {
                        yield return abRequest.SendWebRequest();

                        if (abRequest.isNetworkError || abRequest.isHttpError)
                        {
                            Debug.LogError(
                                $"Asset Bundle load request failed. Code:{abRequest.responseCode}, error: {abRequest.error}");
                            OnError.Invoke();
                            yield break;
                        }

                        var assetBundle = DownloadHandlerAssetBundle.GetContent(abRequest);
                        State.AssetBundles[bundleName] = assetBundle;

                        if (assetBundle.isStreamedSceneAssetBundle)
                        {
                            continue;
                        }

                        var manifest = assetBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");

                        if (manifest == null)
                        {
                            continue;
                        }

                        foreach (var depenedetAsset in manifest.GetAllAssetBundles())
                        {
                            assetBundleRequests.Enqueue(depenedetAsset);
                        }
                    }
                }

                foreach (var assetBundle in State.AssetBundles.Values)
                {
                    foreach (var assetName in assetBundle.GetAllAssetNames())
                    {
                        if (DebugMode)
                        {
                            Debug.Log($"asset `{assetName}` place in {assetBundle.name}");
                        }

                        State.AssetBundles[assetName] = assetBundle;
                    }
                }
            }

            Debug.Log("Loading finish");

            OnLoaded.Invoke();

            yield return null;
        }
    }
}