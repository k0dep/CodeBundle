using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBundle
{
    public class CodeBundleRuntimeObject : MonoBehaviour
    {
        public string TypeName;

        public UnityEvent OnError;

        public object ObjectInstance;

        public MethodInfo UpdateMethod;

        public void Start()
        {
            try
            {
                var type = Type.GetType(TypeName);
                ObjectInstance = Activator.CreateInstance(type);
                UpdateMethod = type.GetMethod("Update");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when activate object form loaded assembly. Type: {TypeName}");
                Debug.LogException(e);
                OnError.Invoke();
            }
        }

        private void Update()
        {
            UpdateMethod?.Invoke(ObjectInstance, null);
        }
    }
}