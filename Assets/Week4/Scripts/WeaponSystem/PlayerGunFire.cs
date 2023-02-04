using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WeaponSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGunSelector))]
    public class PlayerGunFire : MonoBehaviour
    {
        private PlayerGunSelector gunSelector;

        private void Start()
        {
            gunSelector = GetComponent<PlayerGunSelector>();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.isPressed && gunSelector.activeGun)
            {
                gunSelector.activeGun.Shoot();
            }
        }
    }
}
