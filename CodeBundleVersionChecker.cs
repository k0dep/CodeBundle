using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace CodeBundle
{
    public class CodeBundleVersionChecker : MonoBehaviour
    {
        public float CheckIntervalInSeconds = 100;
        public int RetryCount;

        public CodeBundleState State;

        public UnityEvent OnRemoteDetectChanges;
        public UnityEvent OnRemoteRequestSingleError;
        public UnityEvent OnRemoteRequestRetrySeriesError;
        public UnityEvent OnError;
        
        private IEnumerator Start()
        {
            if ((State = CodeBundleState.CheckAndLoad(State)) == null)
            {
                OnError.Invoke();
                yield break;
            }
            
            if (Application.isEditor)
            {
                yield break;
            }
        
            var waiter = (object)new WaitForSeconds(CheckIntervalInSeconds);
            var countOfRetries = 0;
            
            while (true)
            {
                var url = new Uri(new Uri(State.RemoteUrl), "/version.json");
                using (var request = UnityWebRequest.Get(url.ToString()))
                {
                    yield return request.SendWebRequest();
                    if (request.isNetworkError || request.isHttpError)
                    {
                        Debug.LogError(
                            $"Asset Bundle version request failed. Code:{request.responseCode}, error: {request.error}");
                        if (countOfRetries >= RetryCount)
                        {
                            Debug.LogError("Too much request retries for version check!");
                            OnRemoteRequestRetrySeriesError.Invoke();
                        }
                        
                        OnRemoteRequestSingleError.Invoke();

                        countOfRetries++;
                        continue;
                    }

                    countOfRetries = 0;

                    var versionState = JsonUtility.FromJson<AssetBundleVersionState>(request.downloadHandler.text);

                    if (!versionState.Equals(State.CurrentRemoteState))
                    {
                        State.CurrentRemoteState = versionState;
                        Debug.Log("Found new version");
                        OnRemoteDetectChanges.Invoke();
                    }
                }

                yield return waiter;
            }
        }
    }
}