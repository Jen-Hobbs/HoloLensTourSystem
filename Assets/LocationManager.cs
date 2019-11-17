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

    // Update is called once per frame
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
            
            // Debug.Log("curser hit");
            FocusedObject = hitInfo.collider.gameObject;

        }
        else
        {
            Debug.Log("nothing found");
        }
    }
}
