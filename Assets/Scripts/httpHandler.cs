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
    //gameObjects to update on canvas
    public GameObject docImage;
    public TextMeshProUGUI textInfo;
    public TextMeshProUGUI title;

    //variables to save data from request 
    public AudioClip audioClip;
    public Texture texture;
    // instance variables
    private int numberOfSlides;
    private int currentSlide;

    //list of all RootObjects for one pop up canvas
    public List<RootObject> slideList =  new List<RootObject>();



    void Start()
    {
        currentSlide = 0;
        Debug.Log("start starded");
        Debug.Log("textInfo" + textInfo.text);
        
        //StartCoroutine(PostRequest("name", "Hololens"));
    }

    public void postReq(string str1, string str2)
    {
        StartCoroutine(PostRequest(str1, str2));
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
                    Debug.Log("parsed jason");
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
                    Debug.Log("testing" + r.title);
                    //if the first item is done, we can show the view :) 
                    if(i == 0)
                    {
                        currentSlide = 0;
                        ShowSlide(0);
                    }

                    i++;
                }
                Debug.Log("count" + slideList.Count);
                numberOfSlides = slideList.Count;

                //testing updating serialized fields
                    //rawImg.texture = slideList[0].texture;
                    //audioSource.clip = slideList[0].audioClip;
                    //Debug.Log(audioSource.clip + " length: " + audioSource.clip.length);
                    //audioSource.Play();
            }
        }
    }

    /* GetRequest:
     * Gets all items from database with GET
     */
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
        Debug.Log("currentSlide:" + currentSlide);
        Debug.Log("slideNumber:" + slideNumber);
        Debug.Log("testing" + slideList[slideNumber].text);
        Debug.Log("text box available" + textInfo.text);
        textInfo.text = slideList[slideNumber].text;
        title.text = slideList[slideNumber].title;

        //make texture into sprite programmatically
        Texture2D tex = (Texture2D) slideList[slideNumber].texture;
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        docImage.GetComponent<Image>().sprite = sprite;
    }
    void ShowContents()
    {
        Debug.Log("show contents");
        Debug.Log(numberOfSlides);
        string[] titles = new string[numberOfSlides];
        int y = 1;
        for (int x = 0; x < numberOfSlides; x++)
        {
            int counter = 2;
            while (y < numberOfSlides)
            {
                if (slideList[x].title == slideList[y].title)
                {
                    slideList[y].title += " Part " + counter;
                    counter++;
                    Debug.Log(slideList[x].title);
                }
                y++;
            }
            y++;
        }
        for (int x = 0; x < numberOfSlides; x++)
        {
            GameObject button = Instantiate(tableContent, new Vector3(buttonX, buttonStartY - (x * buttonYScale), 0), Quaternion.identity) as GameObject;
            button.name = slideList[x].title;
            Vector3 scale = button.transform.localScale;
            Vector3 pos = button.transform.position;
            button.transform.SetParent(docImage.transform.parent);
            button.transform.localScale = scale;
            button.transform.localPosition = new Vector3(buttonX, buttonStartY - (x * buttonYScale), 0);
            titles[x] = slideList[x].title;
            if (slideList[x].title.Length > 15)
            {
                string text = slideList[x].title.Substring(0, 15) + "...";
                button.GetComponentInChildren<TextMeshProUGUI>().text = text;
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = slideList[x].title;
            }
        }

        this.gameObject.GetComponentInParent<CanvasGaze>().addGestures(titles);
        this.gameObject.GetComponentInParent<SpeechManager>().addSpeech(titles);

    }
    void OnNextPage()
    {
        //check if its the last page. If it is, go back to first page
        if(currentSlide == numberOfSlides-1)
        {
            ShowSlide(0);
        } else
        {
            ShowSlide(currentSlide + 1);
        }  
    }

    void OnPreviousPage()
    {
        //check if its the first page. If it is, do nothing.
        if (currentSlide == numberOfSlides - 1)
        {
         
        }
        else
        {
            ShowSlide(currentSlide - 1);
        }
    }
}

