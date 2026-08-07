using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text textField;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (slider == null)
            {
                Debug.Log("Slider didnt set");
                return;
            }
            if (textField == null)
            {
                Debug.Log("TextField didnt set");
                return;
            }
            
            int volume = SaveData.Volume;
            slider.value = volume;
            textField.SetText($"{volume}");
        }

        public void HandleSliderValueChanged(float value)
        {
            int intValue = (int)value;
            textField.SetText($"{intValue}");
            SaveData.Volume = intValue;
        }
    }
}