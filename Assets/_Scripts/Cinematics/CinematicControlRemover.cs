using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using RPG.Controller;
using RPG.Core;


namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        // caching vars
        PlayableDirector playableDirector;
        GameObject player;

        private void Start()
        {
            // caching
            playableDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindGameObjectWithTag("Player");

            // subscribing to the events
            playableDirector.played += CinematicsStarted;
            playableDirector.stopped += CinematicsStopped;
        }

        // when cinematics started
        private void CinematicsStarted(PlayableDirector obj)
        {
            // cancel the current action of player and disable control
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }


        // when cinematics finished
        private void CinematicsStopped(PlayableDirector obj)
        {
            // enable control for the player
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}