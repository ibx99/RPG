using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float healthPoint;

        private bool isDead = false;

        // sort of getter for isDead var
        public bool IsDead()
        {
            return isDead;
        }


        public void TakeDamage(float damageRecieved)
        {
            // to make sure the value of health doesnt go below 0
            healthPoint = Mathf.Max(healthPoint - damageRecieved, 0);

            if (healthPoint == 0)
            {
                Die(); 
            }
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");

            // cancelling whatever action the current character is
            // like if attacking, setting the target to null and 
            // when in movement, stop moving the agent
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }


        public object CaptureState()
        {
            return healthPoint;
        }

        public void RestoreState(object state)
        {
            healthPoint = (float)state;
            if (healthPoint <= 0f)
                Die();
        }
    }
}