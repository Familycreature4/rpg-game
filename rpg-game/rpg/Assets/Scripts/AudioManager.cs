using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
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
}
