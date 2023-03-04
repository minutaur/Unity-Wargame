using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Wargame.MenuSystem
{
    public class SettingsUIManager : MonoBehaviour
    {
        public AudioMixer masterMixer;
        
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetVolume(float volume)
        {
            masterMixer.SetFloat("Volume", volume);
        }
    }
}