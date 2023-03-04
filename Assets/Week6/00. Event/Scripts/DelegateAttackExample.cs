using UnityEngine;

namespace Week6
{
    public class DelegateAttackExample : MonoBehaviour
    {
        delegate void MyDelegate();
        MyDelegate attack;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (attack != null)
                {
                    attack();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                attack = PrimaryAttack;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                attack = SecondaryAttack;
            }
        }
        void PrimaryAttack()
        {
            // Primary attack
        }
        void SecondaryAttack()
        {
            // Secondary attack
        }
    }
}