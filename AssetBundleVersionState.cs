using System;

namespace CodeBundle
{
    [Serializable]
    public class AssetBundleVersionState
    {
        public string hash;

        public bool Equals(AssetBundleVersionState other)
        {
            if (other == null)
            {
                return false;
            }
            
            return string.Equals(hash, other.hash);
        }
    }
}