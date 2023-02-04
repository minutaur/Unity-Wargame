using UnityEngine;

namespace WeaponSystem
{
    public class Entity : MonoBehaviour
    {
        public int health = 100;
        
        public void OnDamaged(int damage)
        {
            health -= damage;
            
            if (health <= 0)
                OnDeath();
        }

        void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}