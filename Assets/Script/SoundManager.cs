using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Gun Sounds")]
    public AudioClip[] gunShotClips; // Danh sách tiếng bắn súng
    public AudioClip reloadClip;     // Tiếng nạp đạn
    public AudioClip zombieChasing;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDie;
    public AudioSource zombieAudioSource;
    private AudioSource sfxAudioSource;

    [Header("Music & SFX")]
    public AudioClip backgroundMusic;
    private AudioSource musicAudioSource;

    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Music AudioSource
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.spatialBlend = 0f;

        // SFX AudioSource
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.loop = false;
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.spatialBlend = 0f;
        // Phát nhạc nền nếu có
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    // Phát tiếng bắn súng random
    public void PlayGunShot()
    {
        if (gunShotClips.Length == 0) return;
        int idx = Random.Range(0, gunShotClips.Length);
        sfxAudioSource.PlayOneShot(gunShotClips[idx], sfxVolume);
    }

    // Phát tiếng nạp đạn
    public void PlayReload()
    {
        if (reloadClip == null) return;
        sfxAudioSource.PlayOneShot(reloadClip, sfxVolume);
    }

    // Phát nhạc nền
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;
        if (musicAudioSource.clip == musicClip && musicAudioSource.isPlaying) return;
        musicAudioSource.clip = musicClip;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.Play();
    }

    // Dừng nhạc nền
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    // Chỉnh âm lượng nhạc nền
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = musicVolume;
    }

    // Chỉnh âm lượng SFX
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxAudioSource.volume = sfxVolume;
    }

    // Phát SFX chung
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxAudioSource.PlayOneShot(clip, sfxVolume);
    }
}
