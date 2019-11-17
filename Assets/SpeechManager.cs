using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer = null;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    // Start is called before the first frame update
    void Start()
    {
        keywords.Add("Help", () =>
        {
            Debug.Log("help called");
            this.BroadcastMessage("OnHelp");
        });
        keywords.Add("Next Page", () =>
        {
            var focusedObject = LocationManager.Instance.FocusedObject;
            Debug.Log("Next page Called");
            if(focusedObject != null)
            {
                focusedObject.SendMessage("OnNextPage", SendMessageOptions.DontRequireReceiver);
                Debug.Log(focusedObject);
            }
        });
        keywords.Add("Previous Page", () =>
        {
            var focusedObject = LocationManager.Instance.FocusedObject;
            Debug.Log("Previous page Called");
            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnPreviousPage", SendMessageOptions.DontRequireReceiver);
                Debug.Log(focusedObject);
            }
        });
        keywords.Add("Open Document", () =>
        {
            var focusedObject = LocationManager.Instance.FocusedObject;
            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnOpenDocument", SendMessageOptions.DontRequireReceiver);
            }
        });
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

    // Update is called once per frame
    void Update()
    {
        
    }
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
