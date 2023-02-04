using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Pool;

namespace WeaponSystem
{
    [CreateAssetMenu(fileName = "Gun", menuName = "FPS/Gun", order = 0)]
    public class GunScriptable : ScriptableObject
    {
        public GunType type;
        public new string name;
        public GameObject modelPrefab;
        public Vector3 spawnPoint;
        public Vector3 spawnRotation;
        public VisualEffect hitVFX;

        public ShootConfigScriptable shootConfig;
        public TrailConfigScriptable trailConfig;

        private MonoBehaviour activeMonoBehaviour;
        private GameObject model;
        private float lastShootTime;
        private VisualEffect shootVFX;
        private ObjectPool<TrailRenderer> trailPool;

        private Camera _fpsCam;

        public void Spawn(Transform parent, MonoBehaviour monoBehaviour)
        {
            activeMonoBehaviour = monoBehaviour;
            lastShootTime = 0;
            trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
            model = Instantiate(modelPrefab, parent, false);
            model.transform.localPosition = spawnPoint;
            model.transform.localRotation = Quaternion.Euler(spawnRotation);

            shootVFX = model.GetComponentInChildren<VisualEffect>();
            _fpsCam = Camera.main;
        }

        public void Shoot()
        {
            if (Time.time > shootConfig.fireRate + lastShootTime)
            {
                lastShootTime = Time.time;
                shootVFX.Play();

                Vector3 rayOrigin = _fpsCam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));
                Vector3 shootDir = _fpsCam.transform.forward;
                if (Physics.Raycast(rayOrigin, shootDir, out RaycastHit hit))
                {
                    activeMonoBehaviour.StartCoroutine(PlayTrail(shootVFX.transform.position, hit.point, hit));
                }
                else
                {
                    activeMonoBehaviour.StartCoroutine(
                        PlayTrail(shootVFX.transform.position,
                            shootVFX.transform.position + (shootDir * trailConfig.missDistance), new RaycastHit()));
                }
            }
        }

        private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
        {
            TrailRenderer instance = trailPool.Get();
            instance.gameObject.SetActive(true);
            instance.transform.position = startPoint;
            yield return null;

            instance.emitting = true;

            float distance = Vector3.Distance(startPoint, endPoint);
            float remainingDistance = distance;
            while (remainingDistance > 0)
            {
                instance.transform.position = Vector3.Lerp(
                    startPoint,
                    endPoint,
                    Mathf.Clamp01(1 - (remainingDistance / distance))
                );
                remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

                yield return null;
            }

            instance.transform.position = endPoint;

            if (hit.collider)
            {
                Transform go = Instantiate(hitVFX).transform;
                go.position = hit.point;
                go.up = hit.normal;
                Destroy(go.gameObject, .5f);
            }
            
            yield return new WaitForSeconds(trailConfig.duration);
            instance.emitting = false;
            instance.gameObject.SetActive(false);
            trailPool.Release(instance);
        }

        private TrailRenderer CreateTrail()
        {
            GameObject instance = new GameObject("Bullet Trail");
            TrailRenderer trail = instance.AddComponent<TrailRenderer>();
            trail.colorGradient = trailConfig.color;
            trail.material = trailConfig.material;
            trail.widthCurve = trailConfig.widthCurve;
            trail.time = trailConfig.duration;
            trail.minVertexDistance = trailConfig.minVertexDistance;

            trail.emitting = false;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return trail;
        }
    }
}