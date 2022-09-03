using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource bgm;
    public AudioSource sfx;
    public AudioSource menuSfx;
    public float[] spectrumWidth;

    [SerializeField] private AudioClip menuBGMClip;
    [SerializeField] private AudioClip levelBGMClip;


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
    }

    private void Start() {
        spectrumWidth = new float[64];
    }

    private void FixedUpdate() 
    {
        bgm.GetSpectrumData(spectrumWidth, 0, FFTWindow.Blackman);
    }

#region Player
	    public void PlaySFX(AudioClip clip) => sfx.PlayOneShot(clip);
	
	    public void PlayBGM(AudioClip clip)
	    {
	        bgm.clip = clip;
	        bgm.Play();
	    }
	
	    public void PlayMenuSFX(AudioClip clip) => menuSfx.PlayOneShot(clip);
	
	    public void PlayMenuSFXWithDelay(AudioClip clip, float delay)
	    {
	        menuSfx.clip = clip;
	        menuSfx.PlayDelayed(delay);
	    }
	
	    public void StopBGM() => bgm.Stop();
	
	    public void PauseBGM() => bgm.Pause();
#endregion

#region Volume
	    public void SetSfxVolume(float val) => sfx.volume = val;
	
	    public void SetBgmVolume(float val) => bgm.volume = val;
	
	    public float GetBGMVolume() => bgm.volume;
	
	    public float GetSFXVolume() => sfx.volume;
	
	    public float GetMasterVolume() => AudioListener.volume;
	
	    public void AdjustMasterVolume(float vol) => AudioListener.volume = vol;
#endregion

#region Other
	    public void PlayMenuBGM()
	    {
	        bgm.clip = menuBGMClip;
	        bgm.Play();
	    }
	
	    public void PlayLevelBGM()
	    {
	        bgm.clip = levelBGMClip;
	        bgm.Play();
	    }
#endregion

    //Tutorial: https://www.youtube.com/watch?v=w0OVIUZXHzY
    private float GetFrequenciesDiapason(int start, int end, int mult)
    {
        return spectrumWidth.ToList().GetRange(start, end).Average() * mult;
    }

    public float GetBaseFrequencyDiapason()
    {
        return GetFrequenciesDiapason(0, 7, 10);
    }
    public float GetNBFrequencyDiapason()
    {
        return GetFrequenciesDiapason(7, 15, 100);
    }
    public float GetMiddlesFrequencyDiapason()
    {
        return GetFrequenciesDiapason(15, 30, 200);
    }
    public float GetHighsFrequencyDiapason()
    {
        return GetFrequenciesDiapason(30, 32, 1000);
    }

}
