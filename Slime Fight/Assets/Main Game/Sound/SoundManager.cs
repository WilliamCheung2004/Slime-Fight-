using UnityEngine;
using UnityEngine.UI;
using System;

public enum SoundType
{
    PLAYER,
    SWORD,
    BOW,
    BACKGROUND,
    ENEMY1,
    ENEMY2,
    BOSS
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;

    private static float globalEffectsSound = 1;
    private static float globalBackgroundSound = 1;

    private AudioSource loopAudioSource;

    [Header("Volume Settings")]
    [SerializeField] private Slider[] effectsSliders;
    [SerializeField] private Slider[] backgroundSliders;

    private static AudioSource[] sfxSources;
    private const int SFX_SOURCE_COUNT = 6;

    private void Awake()
    {
        instance = this;

        AudioSource[] sources = GetComponents<AudioSource>();

        loopAudioSource = sources[1];

        sfxSources = new AudioSource[SFX_SOURCE_COUNT];

        for (int i = 0; i < SFX_SOURCE_COUNT; i++)
        {
            sfxSources[i] = sources[i];
            sfxSources[i].playOnAwake = false;
            sfxSources[i].loop = false;
        }
    }

    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        PlayOnFreeSource(clip, volume);
    }


    public static void SelectSound(SoundType sound, int index, float volume = 1f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;

        if (clips == null || clips.Length == 0) return;
        if (index < 0 || index >= clips.Length) return;

        AudioClip clip = clips[index];

        PlayOnFreeSource(clip, volume * globalEffectsSound);
    }


    public static void PlayFootstep(SoundType sound, float volume = 1f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        PlayOnFreeSource(clip, volume * globalEffectsSound);
    }


    public static void PlayLoopSound(SoundType sound, float volume = 1f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) return;

        instance.loopAudioSource.clip = clips[0];
        instance.loopAudioSource.volume = volume * globalBackgroundSound;
        instance.loopAudioSource.loop = true;
        instance.loopAudioSource.Play();
    }

    public static void StopLoopSound()
    {
        if (instance.loopAudioSource.isPlaying)
            instance.loopAudioSource.Stop();
    }


    private static void PlayOnFreeSource(AudioClip clip, float volume)
    {
        AudioSource freeSource = null;

        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (!sfxSources[i].isPlaying)
            {
                freeSource = sfxSources[i];
                break;
            }
        }

        if (freeSource == null)
            freeSource = sfxSources[0];

        freeSource.clip = clip;
        freeSource.volume = volume * globalEffectsSound;
        freeSource.Play();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);

        for (int i = 0; i < soundList.Length; i++)
            soundList[i].name = names[i];
    }

    public void ChangeEffectsVolume(Slider changedSlider)
    {
        globalEffectsSound = changedSlider.value;
        PlayerPrefs.SetFloat("EffectsVolume", globalEffectsSound);
    }

    public void ChangeBackgroundVolume(Slider changedSlider)
    {
        globalBackgroundSound = changedSlider.value;
        PlayerPrefs.SetFloat("BackgroundVolume", globalBackgroundSound);

        if (instance.loopAudioSource.isPlaying)
            instance.loopAudioSource.volume = globalBackgroundSound;
    }

    public void InitialiseEffectsSlider(Slider current)
    {
        if(PlayerPrefs.HasKey("EffectsVolume"))
        {
            current.value = PlayerPrefs.GetFloat("EffectsVolume");
            globalEffectsSound = current.value;
        }   
    }

    public void InitialiseBackgroundSlider(Slider current)
    {
        if (PlayerPrefs.HasKey("BackgroundVolume"))
        {
            current.value = PlayerPrefs.GetFloat("BackgroundVolume");
            globalBackgroundSound = current.value;
        }
    }




#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds => sounds;

    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}