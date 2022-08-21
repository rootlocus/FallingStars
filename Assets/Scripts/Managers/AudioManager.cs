using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource bgm;
    public AudioSource sfx;
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
        // bgmSlider.value = bgm.volume;
        // sfxSlider.value = sfx.volume;
    }

    public void PlaySFX(AudioClip _clip)
    {
        sfx.PlayOneShot(_clip);
    }

    public void PlayBGM(AudioClip _clip)
    {
        bgm.PlayOneShot(_clip);
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
