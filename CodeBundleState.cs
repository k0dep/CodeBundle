using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeBundle
{
    [CreateAssetMenu(menuName = "CodeBundle/Options state", fileName = "CodeBundleOptions")]
    public class CodeBundleState : ScriptableObject
    {
        [Tooltip("Link to the version.json file with information about the current version")]
        public string RemoteUrl;

        public string MainAssetBundleName => $"{Application.companyName}.{Application.productName}";

        public bool DisableWindows;
        public bool DisableLinux;
        public bool DisableOSX;
        public bool DisableAndroid;

        public AssetBundleVersionState CurrentRemoteState { get; set; }

        public IDictionary<string, AssetBundle> AssetBundles;
        public IDictionary<string, AssetBundle> AssetNameToBundle;

        private void OnEnable()
        {
            AssetBundles = new Dictionary<string, AssetBundle>();
            AssetNameToBundle = new Dictionary<string, AssetBundle>();
        }

        /// <summary>
        ///     Load state asset or just check that argument is not null
        /// </summary>
        public static CodeBundleState CheckAndLoad(CodeBundleState state)
        {
            if (state != null)
            {
                return state;
            }
            
            Debug.Log("State asset not set. Try find in resources");
            state = Resources.FindObjectsOfTypeAll<CodeBundleState>().FirstOrDefault();
            if (state == null)
            {
                Debug.LogError("State asset asset not found in resources. " +
                               "Create it from context menu in any Resources folder and configure");
            }
            else
            {
                Debug.Log($"State asset found. Use asset with name `{state.name}`");
            }

            return state;
        }
    }
}