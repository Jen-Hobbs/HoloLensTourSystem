using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDocument : MonoBehaviour
{
    public GameObject infoDoc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnOpenDocument()
    {
        Debug.Log("open document");
        GameObject info = Instantiate(infoDoc, new Vector3(0, 0, 2), Quaternion.identity) as GameObject;
        info.transform.parent = GameObject.Find("GameManager").transform;
        
    }
}
