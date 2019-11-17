using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasContent : MonoBehaviour
{
    public TextMeshProUGUI textInfo;
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
        textInfo.text = "text changed";
        
    }
    void OnPreviousPage()
    {
        Debug.Log("Pefious page");
        textInfo.text = "previous page";
    }
}
