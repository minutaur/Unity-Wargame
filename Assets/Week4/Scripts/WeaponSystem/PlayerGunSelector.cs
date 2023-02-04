using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    [DisallowMultipleComponent]
    public class PlayerGunSelector : MonoBehaviour
    {
        [SerializeField] private GunType gunType;
        [SerializeField] private Transform gunParent;
        [SerializeField] private List<GunScriptable> guns;

        [Space]
        [Header("Runtime Filled")]
        public GunScriptable activeGun;

        private void Start()
        {
            GunScriptable gun = guns.Find(gun => gun.type == gunType);

            if (!gun)
            {
                Debug.LogError($"{gunType} 인 Gun을 찾지 못하였습니다");
                return;
            }

            activeGun = gun;
            gun.Spawn(gunParent, this);
        }
    }
}