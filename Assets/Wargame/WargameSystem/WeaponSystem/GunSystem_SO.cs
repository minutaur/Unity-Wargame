using UnityEngine;
using UnityEngine.Serialization;

namespace Wargame.WeaponSystem
{

    public enum WeaponStyle
    {
        Hitscan,
        Projectile,
        Melee
    }

    public enum ShootingType
    {
        Press,
        PressHold
    }
    
    [CreateAssetMenu(menuName = "FPS/Gun", fileName = "Gun Config")]
    public class GunSystem_SO : ScriptableObject
    {
        public string weaponName;
        
        public WeaponStyle weaponStyle;
        public ShootingType shootingType;
        public int magMaxCount;
        public int magSize;

        public float fireRate;
        public float reloadTime;
        public int damage;
        [Range(1, 2)] public float headshotMultiplier = 1f;
        [Tooltip("한번에 몇 발을 발사할 것인지")] public int fireCount = 1;
        public float delayBetweenFire;
        [Tooltip("발사당 몇의 탄약을 소모하게 할 것인지, 0이면 발사한 횟수만큼 소모")] public int customAmmoConsume = 0;

        public float spread;
        public float moveSpeedMultiplier = 1f;
        
        [Header("Aim Config")]
        public bool canAim;
        public Vector3 aimPosition;
        
        public float aimMoveSpeedMultiplier = 1f;
        
        [Range(0, 2)] public float aimSpeed = 0.2f;
        [Range(1, 120)] public float aimFOV = 60f;
        public float aimSpread;

        public GameObject muzzleFX, hitFX;
        public AudioClip shootSFX;
        public AudioClip drySFX;
        public AudioClip reloadSFX;
    }
}