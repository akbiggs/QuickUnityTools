﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace QuickUnityTools.Sound {

    /// <summary>
    /// A class for quickly importing and playing one-off sounds.
    /// </summary>
    [ResourceSingleton("SoundPlayer")]
    public class SoundPlayer : Singleton<SoundPlayer> {

        public AudioMixerGroup soundMixerGroup;

        private Dictionary<string, AudioClip> loadedSounds = new Dictionary<string, AudioClip>();


        public AudioSource Play(string soundName) {
            if (loadedSounds.ContainsKey(soundName)) {
                return this.Play(this.loadedSounds[soundName]);
            } else {
                AudioClip clip = Resources.Load<AudioClip>(soundName);
                if (clip == null) {
                    Debug.LogWarning("Tried to play sound from string, but failed: " + soundName);
                    return null;
                } else {
                    this.loadedSounds.Add(soundName, clip);
                    return this.Play(clip);
                }
            }
        }

        public AudioSource Play(AudioClip clip, float volume = 1) {
            return this.Play(clip, this.transform.position, volume);
        }

        public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1, float spatialBlend = 0) {
            return this.PlayOneOff(clip, position, volume, spatialBlend);
        }

        public AudioSource PlayLooped(AudioClip clip, Vector3 position) {
            AudioSource source = this.CreateAudioObject(clip, position);
            source.loop = true;
            source.Play();
            return source;
        }

        private AudioSource PlayOneOff(AudioClip clip, Vector3 position, float volume = 1, float spatialBlend = 0) {
            if (clip == null) {
                Debug.LogWarning("Audio clip is not assigned to a value!");
                return null;
            }

            AudioSource audioSource = this.CreateAudioObject(clip, position);
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.outputAudioMixerGroup = this.soundMixerGroup;

            audioSource.Play();
            GameObject.Destroy(audioSource.gameObject, clip.length + 0.1f); // Destroy object after clip duration.

            // If the game is paused, create a timer to handle cleaning up the object...
            if (Time.timeScale == 0) {
                Timer.Register(clip.length + 0.1f, () => {
                    if (audioSource != null) {
                        GameObject.Destroy(audioSource.gameObject);
                    }
                }, false, true);
            }
            return audioSource;
        }

        private AudioSource CreateAudioObject(AudioClip clip, Vector3 position) {
            GameObject soundGameObject = new GameObject("AudioSource (Temp)");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            return audioSource;
        }
    }
}