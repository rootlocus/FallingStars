using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource bgm;
    public AudioSource sfx;
    public AudioSource menuSfx;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;


    private void Awake() {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one Audio Manager " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        bgm = transform.Find("BGM").GetComponent<AudioSource>();
        sfx = transform.Find("SFX").GetComponent<AudioSource>();
        menuSfx = transform.Find("MenuSFX").GetComponent<AudioSource>();
        // bgmSlider.value = bgm.volume;
        // sfxSlider.value = sfx.volume;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        bgm.clip = clip;
        bgm.Play();
    }

    public void PlayMenuSFX(AudioClip clip)
    {
        menuSfx.PlayOneShot(clip);
    }

    
    public void PlayMenuSFXWithDelay(AudioClip clip, float delay)
    {
        menuSfx.clip = clip;
        menuSfx.PlayDelayed(delay);
    }

    public void StopBGM()
    {
        bgm.Stop();
    }

    public void PauseBGM()
    {
        bgm.Pause();
    }

    public void ChangeBGMVolume()
    {
        bgm.volume = bgmSlider.value;
    }

    public void ChangeSFXVolume()
    {
        sfx.volume = sfxSlider.value;
    }
}
