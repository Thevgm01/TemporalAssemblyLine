using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioHelper
{
    public static AudioSource PlayRandomClipFromArray(AudioClip[] clips, float volume = 1.0f, float pitch = 1.0f, Transform source = null, float maxDistance = 3f, bool loop = false)
    {
        return PlayClip(clips[UnityEngine.Random.Range(0, clips.Length)], volume, pitch, source, maxDistance, loop);
    }
    public static AudioSource PlayClip(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, Transform source = null, float maxDistance = 1f, bool loop = false)
    {
        if (clip == null) return null;

        GameObject audioObject = new GameObject("Audio2D");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        if (source)
        {
            audioObject.transform.position = source.position;
            audioSource.spatialBlend = 1.0f;
            audioSource.maxDistance = maxDistance;
        }
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        if (loop) audioSource.loop = true;
        else Object.Destroy(audioObject, clip.length);
        return audioSource;
    }
}
