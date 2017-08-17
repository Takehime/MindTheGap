using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioManager Get_Audio_Manager() {
		return GameObject.FindGameObjectWithTag("AudioManager").GetComponentInChildren<AudioManager>();
	}

	public AudioSource source;
	public AudioClip passenger_out;
	public AudioClip passenger_in;
	public AudioClip bus_stopping;
	public AudioClip pachinko_button;
	public AudioClip pachinko_confirmation;
	public AudioClip pachinko_cancel;
	public AudioClip scan_enter;
	public AudioClip scan_leave;

	public void Play(AudioClip clip, float volume) {
		source.PlayOneShot(clip, volume);
	}
}