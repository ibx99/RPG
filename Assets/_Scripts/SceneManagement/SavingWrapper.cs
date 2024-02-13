using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;
        private const string SaveFileName = "save";
        private SavingSystem savingSystem;

        private IEnumerator Start()
        {
            savingSystem = GetComponent<SavingSystem>();

            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return savingSystem.LoadLastScene(SaveFileName);
            yield return fader.FadeIn(fadeInTime);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                savingSystem.Save(SaveFileName);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                savingSystem.Load(SaveFileName);
            }
        }

        public void Save()
        {
            savingSystem.Save(SaveFileName);
        }

        public void Load()
        {
            savingSystem.Load(SaveFileName);
        }
    }
}