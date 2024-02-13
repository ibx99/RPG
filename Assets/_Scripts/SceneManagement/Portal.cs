using System.Collections;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum PortalIdentifier
        { A, B, C, D }

        [SerializeField] private int sceneToLoad;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] PortalIdentifier destination;
        [SerializeField] private float fadeOutTime = 1f;
        [SerializeField] private float fadeInTime = 2f;
        [SerializeField] private float fadeWaitTime = 1f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition(sceneToLoad));
            }
        }


        private IEnumerator Transition(int sceneIndex)
        {
            // for invalid scene index
            if (sceneIndex < 0)
            {
                Debug.LogError("Scene index not correct on " + this.name);
                yield break;
            }

            // remove the portal from the parent object in order for dont destroy to run correctly
            transform.parent = null;
            DontDestroyOnLoad(this.gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            yield return fader.FadeOut(fadeOutTime);

            savingWrapper.Save();

            // when scene is completely loaded
            yield return SceneManager.LoadSceneAsync(sceneIndex);

            savingWrapper.Load();

            Portal portal = GetOtherPortal();
            UpdatePlayer(portal);

            // to save the player position in the new scene
            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);


            Destroy(this.gameObject);
        }


        private Portal GetOtherPortal()
        {
            Portal portalToReturn = null;

            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal.gameObject == this.gameObject) continue;
                if (portal.destination != this.destination) continue;
                    
                portalToReturn = portal;
            }
            return portalToReturn;
        }


        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            player.GetComponent<NavMeshAgent>().enabled = false;

            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}