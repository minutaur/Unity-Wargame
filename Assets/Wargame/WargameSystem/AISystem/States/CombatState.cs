using UnityEngine;
using Wargame.WeaponSystem;

namespace Wargame.AISystem
{//전투
    public class CombatState : State
    {
        public FindState findState;
        private AIController _control;
        private bool _isMoving;
        private Vector3 lastSeenLocation;
        private static readonly int IsMovingID = Animator.StringToHash("isMoving");
        private static readonly int ShootID = Animator.StringToHash("Shoot");

        private void Start()
        {
            _control = GetComponentInParent<AIController>();
        }

        public override State Run()
        {
            Entity e = _control.DetectEnemy();
            if (e)
            {
                if (_control.holdingGun.currentMagSize == 0)
                    _control.holdingGun.StartReload();
                else if(_control.holdingGun.StartShoot())
                    _control.animator.SetTrigger(ShootID);
                lastSeenLocation = e.transform.position;
                Vector3 targetDir = e.head.position - _control.entity.head.position;
                _control.transform.forward = new Vector3(targetDir.x, 0, targetDir.z).normalized;
                _control.agent.SetDestination(transform.position);
                _isMoving = false;
            }
            else
            {
                _control.agent.SetDestination(lastSeenLocation);
                _isMoving = true;
                if (Vector3.Distance(transform.position, lastSeenLocation) <= 4f)
                {
                    return findState;
                }
            }
            
            _control.animator.SetBool(IsMovingID, _isMoving);
            return this;
        }
    }
}