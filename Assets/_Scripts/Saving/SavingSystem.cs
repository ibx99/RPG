using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        private string lastSceneBuildIndexString = "lastSceneBuildIndex";

        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);

            if (state.ContainsKey(lastSceneBuildIndexString))
            {
                int buildIndex = (int)state[lastSceneBuildIndexString];

                if (SceneManager.GetActiveScene().buildIndex != buildIndex)
                {
                    yield return SceneManager.LoadSceneAsync(buildIndex);
                }
            }
            RestoreState(state);
        }


        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }


        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }


        private void SaveFile(string saveFile, object state)
        {
            string filePath = GetPathFromSaveFile(saveFile);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, state);
            }
        }


        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string filePath = GetPathFromSaveFile(saveFile);

            if (!File.Exists(filePath))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(fileStream);
            }
        }


        public void CaptureState(Dictionary<string, object> state)
        {            
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            state[lastSceneBuildIndexString] = SceneManager.GetActiveScene().buildIndex;
        }


        public void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string uniqueId = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(uniqueId))
                {
                    saveable.RestoreState(state[uniqueId]);
                }
            }
        }


        private string GetPathFromSaveFile(string saveFile)
            => Path.Combine(Application.persistentDataPath, saveFile + ".sav");

    }
}
