using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    private AudioSource _audio = null;

    private object _lock = new object();
    
    // prevent non-singleton instantiation by protecting constructor
    protected AudioManager() { }

    // play given clip from audio source
    public void PlayAudioClip(AudioClip clip) {
        // dont want to accidentally create a bunch of audio sources if multithreading later
        lock (_lock) {
            if (!Instance._audio) {
                Instance._audio = Instance.gameObject.AddComponent<AudioSource>();
                Instance._audio.playOnAwake = false;
                Instance._audio.volume = 0.5f;
            }
        }

        Instance._audio.PlayOneShot(clip);
    }
}