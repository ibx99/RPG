using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.GetChild(i).position, 0.5f);

                int j = GetNextIndex(i);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(GetWaypoint(i), transform.GetChild(j).position);
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetNextIndex(int index)
        {
            if (index + 1 == transform.childCount)
            {
                return 0;
            }
            return index + 1;
        }
    }
}