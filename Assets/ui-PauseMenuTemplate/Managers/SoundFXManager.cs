using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
     public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign the AudioClip
        audioSource.clip = audioClip;

        //Assign volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //Destroy clip after playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //Assign Random Index
        int rand = Random.Range(0, audioClip.Length);

        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign the AudioClip
        audioSource.clip = audioClip[rand];

        //Assign volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //Destroy clip after playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
