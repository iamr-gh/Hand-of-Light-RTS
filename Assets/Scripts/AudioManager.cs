using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    private AudioSource audioSource;
    private AudioSource dialogueSource;

    private enum BGMStatus {
        Playing,
        Paused,
        Stopped,
    }

    private BGMStatus bgmStatus;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        TryGetComponent(out audioSource);
        transform.GetChild(0).TryGetComponent(out dialogueSource);
        bgmStatus = BGMStatus.Playing;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        dialogueSource.Stop();
    }

    private void Update() {
        if (Camera.main != null) {
            transform.position = Camera.main.transform.position;
        }
    }

    public void SetBGMVolume(float volume) {
        audioSource.volume = volume;
    }

    public void PauseBGM() {
        if (bgmStatus == BGMStatus.Playing) {
            audioSource.Pause();
            bgmStatus = BGMStatus.Paused;
        }
    }

    public void PlayBGM() {
        if (bgmStatus == BGMStatus.Paused) {
            audioSource.UnPause();
            bgmStatus = BGMStatus.Playing;
        } else if (bgmStatus == BGMStatus.Stopped) {
            audioSource.Play();
            bgmStatus = BGMStatus.Playing;
        }
    }

    public void StopBGM() {
        if (bgmStatus != BGMStatus.Stopped) {
            audioSource.Stop();
            bgmStatus = BGMStatus.Stopped;
        }
    }

    public void ChangeBGMClip(AudioClip clip) {
        if (bgmStatus == BGMStatus.Stopped) {
            audioSource.clip = clip;
        }
    }

    public bool IsBGMPlaying() {
        return bgmStatus == BGMStatus.Playing;
    }

    public void PlayAudioClip(AudioClip clip, float volume = 0.05f) {
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayDialogue(AudioClip clip, float volume = 1f) {
        dialogueSource.Stop();
        dialogueSource.volume = volume;
        dialogueSource.clip = clip;
        dialogueSource.Play();
    }

    public void StopDialogue() {
        dialogueSource.Stop();
    }
}
