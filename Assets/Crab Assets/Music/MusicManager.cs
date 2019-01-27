using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip [] audios;
    [SerializeField] AudioSource audioSource;
    AudioClip currentAudio;

    private void Start()
    {
        currentAudio = GetComponent<AudioClip>();
    }

    public void ChangeMusic(int audioToPlay)
    {
         
        if (audioToPlay <= audios.Length)
        {

            currentAudio = audios[audioToPlay];
            audioSource.clip = currentAudio;
            audioSource.Play();
            
        }

     
    }
    
}
