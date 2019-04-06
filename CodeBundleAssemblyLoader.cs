using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBundle
{
    /// <summary>
    ///     Load assembly from dll placed in asset bundle or just in Resource folder
    /// </summary>
    public class CodeBundleAssemblyLoader : MonoBehaviour
    {
        public string DllName;
        public string DllNameForAssetBundle;

        public UnityEvent OnError;
        public UnityEvent OnSuccess;

        public CodeBundleState State;
        
        public bool ForceUseAssetBundles;

        
        public void Load()
        {
            if ((State = CodeBundleState.CheckAndLoad(State)) == null)
            {
                OnError.Invoke();
                return;
            }
            
            if (!Application.isEditor || ForceUseAssetBundles)
            {
                LoadFromAssetBundle();
            }
            else
            {
                LoadFromResources();
            }
        }

        private void LoadFromResources()
        {
            Debug.Log("Start loading dll from resource");
            
            var dllText = Resources.Load<TextAsset>(DllName);
            
            if (dllText == null)
            {
                Debug.LogError($"Dll text asset not found in resources, name is: {DllName}");
                OnError.Invoke();
                return;
            }

            try
            {
                AppDomain.CurrentDomain.Load(dllText.bytes);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when loading dll from assembly in AppDomain");
                Debug.LogException(e);
                OnError.Invoke();
                return;
            }
            
            Debug.Log("Loading dll from resource finish succes");
            
            OnSuccess.Invoke();

        }

        private void LoadFromAssetBundle()
        {
            Debug.Log("Start loading dll from asset bunle");
            
            var dllText = State.AssetNameToBundle[DllNameForAssetBundle].LoadAsset<TextAsset>(DllNameForAssetBundle);
            
            if (dllText == null)
            {
                Debug.LogError($"Dll text asset not found in assetbundle, name is: {DllNameForAssetBundle}");
                OnError.Invoke();
                return;
            }

            try
            {
                AppDomain.CurrentDomain.Load(dllText.bytes);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when loading dll from assembly in AppDomain");
                Debug.LogException(e);
                OnError.Invoke();
                return;
            }
            
            Debug.Log("Loading dll from asset bunle finish succes");
            
            OnSuccess.Invoke();
        }
    }
}