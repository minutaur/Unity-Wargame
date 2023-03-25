using System;
using UnityEngine;
using UnityEngine.AI;
using Wargame.WeaponSystem;

namespace Wargame.AISystem
{
    public class AIController : MonoBehaviour
    {
        [Header("Properties")]
        public State currentState;
        public float moveSpeed = 3f;
        public float combatWalkSpeed = 1.5f;
        public GunSystem holdingGun;
        public LayerMask targetLayer;
        public float visibleDistance = 12f;
        
        [Header("Auto-Assign")]
        public NavMeshAgent agent;
        public Entity entity;
        public Animator animator;
        public Sector[] sectors;
        public bool isDeath;
        
        private static readonly int InCombatID = Animator.StringToHash("inCombat");
        private static readonly int IsDeathID = Animator.StringToHash("isDeath");

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            entity = GetComponent<Entity>();
            animator = GetComponent<Animator>();

            agent.speed = moveSpeed;
            holdingGun.gunHolder = entity;
        }

        private void Start()
        {
            entity.onDeath.AddListener(OnAIDeath);
        }

        public void AfterGameInit()
        {
            sectors = GameManager.inst.sectors;
        }

        private void Update()
        {
            RunStateMachine();
        }

        void RunStateMachine()
        {
            if (isDeath)
                return;
            
            State nextState = currentState.Run();

            if (nextState)
            {
                SwitchState(nextState);
            }
        }

        void SwitchState(State nextState)
        {
            if (nextState is FindState)
            {
                animator.SetBool(InCombatID, false);
                agent.speed = moveSpeed;
            }
            else if (nextState is CombatState)
            {
                animator.SetBool(InCombatID, true);
                agent.speed = combatWalkSpeed;
            }
            
            currentState = nextState;
        }
        
        public Entity DetectEnemy()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, visibleDistance, targetLayer);
            
            foreach(var col in cols)
            {
                if (col.TryGetComponent(out Entity e))
                {
                    if (e == entity || e.team == entity.team)
                        continue;
                    
                    Vector3 headDiff = e.head.position - entity.head.position;
                    Vector3 targetDir = new Vector3(headDiff.x, 0, headDiff.z);
                    if (Physics.Raycast(entity.head.position, targetDir.normalized, out var hit, visibleDistance, ~LayerMask.GetMask("Sector")))
                        if (hit.collider.CompareTag("Head") || hit.collider.CompareTag("Player"))
                            return e;
                }
            }
                
            return null;
        }

        private void OnAIDeath(Entity self)  
        {
            isDeath = true;
            entity.curHealth = entity.maxHealth;
            entity.isGodMode = true;
            agent.SetDestination(transform.position);
            animator.SetBool(IsDeathID, isDeath);
            if (entity.team >= 0)
                GameManager.inst.teams[entity.team].RespawnOnDeath(self);
        }

        public void OnAIRespawn()
        {
            isDeath = false;
            entity.isGodMode = false;
            holdingGun.ResetGun();
            animator.SetBool(IsDeathID, isDeath);
        }
    }
}