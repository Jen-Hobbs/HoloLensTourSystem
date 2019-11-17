using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasContent : MonoBehaviour
{
    public GameObject docImage;
    public TextMeshProUGUI textInfo;
    public Sprite NextSprite;
    public Sprite PreviousSprite;
    public TextMeshProUGUI title;
    // Start is called before the first frame update
    void Start()
    {
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
    void OnPreviousPage()
    {
        Debug.Log("previous page");
        textInfo.text = "previous page content";
        title.text = "Previous Page";
        docImage.GetComponent<Image>().sprite = PreviousSprite;
    }
}
