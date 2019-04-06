using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CodeBundle
{
    /// <summary>
    ///     Helper to restart application
    /// </summary>
    public static class CodeBundleRestarter
    {
        public static void Restart()
        {
            Debug.Log("Restarting");

            var allowedPlatforms = new[]
            {
                RuntimePlatform.Android,
                RuntimePlatform.LinuxEditor,
                RuntimePlatform.LinuxPlayer,
                RuntimePlatform.WindowsEditor,
                RuntimePlatform.WindowsPlayer,
                RuntimePlatform.OSXEditor,
                RuntimePlatform.OSXPlayer
            };

            if (!allowedPlatforms.Contains(Application.platform))
            {
                throw new NotSupportedException($"Platform {Application.platform} not support");
            }
            
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                jo.Call("makeRestart");
                return;
            }

            var players = new[]
                {RuntimePlatform.WindowsPlayer, RuntimePlatform.LinuxPlayer, RuntimePlatform.OSXPlayer};
            if (players.Contains(Application.platform))
            {
                var endings = new string[]
                {
                    "exe", "x86", "x86_64", "app"
                };

                var executablePath = Application.dataPath + "/..";
                foreach (string file in System.IO.Directory.GetFiles(executablePath))
                {
                    foreach (string ending in endings)
                    {
                        if (file.ToLower().EndsWith("." + ending))
                        {
                            System.Diagnostics.Process.Start(executablePath + file);
                            Application.Quit();
                            return;
                        }
                    }
                }
                return;
            }
            
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                EditorApplication.isPlaying = false;
            }
#endif
        }
    }
}