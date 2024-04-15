using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    private AudioSource audioSource;

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
        bgmStatus = BGMStatus.Playing;
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

    public void PlayAudioClip(AudioClip clip, float volume = 1) {
        audioSource.PlayOneShot(clip, volume);
    }
}
