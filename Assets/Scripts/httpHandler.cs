using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;
using System.Collections.Generic;

/* 
 * Note: the QR code will have to be the name attribute right now for it to work
 */

public class HttpHandler : MonoBehaviour
{
    public GameObject docImage;
    public TextMeshProUGUI textInfo;
    public Sprite NextSprite;
    public Sprite PreviousSprite;
    public TextMeshProUGUI title;


    //variables to save data from request 
    public AudioClip audioClip;
    public Texture texture;

    public bool isReady =  false;
    //list of all RootObjects for one pop up canvas
    public List<RootObject> slideList =  new List<RootObject>();

    void Start()
    {
        StartCoroutine(PostRequest("name", "Hololens"));
    }

    void Update()
    {
        
    }

    /* PostRequest:
     * Gets all items from database by name as a json object, saves the data members in RootObjects, and updates slideList list
     */
    public IEnumerator PostRequest(string fieldName1, string fieldValue1)
    {
        WWWForm form = new WWWForm();
        form.AddField(fieldName1, fieldValue1);

        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-hololens-serverless.cloudfunctions.net/getAllItemsMatchingName", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield return null;
            }
            else
            {
                //Debug.Log(":\nReceived: " + www.downloadHandler.text);
                Debug.Log("Form upload complete!");

                //Deserialize JSON 
                var parsedJSON = JSON.Parse(www.downloadHandler.text);
                var id = parsedJSON[3]["id"].Value;

                int i = 0;
                //don't know how to get the total list size so just check if the next entry is an empty string
                //might wanna change json structure later
                while(!(parsedJSON[i]["id"].Value.Equals("")))
                {
                    RootObject r = new RootObject();

                    r.id = parsedJSON[i]["id"].Value;
                    r.name = parsedJSON[i]["name"].Value;
                    r.page = parsedJSON[i]["page"].Value;
                    r.title = parsedJSON[i]["title"].Value;
                    r.category = parsedJSON[i]["category"].Value;
                    r.text = parsedJSON[i]["text"].Value;
                    r.imgurl = parsedJSON[i]["imgurl"].Value;
                    r.audiourl = parsedJSON[i]["audiourl"].Value;

                    //wait for images and audio to download
                    yield return StartCoroutine(DownloadImage(r.imgurl, value => r.texture = value));
                    yield return StartCoroutine(DownloadAudio(r.audiourl, value => r.audioClip = value));

                    r.texture = texture;
                    r.audioClip = audioClip;
                    Debug.Log(r.id);
                    slideList.Add(r);

                    //can probably update the view here?
                    if(i == 0)
                    {
                        ShowSlide(0);
                        //GetComponent<CanvasContent>().help();

                        //update the first page if it is ready
                    }

                    i++;
                }

                //testing updating serialized fields
                //rawImg.texture = slideList[0].texture;
                //audioSource.clip = slideList[0].audioClip;
                //Debug.Log(audioSource.clip + " length: " + audioSource.clip.length);
                //audioSource.Play();
            }
        }
    }

    public IEnumerator GetRequest(string uri)
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

    /* 
     * Downloads the image from url to texture
     */
    public IEnumerator DownloadImage(string MediaUrl, System.Action<Texture2D> result)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    /* 
     * Downloads the audio from url to audioClip
     */
    public IEnumerator DownloadAudio(string MediaUrl, System.Action<AudioClip> result)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(MediaUrl, AudioType.WAV);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            //Debug.Log(clip + " length: " + clip.length);
            if (clip)
            {
                audioClip = clip;
                //audioSource.Play();
            }
        }
    }

    public void ShowSlide(int slideNumber)
    {
        textInfo.text = slideList[slideNumber].text;
        title.text = slideList[slideNumber].title;
    }

    public void OnNextPage(int currentSlideNumber)
    {
        ShowSlide(currentSlideNumber + 1);
    }

    public void OnPreviousPage(int currentSlideNumber)
    {
        ShowSlide(currentSlideNumber - 1);
    }
}

