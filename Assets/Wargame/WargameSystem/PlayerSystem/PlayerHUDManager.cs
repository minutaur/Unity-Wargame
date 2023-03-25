using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Wargame.WeaponSystem;

namespace Wargame.PlayerSystem
{
    public class PlayerHUDManager : MonoBehaviour
    {
        public static PlayerHUDManager inst;

        public TextMeshProUGUI ammoText;
        public Image healthUI, healthMotionUI;
        public Image captureUI;
        public Entity playerEntity;
        
        private Sector _currentSector;

        void Awake()
        {
            if (!inst)
            {
                inst = this;
            }
        }
        
        private void Update()
        {
            UpdateSector();
        }

        public void ChangeHealth(float ratio)
        {
            healthUI.fillAmount = ratio;
            StopCoroutine(nameof(HealthMotion));
            StartCoroutine(HealthMotion(ratio));
        }

        IEnumerator HealthMotion(float ratio)
        {
            while (healthMotionUI.fillAmount - ratio > 0.01f)
            {
                healthMotionUI.fillAmount = Mathf.Lerp(healthMotionUI.fillAmount, ratio, Time.deltaTime * 3f);
                yield return null;
            }

            healthMotionUI.fillAmount = ratio;
        }

        public void ChangeAmmo(int maxAmmo, int curAmmo)
        {
            ammoText.text = curAmmo + " / " + maxAmmo;
        }

        public void SetSector(Sector sector)
        {
            captureUI.gameObject.SetActive(true);
            _currentSector = sector;
        }

        public void UnsetSector()
        {
            captureUI.gameObject.SetActive(false);
        }

        public void UpdateSector()
        {
            if (!_currentSector)
                return;

            float ratio = _currentSector.curConquerPercent / _currentSector.conquerTime;
            Vector2 newSize = new Vector2(Mathf.Lerp(0, 600, ratio), 20);
            captureUI.rectTransform.sizeDelta = newSize;
            
            if (_currentSector.owningTeam < 0 || playerEntity.team == _currentSector.owningTeam)
            {
                captureUI.color = Color.cyan;
            }
            else
            {
                captureUI.color = Color.red;
            }
        }
    }
}