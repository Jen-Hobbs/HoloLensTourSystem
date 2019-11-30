using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    
    private Vector3 headPosition;
    private Vector3 gazeDirection;
    public static LocationManager Instance { get; private set; }
    public GameObject FocusedObject { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
    }
    void Awake()
    {
        Instance = this;
    }
    //Color to change #048FFD
    // Update is called once per frame
    /// <summary>
    /// sends raycast outwards to see if an object is hit makes it possible to check what is hit to select different objects
    /// </summary>
    void Update()
    {
        GameObject oldFocusObject = FocusedObject;

        headPosition = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.position;
        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo)

            //&& hitInfo.transform.tag == "interactible"
            )
        {
            Debug.Log("object Hit");
            if(hitInfo.transform.tag == "Next Page")
            {
                Debug.Log("Next Page button hit");
            }
            else if(hitInfo.transform.tag == "Next Page")
            {
                Debug.Log("Previous Page button hit");
            }
            // Debug.Log("curser hit");
            FocusedObject = hitInfo.collider.gameObject;

        }
        else
        {
        
            //Debug.Log("nothing found");
        }
    }
}
