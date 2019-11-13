using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    private KeywordRecognizer recognizeWord = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    // Start is called before the first frame update
    void Start()
    {
        keywords.Add("Next Page", () =>
        {
             //interact = CanvasGaze.Instance.FocusedObject;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
