using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
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

        float value = slider.value;
        textField.SetText(value.ToString("F2"));
    }

    // Update is called once per frame
    public void HandleSliderValueChanged(float value)
    {
        textField.SetText(value.ToString("F2"));
    }
}