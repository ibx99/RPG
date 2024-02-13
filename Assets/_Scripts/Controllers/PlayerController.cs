using UnityEngine;

using RPG.Combat;
using RPG.Movement;
using RPG.Core;

namespace RPG.Controller
{
    public class PlayerController : MonoBehaviour
    {

        #region Vars

        private Camera cam;
        private Mover mover;
        private Health health;


        #endregion


        // =====================================

        
        #region Unity

        private void Start()
        {
            cam = Camera.main;
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            // return if health is 0
            if (health.IsDead()) return;

            // for cusor affordance

            if (InteractWithCombat()) return;

            if (InteractWithMovement()) return;
        }

        #endregion


        // =====================================


        #region Private Methods

        private bool InteractWithCombat()
        {
            // create a ray to hit all objects and return as a list
            RaycastHit[] hitList =  Physics.RaycastAll(GetMouseRay());

            foreach(RaycastHit hit in hitList)
            {
                // whether mouse on enemy or not
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();

                // if the target doesn't have a combat target script then move on
                if (target == null) continue;
                
                // check for next item in raycast hit list if there is either no enemy
                // or if the enemy is dead
                if (!GetComponent<Fighting>().CanAttack(target.gameObject)) continue;

                // attack when mouse down
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Fighting>().Attack(target.gameObject);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool isHit = Physics.Raycast(GetMouseRay(), out hit);

            // when hit something, move to that point
            if (isHit)
            {
                // when mouse button clicked - cancell attack then move
                if (Input.GetMouseButton(0))
                    mover.StartMoveAction(hit.point, 1f);
    
                return true;
            }
            return false;
        }

        private Ray GetMouseRay()
        {
            return cam.ScreenPointToRay(Input.mousePosition);
        }

        #endregion


    }
}