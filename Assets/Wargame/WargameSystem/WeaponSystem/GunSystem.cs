using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wargame.WeaponSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class GunSystem : MonoBehaviour
    {
        public GunSystem_SO gun;
        public Transform firePos;
        public Entity gunHolder;
        public int currentMagCount;
        public int currentMagSize;
        
        public float moveSpeedMultiply { get; private set; }

        #region Private Fields
        private AudioSource _audio;
        private CinemachineImpulseSource _shakeSource;

        private Vector3 _startPos;
        private Vector3 _gunTargetPos;
        private Vector3 _gunAimVel;
        private bool _isHolding;
        private bool _isReloading;
        private bool _isAimReadying;
        private bool _canShoot = true;
        private float _spread;
        private float _prevShootTime;
        #endregion
        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            _audio.playOnAwake = false;
            _shakeSource = GetComponent<CinemachineImpulseSource>();
        }

        public void ResetGun()
        {
            currentMagCount = gun.magMaxCount;
            currentMagSize = gun.magSize;
            moveSpeedMultiply = gun.moveSpeedMultiplier;
            _spread = gun.spread;
            _gunTargetPos = _startPos;
        }
        void Start()
        {
            _startPos = transform.localPosition;
            ResetGun();
        }

        void Update()
        {
            HandleAimMotion();
        }

        public bool StartShoot()
        {
            if (_isAimReadying || _isReloading || !_canShoot || _isHolding && gun.shootingType == ShootingType.Press)
                return false;
            
            

            if (currentMagSize <= 0)
            {
                if (!_isHolding)
                    _audio.PlayOneShot(gun.drySFX);
                _isHolding = true;
                return false;
            }
            
            _isHolding = true;
            _canShoot = false;
            
            _audio.PlayOneShot(gun.shootSFX);

            if(_shakeSource)
                _shakeSource.GenerateImpulse();

            WeaponStyle style = gun.weaponStyle;
            switch (style)
            {
                case WeaponStyle.Hitscan:
                    StartCoroutine(HandleShooting());
                    break;
                case WeaponStyle.Projectile:
                    // Rigidbody 총알을 생성하여 발사하도록 함
                    break;
                case WeaponStyle.Melee:
                    MeleeAttack();
                    break;
            }
            Invoke(nameof(EndShoot), gun.fireRate);
            return true;
        }

        public void StopShoot()
        {
            _isHolding = false;
        }
        
        
        public void StartAim()
        {
            if (!gun.canAim)
                return;
            
            _spread = gun.aimSpread / 2f;
            moveSpeedMultiply = gun.aimMoveSpeedMultiplier;

            _gunTargetPos = gun.aimPosition;
        }


        public void StopAim()
        {
            if (!gun.canAim)
                return;
            
            _spread = gun.spread / 2f;
            moveSpeedMultiply = gun.moveSpeedMultiplier;

            _gunTargetPos = _startPos;
        }

        public void StartReload()
        {
            if (_isReloading || currentMagCount <= 0 || currentMagSize == gun.magSize)
                return;
            _isReloading = true;
            currentMagCount--;
            _audio.PlayOneShot(gun.reloadSFX);
            Invoke(nameof(EndReload), gun.reloadTime);
        }

        private void EndReload()
        {
            _isReloading = false;
            _canShoot = true;
            currentMagSize = gun.magSize;
        }

        private void EndShoot()
        {
            _canShoot = true;
        }

        IEnumerator HandleShooting()
        {
            if (gun.customAmmoConsume > 0)
                currentMagSize -= gun.customAmmoConsume;
            for (int i = 0; i < gun.fireCount; i++)
            {
                HitscanAttack();
                yield return new WaitForSeconds(gun.delayBetweenFire);
            }
        }

        private void HitscanAttack()
        {
            if (gun.customAmmoConsume == 0)
                currentMagSize--;

            Vector3 spreadDir = Quaternion.Euler(0, Random.Range(-_spread, _spread), Random.Range(-_spread, _spread)) * gunHolder.head.forward;

            if (gun.muzzleFX)
            {
                GameObject muzzleFX = Instantiate(gun.muzzleFX, firePos);
                muzzleFX.transform.forward = firePos.forward;
                Destroy(muzzleFX, 0.5f);
            }

            RaycastHit[] hits = Physics.RaycastAll(gunHolder.head.position, spreadDir, 50f);
            if (hits.Length > 0)
            {
                RaycastHit hit = default;
                Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
                foreach (var h in hits)
                {
                    hit = h;
                    Entity targetEntity = h.collider.GetComponentInParent<Entity>();
                    if (targetEntity && targetEntity.team == gunHolder.team)
                        continue;
                    if (h.collider.GetComponent<Sector>())
                        continue;
                    
                    break;
                }

                if (gun.hitFX)
                {
                    GameObject hitFX = Instantiate(gun.hitFX);
                    hitFX.transform.position = hit.point + hit.normal * 0.1f;
                    hitFX.transform.up = hit.normal;
                    Destroy(hitFX, 0.5f);
                }

                Entity e = hit.collider.CompareTag("Head") ? hit.collider.GetComponentInParent<Entity>() : hit.collider.GetComponent<Entity>();
                
                if (e)
                {
                    e.ApplyDamage(hit.collider.CompareTag("Head") ? (int)(gun.damage * gun.headshotMultiplier) : gun.damage, gunHolder);
                }
            }
        }

        private void MeleeAttack()
        {
            Collider[] cols = Physics.OverlapSphere(gunHolder.head.position + gunHolder.head.forward * 2.5f, 5f);

            foreach (var c in cols)
            {
                Entity e = c.CompareTag("Head") ? c.GetComponentInParent<Entity>() : c.GetComponent<Entity>();

                if (e)
                {
                    e.ApplyDamage(gun.damage, gunHolder);
                }
            }
            if (gun.hitFX)
            {
                Ray ray = new Ray(gunHolder.head.position, gunHolder.head.forward);
                if (Physics.Raycast(ray, out var hit, 5f))
                {
                    GameObject hitFX = Instantiate(gun.hitFX); 
                    hitFX.transform.position = hit.point + hit.normal * 0.1f;
                    hitFX.transform.up = hit.normal;
                    Destroy(hitFX, 0.5f);
                }
            }
            
        }

        public void HandleAimMotion()
        {
            if (Vector3.Distance(_gunTargetPos, transform.localPosition) > 0.01f)
            {
                _isAimReadying = true;
                transform.localPosition =
                    Vector3.SmoothDamp(transform.localPosition, _gunTargetPos, ref _gunAimVel, gun.aimSpeed);
            }
            else
            {
                _isAimReadying = false;
            }
        }
    }
}
