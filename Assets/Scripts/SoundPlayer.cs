using System.Reflection;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Tooltip("Объект, издающий звук.")]    public AudioSource cameraAudioSource;
    [Tooltip(".mp3 дорожна звука.")]        public AudioClip soundClip;
    [Tooltip("Громкость.")]                 [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Зацикливание звука.")]        public bool loop = false;
    [Tooltip("Скорость воспроизведения.")]  public float pitch = 1f;


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