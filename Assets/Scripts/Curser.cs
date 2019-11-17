using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curser : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private Vector3 headPosition;
    private Vector3 gazeDirection;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        headPosition = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.position;
        //RaycastHit hitInfo;
        //if (Physics.Raycast(headPosition, gazeDirection, out hitInfo)
            //&& hitInfo.transform.tag == "interactible"
        //    )
        
            meshRenderer.enabled = true;
            this.transform.position = headPosition;
            //this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
    }
}
