using UnityEngine;

using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System.Collections;
using RPG.Control;
using System;
using UnityEngine.AI;

namespace RPG.Controller
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float timeToWaitBeforeReturnToGaurdPosition = 2f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 3f;
        [Range(0, 1)] [SerializeField] private float patrolSpeedFraction;

        private GameObject player;
        private Fighting fighting;
        private Health health;
        private Mover mover;
        private ActionScheduler actionScheduler;
        private NavMeshAgent agent;

        private int currentWaypointIndex = 0;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private Vector3 gaurdingPosition;
        private float timeSinceArrivedWaypoint = Mathf.Infinity;
        
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            fighting = GetComponent<Fighting>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            agent = GetComponent<NavMeshAgent>();

            gaurdingPosition = transform.position;
        }

        void Update()
        {
            if (health.IsDead()) return;

            // when in range and when player has a health component with health remaining
            if (InAttackRangeOfPlayer() && fighting.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (WaitTimeFinished())
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceArrivedWaypoint += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
        }


        #region Gaurd Behaviors

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0f;
            fighting.Attack(player);
        }
        

        private void SuspicionBehaviour()
        {
            actionScheduler.CancelCurrentAction();
        }


        private void PatrolBehaviour()
        {
            Vector3 nextPosition = gaurdingPosition;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedWaypoint > waypointDwellTime)
            {
                timeSinceArrivedWaypoint = 0f;
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }


        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }


        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }


        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        #endregion



        #region Helper Methods

        private bool WaitTimeFinished()
        {
            return timeSinceLastSawPlayer <= timeToWaitBeforeReturnToGaurdPosition;
        }

        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance;
        }

        #endregion


        #region Gizmo Calls

        // unity calls -- gizmo
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }


        #endregion


    }


}