using UnityEngine;
using UnityEngine.InputSystem;

namespace Week4
{
    [RequireComponent(typeof(AudioSource))]
    public class Hitscan_Shoot : MonoBehaviour
    {
        public int damage = 10;
        public float fireRate = 0.25f;
        public float weaponRange = 50f;
        public float hitForce = 100f;
        public GameObject vfxHit;
        
        private Camera _fpsCam;
        private float _nextFire;
        private AudioSource _sfxFire;


        void Start()
        {
            _fpsCam = Camera.main;
            _sfxFire = GetComponent<AudioSource>();
            _sfxFire.playOnAwake = false;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.isPressed && Time.time > _nextFire)
            {
                _nextFire = Time.time + fireRate;
                
                Vector3 rayOrigin = _fpsCam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

                _sfxFire.Play();
                
                RaycastHit hit;
                if (Physics.Raycast (rayOrigin, _fpsCam.transform.forward, out hit, weaponRange))
                {
                    Shootable health = hit.collider.GetComponent<Shootable>();

                    if (health)
                    {
                        health.OnDamaged(damage);
                    }

                    if (hit.rigidbody)
                    {
                        hit.rigidbody.AddForce (-hit.normal * hitForce);
                    }

                    if (vfxHit)
                    {
                        GameObject go = Instantiate(vfxHit);
                        go.transform.position = hit.point;
                        go.transform.up = hit.normal;
                    }
                }
            }
        }
    }
}
