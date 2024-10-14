using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private float sliderMultiplier = 50;

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderText;

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmSliderText;

    [Header("Toggle")]
    [SerializeField] private Toggle friendlyFireToggle;

    private void Start() {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        bgmSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
    }

    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMultiplier;
        AudioManager.instance.mixer.SetFloat("sfx", newValue);
        PlayerPrefs.SetFloat("SFXVolume", newValue);

    }

    public void BGMSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMultiplier;
        AudioManager.instance.mixer.SetFloat("bgm", newValue);
        PlayerPrefs.SetFloat("MusicVolume", newValue);
    }

    public void OnFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        GameManager.instance.friendlyFire = !friendlyFire;
    }

    public void LoadSettings()
    {
        AudioManager.instance.mixer.GetFloat("sfx",  out float f1);
        sfxSlider.value = f1;
        AudioManager.instance.mixer.GetFloat("bgm",  out float f2);
        bgmSlider.value = f2;

        int friendlyFireInt = PlayerPrefs.GetInt("FriendlyFire", 0);
        bool newFriendlyFire = false;

        if (friendlyFireInt == 1)
            newFriendlyFire = true;
        
        friendlyFireToggle.isOn = newFriendlyFire;  
    }

    private void OnDisable()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        int friendlyFireInt = friendlyFire ? 1 : 0;

        PlayerPrefs.SetInt("FriendlyFire", friendlyFireInt);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", bgmSlider.value);
    }
}
