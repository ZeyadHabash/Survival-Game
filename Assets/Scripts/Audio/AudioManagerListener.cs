using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class AudioManagerListener : MonoBehaviour
{
    [SerializeField] private AudioEventChannel audioEventChannel;
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }

    void OnEnable()
    {
        if (audioEventChannel != null)
        {
            audioEventChannel.RegisterPlaySound(OnPlaySound);
            audioEventChannel.RegisterPlaySound3D(OnPlaySound3D);
            audioEventChannel.RegisterPlayMusic(OnPlayMusic);
            audioEventChannel.RegisterStopMusic(OnStopMusic);
            audioEventChannel.RegisterStopSound(OnStopSound);
            audioEventChannel.RegisterStopAllSounds(OnStopAllSounds);
        }
    }

    void OnDisable()
    {
        if (audioEventChannel != null)
        {
            audioEventChannel.UnregisterPlaySound(OnPlaySound);
            audioEventChannel.UnregisterPlaySound3D(OnPlaySound3D);
            audioEventChannel.UnregisterPlayMusic(OnPlayMusic);
            audioEventChannel.UnregisterStopMusic(OnStopMusic);
            audioEventChannel.UnregisterStopSound(OnStopSound);
            audioEventChannel.UnregisterStopAllSounds(OnStopAllSounds);
        }
    }

    private void OnPlaySound(string soundName)
    {
        audioManager.PlaySound(soundName);
    }

    private void OnPlaySound3D(string soundName, Vector3 position)
    {
        audioManager.PlaySound(soundName, position);
    }

    private void OnPlayMusic(string musicName)
    {
        audioManager.PlayMusic(musicName);
    }

    private void OnStopMusic()
    {
        audioManager.StopMusic();
    }

    private void OnStopSound(string soundName)
    {
        audioManager.StopSound(soundName);
    }

    private void OnStopAllSounds()
    {
        audioManager.StopAllSounds();
    }
}