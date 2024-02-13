using System.Collections;
using UnityEngine;

using RPG.Movement;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighting : MonoBehaviour, IAction
    {

        [SerializeField] private float weaponRange = 2.0f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] private float weaponDamage;

        // for caching target and mover script
        Mover mover;
        Health targetToAttack;

        // for time counter -- attack timer
        private float timeSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            // helps in cancelling the attack
            if (targetToAttack == null) return;
            // ignore chasing or attack behavior when dead
            if (targetToAttack.IsDead()) return;

            if (!GetRange())
            {
                mover.MoveTo(targetToAttack.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }


        private bool GetRange()
        {
            return Vector3.Distance(targetToAttack.transform.position, transform.position) <= weaponRange;
        }


        public void Attack(GameObject target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            targetToAttack = target.GetComponent<Health>();
        }


        private void AttackBehaviour()
        {
            // look at the enemy before attacking 
            transform.LookAt(targetToAttack.transform.position, Vector3.up);

            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // this will trigger the Hit() event
                TriggerAttackAnimation();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttackAnimation()
        {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("UnArmedAttack");
        }


        public void Cancel()
        { 
            if (targetToAttack != null)
                targetToAttack = null;

            GetComponent<Animator>().ResetTrigger("UnArmedAttack");
            GetComponent<Animator>().SetTrigger("StopAttack");
            mover.Cancel();
        }


        public bool CanAttack(GameObject target)
        {
            Health health = target.GetComponent<Health>();
            return health != null && !health.IsDead();
        }


        // animation event
        private void Hit()
        {
            // checking null for target as the character is currently 
            // performing the animation even after being attack action get cancelled.
            if (targetToAttack == null) return; 

            targetToAttack.TakeDamage(weaponDamage);
        }
    }
}