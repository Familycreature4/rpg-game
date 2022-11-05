using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Audio Set", menuName ="Audio/Audio Set")]
public class AudioSet : ScriptableObject
{
    public static AudioClip GetRandomClip(string setName)
    {
        AudioSet set = Resources.Load<AudioSet>($"AudioSets/{setName}");
        if (set != null && set.clips != null && set.clips.Length > 0)
        {
            return set.clips[Random.Range(0, set.clips.Length)];
        }

        return null;
    }

    public AudioClip[] clips;
}
