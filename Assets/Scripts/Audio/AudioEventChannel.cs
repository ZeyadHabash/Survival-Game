using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AudioEventChannel", menuName = "Audio/Audio Event Channel")]
public class AudioEventChannel : ScriptableObject
{
    private UnityAction<string> onPlaySound;
    private UnityAction<string, Vector3> onPlaySound3D;
    private UnityAction<string> onPlayMusic;
    private UnityAction onStopMusic;
    private UnityAction<string> onStopSound;
    private UnityAction onStopAllSounds;

    public void PlaySound(string soundName)
    {
        onPlaySound?.Invoke(soundName);
    }

    public void PlaySound3D(string soundName, Vector3 position)
    {
        onPlaySound3D?.Invoke(soundName, position);
    }

    public void PlayMusic(string musicName)
    {
        onPlayMusic?.Invoke(musicName);
    }

    public void StopMusic()
    {
        onStopMusic?.Invoke();
    }

    public void StopSound(string soundName)
    {
        onStopSound?.Invoke(soundName);
    }

    public void StopAllSounds()
    {
        onStopAllSounds?.Invoke();
    }

    // Listener registration
    public void RegisterPlaySound(UnityAction<string> action)
    {
        onPlaySound += action;
    }

    public void UnregisterPlaySound(UnityAction<string> action)
    {
        onPlaySound -= action;
    }

    public void RegisterPlaySound3D(UnityAction<string, Vector3> action)
    {
        onPlaySound3D += action;
    }

    public void UnregisterPlaySound3D(UnityAction<string, Vector3> action)
    {
        onPlaySound3D -= action;
    }

    public void RegisterPlayMusic(UnityAction<string> action)
    {
        onPlayMusic += action;
    }

    public void UnregisterPlayMusic(UnityAction<string> action)
    {
        onPlayMusic -= action;
    }

    public void RegisterStopMusic(UnityAction action)
    {
        onStopMusic += action;
    }

    public void UnregisterStopMusic(UnityAction action)
    {
        onStopMusic -= action;
    }

    public void RegisterStopSound(UnityAction<string> action)
    {
        onStopSound += action;
    }

    public void UnregisterStopSound(UnityAction<string> action)
    {
        onStopSound -= action;
    }

    public void RegisterStopAllSounds(UnityAction action)
    {
        onStopAllSounds += action;
    }

    public void UnregisterStopAllSounds(UnityAction action)
    {
        onStopAllSounds -= action;
    }
}