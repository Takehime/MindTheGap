using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioManager Get_Audio_Manager() {
		return GameObject.FindGameObjectWithTag("AudioManager").GetComponentInChildren<AudioManager>();
	}

	public AudioSource source;
	public AudioSource bus_background_source;

	[Header("Intro Scene")]
	public AudioClip metal_pesado;
	public AudioClip city_noises;
	public AudioClip shake_f_sound;

	[Header("Bus Scene")]
	public AudioClip passenger_out;
	public AudioClip passenger_in;
	public AudioClip bus_stopping;
	public AudioClip bus_stopping_fast;
	public AudioClip pachinko_button;
	public AudioClip pachinko_confirmation;
	public AudioClip pachinko_cancel;
	public AudioClip scan_enter;
	public AudioClip scan_leave;
	public AudioClip libera_me;
	public AudioClip enya_time;

	public void Play(AudioClip clip, float volume) {
		source.PlayOneShot(clip, volume);
	}

	public void Play_Real(AudioClip clip) {
		source.clip = clip;
		source.Play();
	}

	public IEnumerator Fade_Out(float fadeTime) {
        float startVolume = source.volume;
 
        while (source.volume > 0) {
            source.volume -= startVolume * Time.deltaTime / fadeTime;
 
            yield return null;
        }
 
        source.Stop ();
        source.volume = startVolume;
	}
}