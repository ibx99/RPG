using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core { 
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        public void StartAction(IAction action)
        {
            if (currentAction == action) return;

            if (currentAction != null)
                currentAction.Cancel();
    
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            /// when passed null into the startaction method
            /// current action gets cancelled and current action gets
            /// set as null
            StartAction(null);
        }
    }
}