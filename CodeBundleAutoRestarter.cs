using UnityEngine;

namespace CodeBundle
{
    /// <summary>
    ///     Сomponent that allows you to restart the application
    /// </summary>
    public class CodeBundleAutoRestarter : MonoBehaviour
    {
        public bool RestartOnStart;

        private void Start()
        {
            if (RestartOnStart)
            {
                Restart();
            }
        }

        /// <summary>
        ///     For restart, just call this method
        /// </summary>
        public void Restart()
        {
            CodeBundleRestarter.Restart();
        }
    }
}