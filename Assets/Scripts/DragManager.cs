using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://forums.hololens.com/discussion/4392/applying-hold-gesture-and-gameobject-move-code-to-the-origami-project
public class DragManager : MonoBehaviour
{
    private bool holding = false;
    // Start is called before the first frame update
    void Start()
    {
        holding = false;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 canvasPosition;
        if (holding)
        {
            canvasPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
            transform.position = canvasPosition;

        }
        

    }
    public void Move()
    {

        Debug.Log("testing successful");
        holding = true;
    }
    public void PlaceCanvas()
    {
        Debug.Log("place canvas");
        holding = false;
    }
}
