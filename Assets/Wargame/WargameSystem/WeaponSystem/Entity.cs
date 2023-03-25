using UnityEngine;
using UnityEngine.Events;

namespace Wargame.WeaponSystem
{
    public class Entity : MonoBehaviour
    {
        public int maxHealth = 100;
        public int curHealth;
        public int team = -1;
        public Transform head;
        public bool isGodMode;

        public UnityEvent onDamaged;
        public UnityEvent<Entity> onDeath;
        
        void Start()
        {
            curHealth = maxHealth;
        }
        
        public void ApplyDamage(int damage, Entity attacker)
        {
            if (isGodMode)
                return;
            if (attacker.team == team)
                return;
            
            curHealth -= damage;
            
            onDamaged?.Invoke();
            
            if (curHealth <= 0)
            {
                onDeath?.Invoke(this);
            }
        }
    }
}