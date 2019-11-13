using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{
    public RawImage img;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownloadImage("https://randomuser.me/api/portraits/med/women/11.jpg"));
        StartCoroutine(PlayAudio("http://techslides.com/demos/samples/sample.wav"));
        // http://en-support.files.wordpress.com/2014/10/istock_audio.wav

        audioSource.loop = true;
        audioSource.Stop();
    }
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            img.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    IEnumerator PlayAudio(string MediaUrl)
    {
        UnityWebRequest music = UnityWebRequestMultimedia.GetAudioClip(MediaUrl, AudioType.WAV);
        yield return music.SendWebRequest();

        if (music.isNetworkError)
        {
            Debug.Log(music.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(music);
            Debug.Log(clip + " length: " + clip.length);
            if (clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

    }
}
