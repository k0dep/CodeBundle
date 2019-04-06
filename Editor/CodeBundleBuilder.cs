using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeBundle
{
    public static class CodeBundleBuilder
    {
        [MenuItem("File/Build CodeBundles")]
        public static void Build()
        {
            var state = CodeBundleState.CheckAndLoad(null);

            if (state == null)
            {
                Debug.LogError("Cannot build CodeBundles without Options asset contains in Resources folder. " +
                          "Create asset from create context menu!");
                return;
            }
            
            var abPath = Path.GetFullPath(Application.dataPath + "/../AssetBundles");
            if (Directory.Exists(abPath))
            {
                Directory.Delete(abPath, true);
            }

            var androidPath = Path.GetFullPath(Application.dataPath +
                                        $"/../AssetBundles/{RuntimePlatform.Android.ToString()}");
            
            var winPath = Path.GetFullPath(Application.dataPath +
                                           $"/../AssetBundles/{RuntimePlatform.WindowsPlayer.ToString()}");
            
            var linuxPath = Path.GetFullPath(Application.dataPath +
                                             $"/../AssetBundles/{RuntimePlatform.LinuxPlayer.ToString()}");

            var macPath = Path.GetFullPath(Application.dataPath +
                                             $"/../AssetBundles/{RuntimePlatform.OSXPlayer.ToString()}");


            if (!state.DisableAndroid)
            {
                Build(androidPath, state.MainAssetBundleName, BuildTarget.Android, RuntimePlatform.Android);
            }

            if (!state.DisableWindows)
            {
                Build(winPath, state.MainAssetBundleName, BuildTarget.StandaloneWindows, RuntimePlatform.WindowsPlayer);
            }

            if (!state.DisableLinux)
            {
                Build(linuxPath, state.MainAssetBundleName, BuildTarget.StandaloneLinux, RuntimePlatform.LinuxPlayer);
            }

            if (!state.DisableOSX)
            {
                Build(macPath, state.MainAssetBundleName, BuildTarget.StandaloneOSX, RuntimePlatform.OSXPlayer);
            }
            
            File.WriteAllText(Application.dataPath + "/../AssetBundles/version.json", JsonUtility.ToJson(
                new AssetBundleVersionState()
                {
                    hash = DateTime.UtcNow.ToString()
                }));
            
            Debug.Log("[CodeBundle building] build finished");
        }
        
        private static void Build(string path, string mainAssetName, BuildTarget target, RuntimePlatform platform)
        {
            Directory.CreateDirectory(path);
            
            Debug.Log($"[CodeBundle building] created path at: {path}");
            
            var res = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, target);
            
            Debug.Log($"[CodeBundle building] build assets end for target: {target.ToString()}");

            var resultPath = Path.Combine(path, mainAssetName);
            File.Move(Path.Combine(path, platform.ToString()), resultPath);
            
            Debug.Log($"[CodeBundle building] main asset moved as: {resultPath}");
        }
    }
}