using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Wargame.WeaponSystem
{
    public class Entity : MonoBehaviour
    {
        public int maxHealth = 100;
        public int team = -1;
        public Transform head;

        public int curHealth { get; private set; }
        public UnityEvent onDamaged;
        public UnityEvent onDeath;
        
        void Start()
        {
            curHealth = maxHealth;
        }
        public void ApplyDamage(int damage)
        {
            curHealth -= damage;
            
            onDamaged?.Invoke();
            
            if (curHealth <= 0)
            {
                onDeath?.Invoke();
            }
        }
    }
}