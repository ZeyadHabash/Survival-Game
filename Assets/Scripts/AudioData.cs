using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/Audio Data")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;

        [Header("3D Sound Settings")]
        public bool is3D = false;
        [Range(0f, 500f)]
        public float maxDistance = 100f;
    }

    [Header("Music Tracks")]
    public Sound[] musicTracks;

    [Header("Sound Effects")]
    public Sound[] soundEffects;

    [Header("UI Sounds")]
    public Sound[] uiSounds;

    public Sound GetSound(string soundName)
    {
        // Search in all categories
        Sound sound = System.Array.Find(musicTracks, s => s.name == soundName);
        if (sound != null) return sound;

        sound = System.Array.Find(soundEffects, s => s.name == soundName);
        if (sound != null) return sound;

        sound = System.Array.Find(uiSounds, s => s.name == soundName);
        return sound;
    }
}