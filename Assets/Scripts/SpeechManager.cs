using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    private delegate void MyDelegate(int x);
    private static KeywordRecognizer keywordRecognizer = null;
    private static Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    
    // Start is called before the first frame update
    /// <summary>
    /// starts adding keywords to dictionary. When keyword is said the System.Action is done to send a message to a function in another script)
    /// </summary>
    void Start()
    {
        
        keywords.Add("Help", () =>
        {
            Debug.Log("help called");
            this.BroadcastMessage("OnHelp");
        });
        keywords.Add("Next Page", () =>
        {
            //var focusedObject = LocationManager.Instance.FocusedObject;
            Debug.Log("Next page Called");
            //if(focusedObject != null)
            //{
            this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnNextPage", SendMessageOptions.DontRequireReceiver);
            //focusedObject.SendMessage("OnNextPage", SendMessageOptions.DontRequireReceiver);
                //Debug.Log(focusedObject);
            //}
        });
        keywords.Add("Previous Page", () =>
        {
            //var focusedObject = LocationManager.Instance.FocusedObject;
            Debug.Log("Previous page Called");
            this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnNextPage", SendMessageOptions.DontRequireReceiver);
            //if (focusedObject != null)
            //{
            //focusedObject.SendMessage("OnPreviousPage", SendMessageOptions.DontRequireReceiver);
            //Debug.Log(focusedObject);
            //}
        });
        //keywords.Add("Open Document", () =>
        //{
        //    var focusedObject = LocationManager.Instance.FocusedObject;
        //    if (focusedObject != null)
        //    {
        //        Debug.Log("open document said");
        //        focusedObject.SendMessage("OnOpenDocument", SendMessageOptions.DontRequireReceiver);
        //    }
        //});
        keywords.Add("Close Document", () =>
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("interactible");

            for (var i = 0; i < gameObjects.Length; i++)
            {
                Destroy(gameObjects[i]);
            }
        });
        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }
    void test(int x)
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// invokes keywords (used from https://docs.microsoft.com/en-us/windows/mixed-reality/holograms-101)
    /// </summary>
    /// <param name="args"></param>
    private static void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
    //dont forget to delete words when done with canvas
    /// <summary>
    /// Yet to be implemented but should create words based of the title of each page to go through pages
    /// </summary>
    public static void ContentCanvas()
    {

        Debug.Log("contentcanvas initialized");
        for (int x = 0; x < CanvasContent.titles.Length; x++)
        {
            keywords.Add("view " + CanvasContent.titles[x], () =>
            {
                GameObject focusedObject = LocationManager.Instance.FocusedObject;
                if (focusedObject != null)
                {
                    focusedObject.SendMessage("OnContent", "Working progress" , SendMessageOptions.DontRequireReceiver);
                    //Debug.Log(CanvasContent.titles[x]);
                }
            });
        }
        
        foreach (string key in keywords.Keys)
        {
            Debug.Log(key);
        }
        keywordRecognizer.Dispose();
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
        //Debug.Log(CanvasContent.titles.Length);

    }
}
