using System.Collections.Generic;
using UnityEngine;
using Wargame.WeaponSystem;

namespace Wargame.AISystem
{
    public class FindState : State
    {
        public CombatState combatState;
        private AIController _control;
        private Sector _foundSector;
        private bool _isChasing;
        private Vector3 _destPos;
        private bool _isMoving = true;
        private float _lastIdleTime;
        private static readonly int IsMovingID = Animator.StringToHash("isMoving");

        private void Start()
        {
            _control = GetComponentInParent<AIController>();
        }
        
        
        public override State Run()
        {
            if (_control.DetectEnemy())
                return combatState;

            _control.animator.SetBool(IsMovingID, _isMoving);

            if (_isChasing)
            {
                if (Vector3.Distance(transform.position, _destPos) <= 4f)
                {
                    _isChasing = false;
                }
            }

            if (!_isChasing && _isMoving && (!_foundSector || _foundSector.owningTeam == _control.entity.team))
            {
                List<Sector> foundSectors = new List<Sector>();
                foreach (var sector in _control.sectors)
                {
                    if (sector.owningTeam != _control.entity.team)
                    {
                        foundSectors.Add(sector);
                    }
                }

                if (foundSectors.Count == 0)
                {
                    List<Entity> enemies = new List<Entity>();
                    for (int i = 0; i < GameManager.inst.teams.Count; i++)
                    {
                        if (i != _control.entity.team)
                        {
                            enemies.AddRange(GameManager.inst.teams[i].players);
                        }
                    }

                    _foundSector = null;
                    _isChasing = true;
                    _destPos = enemies[Random.Range(0, enemies.Count)].transform.position;
                }
                else
                {
                    _foundSector = foundSectors[Random.Range(0, foundSectors.Count)];
                    _destPos = _foundSector.transform.position;
                }
                
            }
            _control.agent.SetDestination(_destPos);

            return this;
        }
    }
}