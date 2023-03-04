using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using Wargame.WeaponSystem;

namespace Wargame.PlayerSystem
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        public CinemachineVirtualCamera playerCam;
        public List<GunSystem> inventory;
        public GunSystem current;
        private WargameInput _input;

        private Entity _selfEntity;
        private bool _wasAim;
        private float _defaultFOV;
        private float _currentFOVSpeed;

        private void Start()
        {
            _input = GetComponent<WargameInput>();
            _defaultFOV = playerCam.m_Lens.FieldOfView;
            current = inventory[0];

            _selfEntity = GetComponent<Entity>();
            foreach (var gun in inventory)
            {
                gun.gunHolder = _selfEntity;
            }

            _selfEntity.onDamaged.AddListener(OnDamaged_UpdateUI);
        }

        public void OnDamaged_UpdateUI()
        {
            PlayerHUDManager.inst.ChangeHealth((float)_selfEntity.curHealth / _selfEntity.maxHealth);
        }

        public void ChangeWeapon(int index)
        {
            if (index >= inventory.Count)
            {
                Debug.LogWarning(index + "번 무기가 등록되어 있지 않습니다");
                return;
            }

            current.gameObject.SetActive(false);
            current = inventory[index];
            current.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (inventory.Count == 0)
                return;

            PlayerHUDManager.inst.ChangeAmmo(current.currentMagCount * current.gun.magSize, current.currentMagSize);


            if (_input.fire)
                current.StartShoot();
            else if (!_input.fire)
                current.StopShoot();

            if (!_wasAim && _input.aim)
            {
                current.StartAim();
            }
            else if (_input.aim)
            {
                playerCam.m_Lens.FieldOfView = Mathf.SmoothDamp(playerCam.m_Lens.FieldOfView,
                    current.gun.aimFOV, ref _currentFOVSpeed,
                    current.gun.aimSpeed);
            }
            else if (_wasAim && !_input.aim)
            {
                current.StopAim();
            }
            else if (!_input.aim)
            {
                playerCam.m_Lens.FieldOfView = Mathf.SmoothDamp(playerCam.m_Lens.FieldOfView, _defaultFOV,
                    ref _currentFOVSpeed,
                    current.gun.aimSpeed);
            }

            if (_input.reload)
                current.StartReload();

            _wasAim = _input.aim;
        }
    }
}