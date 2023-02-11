using System;
using UnityEngine;
using UnityEngine.AI;

namespace Week5
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavTest : MonoBehaviour
    {
        public Transform target;
        private NavMeshAgent _agent;
        
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            _agent.SetDestination(target.position);
        }
    }
}