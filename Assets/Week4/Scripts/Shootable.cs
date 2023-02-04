using UnityEngine;
using UnityEngine.Pool;

namespace Week4
{
    public class Shootable : MonoBehaviour
    {
        public int health = 100;
        
        public void OnDamaged(int damage)
        {
            this.health -= damage;
            
            if(this.health <= 0)
                OnDeath();
        }

        void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}