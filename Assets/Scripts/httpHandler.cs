using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

/* Note: the QR code will have to be the name attribute
 * 
 */
public class httpHandler : MonoBehaviour
{
    //list of all RootObjects for one pop up canvas
    ArrayList slideList = new ArrayList();

    public 
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetRequest("https://us-central1-hololens-serverless.cloudfunctions.net/getAllItems"));
        StartCoroutine(PostRequest("name", "Hololens"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* PostRequest:
     * Gets all items from database by name as a json object, saves the data members in RootObjects, and updates slideList
     */
    IEnumerator PostRequest(string fieldName1, string fieldValue1)
    {
        WWWForm form = new WWWForm();
        form.AddField(fieldName1, fieldValue1);

        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-hololens-serverless.cloudfunctions.net/getAllItemsMatchingName", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(":\nReceived: " + www.downloadHandler.text);
                Debug.Log("Form upload complete!");

                //Deserialize JSON 
                var parsedJSON = JSON.Parse(www.downloadHandler.text);
                var id = parsedJSON[3]["id"].Value;

                int i = 0;
                //don't know how to get the total size so just check if the next entry is an empty string
                while(!(parsedJSON[i]["id"].Value.Equals("")))
                {
                    RootObject r = new RootObject();
                    RawImage rawImg = new RawImage();
                    AudioSource audioSource = new AudioSource();

                    r.id = parsedJSON[i]["id"].Value;
                    r.name = parsedJSON[i]["name"].Value;
                    r.page = parsedJSON[i]["page"].Value;
                    r.title = parsedJSON[i]["title"].Value;
                    r.category = parsedJSON[i]["category"].Value;
                    r.text = parsedJSON[i]["text"].Value;
                    r.imgurl = parsedJSON[i]["imgurl"].Value;
                    r.audiourl = parsedJSON[i]["audiourl"].Value;


                    Debug.Log(r.id);
                    slideList.Add(r);

                    i++;
                }

                
                

            }
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);

            }
        }
    }

        IEnumerator DownloadImage(string MediaUrl, ref RawImage img)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            img.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    IEnumerator PlayAudio(string MediaUrl, ref )
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

