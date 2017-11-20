using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
    private AudioSource soundobj;
    public AudioClip[] tracks;
	// Use this for initialization
	void Start () {
        soundobj = gameObject.AddComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayOnceTrack(int index, float volumeScalar = 1)
    {
        if (index < 0)
            return;
        soundobj.PlayOneShot(tracks[index], volumeScalar);
    }

    public void PlayTrack(bool changetrack, int index = 0)
    {
        if (!changetrack)
        {
            soundobj.Play();
        }
        else
        {
            if (index < 0)
                return;
            soundobj.clip = tracks[index];
            soundobj.Play();
        }

    }

    public bool isPlayingTrack()
    {
        if (soundobj.isPlaying)
            return true;
        return false;
    }
}
