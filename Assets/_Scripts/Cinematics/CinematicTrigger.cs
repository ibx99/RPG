using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool hasPlayedCinematics = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (!hasPlayedCinematics)
                {
                    GetComponent<PlayableDirector>().Play();
                    hasPlayedCinematics = true;
                }
            }
        }
    }
}