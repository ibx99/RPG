using UnityEngine;
using UnityEngine.AI;

using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {

        #region Vars

        Animator animator;
        NavMeshAgent agent;
        private Health health;
        [SerializeField] private float maxSpeed;

        #endregion

        // =====================================

        #region Unity

        void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }


        void Update()
        {
            // disabling the agent when dead
            agent.enabled = !(health.IsDead());

            // handle animation
            HandleAnimation();
        }

        #endregion

        // =====================================

        #region Private Methods

        private void HandleAnimation()
        {
            // refer to self note below

            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
            localVelocity.z = Mathf.Abs(localVelocity.z);

            animator.SetFloat("forwardSpeed", localVelocity.z);
        }

        #endregion

        // =====================================

        #region Public Methods

        // when called -- cancel attack and then move
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        // setting the destination of the agent -- only needs one frame to be set
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
            agent.SetDestination(destination);
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        // saving and loading data
        public void RestoreState(object state)
        {
            Vector3 position = ((SerializableVector3)state).ToVector();
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position;
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();

        }
        
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        #endregion



    }

}






/// The reason InverseTransfromDirection is used is that, the animation clip has average velocity
/// What it means is that the clip will perfectly align with the avatar travelling at that average 
/// velocity. That is why we need avatar or character to move at that speed in order to perfectly 
/// work. 
/// Nav mesh agent max speed is set to the same value that is 5.66 for the run animation.
/// 
/// ======= We need the forward velocity vector of the agent as the animation is about forward movement ===========
/// 
/// So basically we have 2 ways of doing this, 
/// First: We can directly plug the magnitude of the agent velocity which will be 5.66
/// But this will create a problem, whenever the agent will turn, the magnitude of the velocity will mount 
/// quickly than the forward velocity vector.
/// Second: we can take the forward velocity vector and plug it into the animator blend threshold.
/// But for that we have to convert the velocity into local because if we don't then the velocity we will 
/// recieve will be global. For example, if we avatar is moving towards the bottom left of the screen the, 
/// global velocity vectors will be in negative as it is going towards the 3rd quadrant of global, but when used after
/// Transfrom.InverseTransformDirection then, the facing direction and final moving direction will determine the 
/// velocity values.

















