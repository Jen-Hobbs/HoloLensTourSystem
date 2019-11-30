using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasContent : MonoBehaviour
{
    public GameObject docImage;
    public TextMeshProUGUI textInfo;
    public Sprite NextSprite; //test content
    public Sprite PreviousSprite; //test content
    public TextMeshProUGUI title;
    public static string[] titles; //test content
    public GameObject content;
    private const int buttonX = 30;
    private const int buttonStartY = -10;
    private const int buttonYScale = 20;
    // Start is called before the first frame update
    /// <summary>
    /// instantiates the buttons on the canvas that are dynamically generated
    /// Not fully implemented
    /// </summary>
    void Start()
    {
        //test content added from db
        titles = new string[5];
        titles[0] = "information";
        titles[1] = "basic";
        titles[2] = "instructions";
        titles[3] = "contact information";
        titles[4] = "something else";


     //   for(int x = 0; x < titles.Length; x++)
      //  {
     //       GameObject button = Instantiate(content, new Vector3(buttonX, buttonStartY - (x * buttonYScale), 0), Quaternion.identity) as GameObject;
     //       button.transform.SetParent(gameObject.transform.parent.transform,false);
     //       button.GetComponentInChildren<TextMeshProUGUI>().text = titles[x];
     //   }
        Debug.Log("canvas adding keywords");
        //SpeechManager.ContentCanvas();
        Debug.Log("Next Page Script Started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Changes the content towards the next page
    /// </summary>
    void OnNextPage()
    {
        Debug.Log("next page");
        textInfo.text = "next page text changed";
        docImage.GetComponent<Image>().sprite = NextSprite;
        title.text = "next Page";
    }
    /// <summary>
    /// changes content to previous page
    /// </summary>
    void OnPreviousPage()
    {
        Debug.Log("previous page");
        textInfo.text = "previous page content";
        title.text = "Previous Page";
        docImage.GetComponent<Image>().sprite = PreviousSprite;
    }
    /// <summary>
    /// changes content to to selected button not implemented
    /// </summary>
    /// <param name="buttonName"></param>
    void OnContent(string buttonName)
    {
        Debug.Log("content accessed");
        Debug.Log(buttonName);
    }
    
}
