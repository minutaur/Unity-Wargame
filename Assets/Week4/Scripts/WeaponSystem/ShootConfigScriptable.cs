using UnityEngine;
using UnityEngine.Serialization;

namespace WeaponSystem
{
    [CreateAssetMenu(fileName = "Shoot Config", menuName = "FPS/Shoot Configuration", order = 2)]
    public class ShootConfigScriptable : ScriptableObject
    {
        public float fireRate = .25f;
    }
}