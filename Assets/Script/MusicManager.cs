using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class MusicManager : MonoBehaviour {

	public static MusicManager instance;

	public Sound[] sounds;
	
	void Awake ()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		} else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
	}

	public void Play(string sound)
	{
		foreach (Sound ss in sounds)
		{
			ss.source.Stop();
		}
		
		Sound s = Array.Find(sounds, item => item.name == sound);
		s.source.clip = s.clip;
		s.source.volume = s.volume;
		s.source.pitch = s.pitch;
		s.source.loop = s.loop;
		s.source.Play();
	}
	
	

	public void PlayOneshot(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		s.source.PlayOneShot(s.clip);
	}
	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		s.source.Stop();
	}
	public void StopAll(){
		foreach (Sound ss in sounds)
		{
			ss.source.Stop();
		}
	}

	public IEnumerator fading(string sound, float fade_sec){
		Sound s = Array.Find(sounds, item => item.name == sound);
		DOTween.To(() => s.source.volume, x => s.source.volume = x, 0f, fade_sec);
		yield return new WaitForSeconds(fade_sec);
		//s.source.Stop();
		//yield return new WaitForSeconds(0.0f);
	}
	public IEnumerator fadin(string sound, float fade_sec){
		Sound s = Array.Find(sounds, item => item.name == sound);
		DOTween.To(() => s.source.volume, x => s.source.volume = x, s.volume, fade_sec);
		yield return new WaitForSeconds(fade_sec);
		//s.source.Stop();
		//yield return new WaitForSeconds(0.0f);
	}

}
