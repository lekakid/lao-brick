using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public AudioSource BGMSource;
    public AudioSource SFXSource;
    public AudioClip[] BGMClips;
    public AudioClip[] SFXClips;
    public Slider BGMSlider;
    public Slider SFXSlider;

    Dictionary<string, AudioClip> _bgmDictionary;
    Dictionary<string, AudioClip> _sfxDictionary;

    private void Awake() {
        _bgmDictionary = new Dictionary<string, AudioClip>();
        _sfxDictionary = new Dictionary<string, AudioClip>();

        foreach(AudioClip clip in BGMClips) {
            _bgmDictionary.Add(clip.name, clip);
        }

        foreach(AudioClip clip in SFXClips) {
            _sfxDictionary.Add(clip.name, clip);
        }
    }

    private void Start() {
        float bgm = PlayerPrefs.GetFloat("BGM", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFX", 0.5f);
        
        BGMSlider.value = bgm;
        SFXSlider.value = sfx;
    }

    public void SetBGMVolume(float value) {
        AudioMixer.SetFloat("BGM", Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat("BGM", value);
    }

    public void SetSFXVolume(float value) {
        AudioMixer.SetFloat("SFX", Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat("SFX", value);
    }

    public void PlayBGM(string name) {
        AudioClip clip;

        if(_bgmDictionary.TryGetValue(name, out clip)) {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
    }

    public void StopBGM() {
        BGMSource.Stop();
    }

    public void PlaySFX(string name) {
        AudioClip clip;

        if(_sfxDictionary.TryGetValue(name, out clip)) {
            SFXSource.PlayOneShot(clip);
        }
    }
}
