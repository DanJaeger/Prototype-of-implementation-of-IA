using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA
{
    public class FieldOfView : MonoBehaviour
    {
        StateManager stateManager;
        Transform playerTransform;

        public float viewRadius = 0.0f;
        [Range(0, 180)] public float viewAngle = 0.0f;

        [SerializeField] LayerMask targetMask;

        public List<Transform> visibleTargets = new List<Transform>();

        private void Start()
        {
            stateManager = GetComponentInParent<StateManager>();
            playerTransform = stateManager.gameObject.transform;
        }

        void FindVisibleTargets()
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for(int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 directionToTarget = (target.position - playerTransform.position).normalized;
                if(Vector3.Angle(playerTransform.forward, directionToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(playerTransform.position, target.position);
                }
            }
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
