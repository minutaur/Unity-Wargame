using UnityEngine;
using UnityEngine.AI;

namespace Week5
{
    public enum AIState
    {
        Idle, Chase, Combat
    }
    public class StateMachine : MonoBehaviour
    {
        public AIState _currentState;
        public Transform head;
        public Transform target;
        public float detectionRadius = 20f;
        [Range(1, 360)]
        public int detectionAngle = 120;

        public float detectionLostTime = 5f;
        public float combatRadius = 8f;

        private float _lastDetectedTime;
        private NavMeshAgent _agent;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            UpdateState();
        }

        public bool CanDetectTarget()
        {
            Vector3 targetDir = target.position - transform.position;

            bool inRadius = targetDir.magnitude <= detectionRadius;
            
            
            bool inAngle = Vector3.Angle(head.forward, targetDir.normalized) <= detectionAngle;
            
            
            bool notBlocked = true;
            if (Physics.Raycast(head.position, targetDir.normalized, out var hit, detectionRadius))
            {
                notBlocked = hit.transform == target;
            }
            
            return inRadius && inAngle && notBlocked;
        }

        public void UpdateState()
        {
            switch (_currentState)
            {
                case AIState.Idle:
                    if (CanDetectTarget())
                        ChangeState(AIState.Chase);
                    break;
                case AIState.Chase:

                    if (CanDetectTarget())
                    {
                        _lastDetectedTime = Time.time;
                        _agent.SetDestination(target.position);
                    } else if (Time.time > _lastDetectedTime + detectionLostTime)
                        ChangeState(AIState.Idle);
                    if (Vector3.Distance(target.position, transform.position) < combatRadius)
                        ChangeState(AIState.Combat);
                    break;
                case AIState.Combat:
                    Debug.Log("공격!!!");
                    ChangeState(AIState.Chase);
                    break;
            }
        }

        public void ChangeState(AIState newState)
        {
            Debug.Log("현재 상태는 : " + newState);
            _currentState = newState;
        }
    }
}