using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioClip[] playlist;
    private AudioSource audioSource;
    public event Action<float> OnVolumeChanged; // ← NUEVO
    private List<int> history = new List<int>();
    private int currentIndex = -1;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = 0.1f;
    }

    void Start()
    {
        PlayRandom();
    }

    void PlayRandom()
    {
        if (playlist.Length == 0) return;

        int next;
        do { next = Random.Range(0, playlist.Length); }
        while (playlist.Length > 1 && next == currentIndex);

        currentIndex = next;
        history.Add(currentIndex);
        isPaused = false;
        Play(currentIndex);
    }

    void Play(int index)
    {
        audioSource.clip = playlist[index];
        audioSource.Play();
    }

    public void NextSong()    => PlayRandom();
    public void PreviousSong()
    {
        if (history.Count < 2) return;
        history.RemoveAt(history.Count - 1);
        currentIndex = history[history.Count - 1];
        isPaused = false;
        Play(currentIndex);
    }

    public void TogglePause()
    {
        if (isPaused) { audioSource.UnPause(); isPaused = false; }
        else          { audioSource.Pause();   isPaused = true;  }
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    public string GetCurrentSongName()
    {
        return audioSource.clip != null ? audioSource.clip.name : "";
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        OnVolumeChanged?.Invoke(volume);
    }

    public float GetVolume() => audioSource.volume;
    public bool IsPaused()   => isPaused;
    public bool HasPrevious() => history.Count >= 2;
}