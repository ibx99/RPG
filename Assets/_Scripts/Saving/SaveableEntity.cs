using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string uniqueIdentifier = string.Empty;
        static Dictionary<string, SaveableEntity> globalLookUp = new Dictionary<string, SaveableEntity>();


        public string GetUniqueIdentifier() => uniqueIdentifier;

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable iSaveable in GetComponents<ISaveable>())
            {
                string saveableName = iSaveable.GetType().ToString();
                if (stateDict.ContainsKey(saveableName))
                {
                    iSaveable.RestoreState(stateDict[saveableName]);
                }
            }
        }

        public object CaptureState()
        {
            Dictionary<string, object> stateDict = new Dictionary<string, object>();
            foreach (ISaveable iSaveable in GetComponents<ISaveable>())
            {
                stateDict[iSaveable.GetType().ToString()] = iSaveable.CaptureState();
            }
            return stateDict;
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;   

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }
            globalLookUp[property.stringValue] = this;
        }
#endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookUp.ContainsKey(candidate)) return true;
            
            if (globalLookUp[candidate] == this) return true;
        
            // for when we change scene
            if (globalLookUp[candidate] == null)
            {
                globalLookUp.Remove(candidate);
                return true;
            }

            // incase when we change the uid value through inspector
            if (globalLookUp[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookUp.Remove(candidate);
                return true;
            }

            return false;
        }
    }


}