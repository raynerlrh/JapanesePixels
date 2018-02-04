using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipArray
{
    public AudioClip[] clips;
}

public class AudioPlayer : MonoBehaviour 
{
    private AudioSource soundobj;
    public AudioClip[] tracks;

    [SerializeField]
    AudioClipArray[] japaneseClips;

	void Awake() 
    {
        soundobj = gameObject.AddComponent<AudioSource>();
	}

    public void PlayJap(int _groupIndex, int _questionIndex)
    {
        if (soundobj == null)
            Debug.Log("NULL");
        soundobj.PlayOneShot(japaneseClips[_groupIndex].clips[_questionIndex], 5f);
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
