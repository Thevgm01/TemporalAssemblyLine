using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAudioListener : MonoBehaviour
{
    [SerializeField] float duration;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine("PauseAudio");
    }

    // Update is called once per frame
    IEnumerator PauseAudio()
    {
        float volume = AudioListener.volume;
        AudioListener.volume = 0;
        yield return new WaitForSeconds(duration);
        AudioListener.volume = volume;
    }
}
