using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioData audioData;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    [Header("Audio Source Pool")]
    [SerializeField] private int poolSize = 10;

    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private AudioSource musicSource;
    private Dictionary<string, AudioSource> loopingSounds = new Dictionary<string, AudioSource>();

    void Awake()
    {
        InitializeAudioSources();
    }

    void InitializeAudioSources()
    {
        // Create dedicated music source
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixerGroup;
        musicSource.playOnAwake = false;

        // Create pool of audio sources for SFX
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"AudioSource_{i}");
            obj.transform.SetParent(transform);
            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSourcePool.Add(source);
        }
    }

    public void PlayMusic(string musicName)
    {
        AudioData.Sound sound = audioData.GetSound(musicName);
        if (sound == null)
        {
            Debug.LogWarning($"Music '{musicName}' not found!");
            return;
        }

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.pitch = sound.pitch;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySound(string soundName, Vector3? position = null)
    {
        AudioData.Sound sound = audioData.GetSound(soundName);
        if (sound == null)
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
            return;
        }

        // Check if this sound is already playing
        // This prevents the "machine gun" effect or redundant layering
        if (IsClipPlaying(sound.clip))
        {
            return; // Exit if the sound is already active
        }

        AudioSource source = GetAvailableAudioSource();
        if (source == null)
        {
            Debug.LogWarning("No available audio sources in pool!");
            return;
        }

        ConfigureAudioSource(source, sound);

        // Determine which mixer group to use
        if (System.Array.Exists(audioData.uiSounds, s => s.name == soundName))
        {
            source.outputAudioMixerGroup = uiMixerGroup;
        }
        else
        {
            source.outputAudioMixerGroup = sfxMixerGroup;
        }

        if (position.HasValue && sound.is3D)
        {
            source.transform.position = position.Value;
        }

        source.Play();

        if (sound.loop)
        {
            loopingSounds[soundName] = source;
        }
    }

    private bool IsClipPlaying(AudioClip clip)
    {
        // Loop through the pool to see if any active source is playing this specific clip
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying && source.clip == clip)
            {
                return true;
            }
        }
        return false;
    }
    public void StopSound(string soundName)
    {
        if (loopingSounds.ContainsKey(soundName))
        {
            loopingSounds[soundName].Stop();
            loopingSounds.Remove(soundName);
        }
    }

    public void StopAllSounds()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            source.Stop();
        }
        loopingSounds.Clear();
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null;
    }

    private void ConfigureAudioSource(AudioSource source, AudioData.Sound sound)
    {
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;

        if (sound.is3D)
        {
            source.spatialBlend = 1f;
            source.maxDistance = sound.maxDistance;
            source.rolloffMode = AudioRolloffMode.Linear;
        }
        else
        {
            source.spatialBlend = 0f;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetUIVolume(float volume)
    {
        uiMixerGroup.audioMixer.SetFloat("UIVolume", Mathf.Log10(volume) * 20);
    }
}