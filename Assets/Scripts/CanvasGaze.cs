using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class CanvasGaze : MonoBehaviour
{

    private GameObject focusedObject;
    private GestureRecognizer recognizer;
    /// <summary>
    /// starts default gestures
    /// </summary>
    void Start()
    {
        restartGestures();
    }
    /// <summary>
    /// content made from https://docs.microsoft.com/en-us/windows/mixed-reality/holograms-101
    /// gets cameras position and sends raycast based of the persons position to see if it hits an object if it does it shows the mesh
    /// (will highlight object when fully implemented)
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        GameObject oldFocusedObject = focusedObject;
        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {

            if (hitInfo.transform.tag == "Next Page")
            {
                focusedObject = hitInfo.collider.gameObject;
                focusedObject.GetComponent<Image>().color = new Color32(4, 143, 253, 255);

            }

            else if (hitInfo.transform.tag == "Previous Page")
            {
                focusedObject = hitInfo.collider.gameObject;
                focusedObject.GetComponent<Image>().color = new Color32(4, 143, 253, 255);
            }
            else if (hitInfo.transform.tag == "Close")
            {
                focusedObject = hitInfo.collider.gameObject;
                focusedObject.GetComponent<Image>().color = new Color32(4, 143, 253, 255);
            }
            else if (hitInfo.transform.tag == "DragBar")
            {
                focusedObject = hitInfo.collider.gameObject;

            }
            else if (hitInfo.transform.gameObject.tag == "Contents")
            {
                Debug.Log("contents button hit");
                focusedObject = hitInfo.collider.gameObject;
                focusedObject.GetComponent<Image>().color = new Color32(4, 143, 253, 255);

            }
            else
            {
                Debug.Log("nothing selected something hit");
                Debug.Log(hitInfo.collider.gameObject.tag);
                focusedObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                focusedObject = null;
                //this.gameObject.transform.Find("Next Page").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                //focusedObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }

            if (focusedObject != oldFocusedObject)
            {
                recognizer.CancelGestures();
                recognizer.StartCapturingGestures();
            }
            //// If the raycast hit a hologram...
            //// Display the cursor mesh.
            //meshRenderer.enabled = true;

            //// Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            //// Rotate the cursor to hug the surface of the hologram.
            //this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        else
        {
            if (focusedObject != null)
            {

                // If the raycast did not hit a hologram, ensure previous focused object is not selected
                focusedObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            focusedObject = null;
            Debug.Log("focused object is null");
        }
    }
    /// <summary>
    /// adds gestures for table of contents buttons
    /// </summary>
    /// <param name="titles">titles added to the buttons</param>
    public void addGestures(string[] titles)
    {
        Debug.Log("add gestures started");
        Debug.Log(titles);
        recognizer.CancelGestures();
        recognizer.Tapped += (args) =>
        {
            Debug.Log("tapped");
            if (focusedObject != null)
            {
                Debug.Log(focusedObject);
                for (int x = 0; x < titles.Length; x++)
                {
                    if (focusedObject == this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find(titles[x]).gameObject)
                    {
                        Debug.Log("table of contents page " + x + "called");
                        Debug.Log(this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find(titles[x]).gameObject.name);
                        string title = this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find(titles[x]).gameObject.name;
                        this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnChangeContent", title, SendMessageOptions.DontRequireReceiver);
                        //this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find(titles[x]).gameObject.SendMessage("OnChangePage", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        };
        recognizer.StartCapturingGestures();
    }
    /// <summary>
    /// restart gestures to default gestures
    /// </summary>
    public void restartGestures()
    {
        if(recognizer != null)
        {
            recognizer.CancelGestures();
        }
        recognizer = new GestureRecognizer();
        recognizer.Tapped += (args) =>
        {

            ///change to be based on object
            if (focusedObject != null)
            {
                if (focusedObject == this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find("Next Page").gameObject)
                {
                    Debug.Log("next page called by gesture");
                    this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnNextPage", SendMessageOptions.DontRequireReceiver);
                }
                else if (focusedObject == this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find("Previous").gameObject)
                {
                    Debug.Log("previous page");
                    this.gameObject.transform.Find("InfoDoc(Clone)").SendMessage("OnPreviousPage", SendMessageOptions.DontRequireReceiver);
                }
                else if (focusedObject == this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find("Close").gameObject || focusedObject == this.gameObject.transform.Find("Demo").transform.Find("Close").gameObject)
                {
                    Debug.Log("close document");
                    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Canvas");
                    recognizer.CancelGestures();
                    restartGestures();
                    this.gameObject.GetComponent<SpeechManager>().restartSpeech();
                    for (var i = 0; i < gameObjects.Length; i++)
                    {
                        Destroy(gameObjects[i]);
                    }
                }
                



            }
            else
            {
                Debug.Log(this.gameObject.transform.Find("Main"));
                this.gameObject.transform.Find("Main").SendMessage("runCanvas", SendMessageOptions.DontRequireReceiver);
            }
        };
        recognizer.HoldStartedEvent += (source, ray) =>
        {
            Debug.Log("hold started");
            if (focusedObject == this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find("DragBar").gameObject)
            {
                Debug.Log("correct thing selected and holding");
                focusedObject.transform.parent.GetComponent<DragManager>().Move();
            }
            else
            {
                focusedObject.transform.parent.GetComponent<DragManager>().PlaceCanvas();
            }

        };
        recognizer.HoldCompletedEvent += (source, ray) =>
        {
            Debug.Log("hold completed");

            //if (focusedObject == this.gameObject.transform.Find("InfoDoc(Clone)").transform.Find("DragBar").gameObject)
            //{
            focusedObject.transform.parent.GetComponent<DragManager>().PlaceCanvas();
            //}
        };
        recognizer.HoldCanceledEvent += (source, ray) =>
        {
            Debug.Log("hold canceled");
            focusedObject.transform.parent.GetComponent<DragManager>().PlaceCanvas();
        };
        recognizer.StartCapturingGestures();

    }
}
