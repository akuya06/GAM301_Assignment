using UnityEngine;
using UnityEngine.UI;

public class SettingMenuController : MonoBehaviour
{
    [Header("Sliders (Canvas UI)")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    void Start()
    {
        // Đọc volume từ PlayerPrefs
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        if (musicSlider != null)
        {
            musicSlider.value = musicVol;
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        // Áp dụng giá trị cho SoundManager
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(musicVol);
            SoundManager.Instance.SetSFXVolume(sfxVol);
        }
    }

    void OnDestroy()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
    }

    private void OnMusicSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        PlayerPrefs.Save();
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
    }
}
