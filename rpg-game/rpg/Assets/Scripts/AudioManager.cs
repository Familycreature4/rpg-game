using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Current
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AudioManager>();
            }

            if (instance != null)
            {
                return instance;
            }

            return null;
        }
    }
    static AudioManager instance;
    AudioSource audiosource;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        audiosource = GetComponent<AudioSource>();
        AudioClip clip = Resources.Load<AudioClip>("Audio/Music/plexlab");
        audiosource.clip = clip;

    }

    // Update is called once per frame
    void Update()
    {
        if (audiosource.isPlaying == false)
        {
            audiosource.Play();
            Debug.Log("Audio is playing...");
        }
;

    }
    public void PlayOneShot(AudioClip clip, float volumeScale = 1.0f)
    {
        audiosource.PlayOneShot(clip, volumeScale);
    }
}
