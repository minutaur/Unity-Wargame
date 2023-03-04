using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WargameSystem
{
    public class GameUIManager : MonoBehaviour
    {
        public List<TextMeshProUGUI> teamLife;
        public TextMeshProUGUI timeoutText;

        public void UpdateLife(int team, int amount)
        {
            teamLife[team].text = amount.ToString();
        }
        public void UpdateTime(int time)
        {
            int minute = time / 60;
            int second = time % 60;
            timeoutText.text = minute.ToString("D2") + ":" + second.ToString("D2");
        }
    }
}