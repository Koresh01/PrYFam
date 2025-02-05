using System.Reflection;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource cameraAudioSource;   // Ссылка на AudioSource на камере
    public AudioClip soundClip;            // Ссылка на аудиофайл
    [Range(0f, 1f)] public float volume = 1f;   // Громкость (ползунок в инспекторе)
    public bool loop = false;              // Зацикливание звука
    public float pitch = 1f;               // Скорость воспроизведения


    void OnEnable()
    {
        cameraAudioSource = FindAnyObjectByType<AudioSource>();
    }
    // Метод для воспроизведения звука
    public void PlaySound()
    {
        cameraAudioSource.clip = soundClip;
        cameraAudioSource.volume = volume;
        cameraAudioSource.loop = loop;
        cameraAudioSource.pitch = pitch;
        cameraAudioSource.Play();
    }
}