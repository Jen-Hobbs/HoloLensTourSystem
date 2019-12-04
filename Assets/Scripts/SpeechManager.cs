using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    private delegate void MyDelegate(int x);
    private static KeywordRecognizer keywordRecognizer = null;

    private static Dictionary<string, System.Action> keywords = null;
    private static string[] currentWords = null;
    
    // Start is called before the first frame update
    /// <summary>
    /// starts adding keywords to dictionary. When keyword is said the System.Action is done to send a message to a function in another script)
    /// </summary>
    void Start()
    {
        restartSpeech();
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
    //public static void ContentCanvas()
    //{

    //    Debug.Log("contentcanvas initialized");
    //    for (int x = 0; x < CanvasContent.titles.Length; x++)
    //    {
    //        keywords.Add("view " + CanvasContent.titles[x], () =>
    //        {
    //            GameObject focusedObject = LocationManager.Instance.FocusedObject;
    //            if (focusedObject != null)
    //            {
    //                focusedObject.SendMessage("OnContent", "Working progress", SendMessageOptions.DontRequireReceiver);
    //                //Debug.Log(CanvasContent.titles[x]);
    //            }
    //        });
    //    }

    //    foreach (string key in keywords.Keys)
    //    {
    //        Debug.Log(key);
    //    }
    //    keywordRecognizer.Dispose();
    //    keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

    //    // Register a callback for the KeywordRecognizer and start recognizing!
    //    keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
    //    keywordRecognizer.Start();
    //    //Debug.Log(CanvasContent.titles.Length);

    //}

    /// <summary>
    /// Adds table of contents buttons to keyword recognizer
    /// </summary>
    /// <param name="buttonName">titles added to keywords</param>
    public void addSpeech(string[] titles)
    {
        keywordRecognizer.Stop();
        for (int x = 0; x < titles.Length; x++)
        {
            keywords.Add(titles[x], () =>
            {
                Debug.Log(titles[x] + "Called by voice");
                //unable to test at this time bad voice recognizer
                this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnChangeContent", titles[x], SendMessageOptions.DontRequireReceiver);
                
            });
        }
        currentWords = keywords.Keys.ToArray();
        for (int x = 0; x < currentWords.Length; x++)
        {
            Debug.Log(currentWords[x]);
        }
        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }
    /// <summary>
    /// restarts keyword recognizer to default keys
    /// </summary>
    public void restartSpeech()
    {
        if(keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywords = null;
            keywordRecognizer.Dispose();
        }
        keywords = new Dictionary<string, System.Action>();
        Debug.Log("keyword recognizer started");
        keywords.Add("Help", () =>
        {
            Debug.Log("help called");
            this.BroadcastMessage("OnHelp");
        });
        keywords.Add("Next Page", () =>
        {
            //var focusedObject = LocationManager.Instance.FocusedObject;
            Debug.Log("Next page Called by voice");
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
            this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnPreviousPage", SendMessageOptions.DontRequireReceiver);
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
            Debug.Log("close document");
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Canvas");
            restartSpeech();
            this.gameObject.GetComponent<CanvasGaze>().restartGestures();
            for (var i = 0; i < gameObjects.Length; i++)
            {
                Destroy(gameObjects[i]);
            }
        });
        string[] keys = keywords.Keys.ToArray();
        for (int x = 0; x < keys.Length; x++)
        {
            Debug.Log(keys[x]);
        }

        // Tell the KeywordRecognizer about our keywords.



        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();


    }
}
