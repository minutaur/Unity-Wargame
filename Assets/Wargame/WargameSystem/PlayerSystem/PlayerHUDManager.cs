using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wargame.PlayerSystem
{
    public class PlayerHUDManager : MonoBehaviour
    {
        public static PlayerHUDManager inst;

        public TextMeshProUGUI ammoText;
        public Image healthUI, healthMotionUI;

        void Awake()
        {
            if (!inst)
            {
                inst = this;
            }
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
    }
}