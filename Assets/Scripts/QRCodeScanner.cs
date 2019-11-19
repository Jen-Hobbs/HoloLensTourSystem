using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR.WSA.Input;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Networking;

/**
 * Creater : Joe
 * Date : 11/17/2019
 * 
 * Description : 
 * This script is to take photos when a user takes a gesture (tap) - method: OnInputDown()
 * and processe  the image for decoding QR code                    - method: decodeQR()
 * If it is successfully decoded, the returned data would be url,
 * and it sends request for image using the image url               - method: DownloadImage()
 * and it sends request for audio file using the audio url          - method: PlayAudio() {disabled for now}
 * 
 * P.S : the url is hardcoded now (will be modified later, based on the completion of web admin)
 */

public class QRCodeScanner : MonoBehaviour, IMixedRealityInputHandler
{
    Resolution cameraResolution;
    PhotoCapture photoCaptureObject = null;
    public AudioSource audioSource;
    public Text textbox1;
    public Shader sh;
    public GameObject infoDoc;
    private bool qrAvailable = false;

    void Start()
    {
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        audioSource.Stop();

        // create PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) { });
        });
    }


    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        List<byte> imageBufferList = new List<byte>();

        photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

        string resultStr = "";
        byte[] byteArr = imageBufferList.ToArray();
#if !UNITY_EDITOR
            resultStr = ZxingUwp.Barcode.Read(byteArr, cameraResolution.width, cameraResolution.height);
#endif
        textbox1.text = resultStr;

        if (resultStr == "null")
        {
            /* failed to decode QR code */
        }
        else
        {
            //qrAvailable = true;
            GameObject info = Instantiate(infoDoc, new Vector3(0, 0, 2), Quaternion.identity) as GameObject;
            info.transform.parent = GameObject.Find("GameManager").transform;
            info.GetComponent<HttpHandler>().postReq("name", resultStr);
            /* succeeded to decode QR code */
        }
    }

    // called when user clicked with his finger
    // takes a photo with hololens camera and tries to decode the image
    public void OnInputDown(InputEventData eventData)
    {
        // Debug.Log("OnInputDown(InputEventData eventData)");
        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    }

    void OnOpenDocument()
    {
        if (qrAvailable == true)
        {
            //ensure no other document is open
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("interactible");
            for (var i = 0; i < gameObjects.Length; i++)
            {
                Destroy(gameObjects[i]);
            }
            Debug.Log("open document");
            GameObject info = Instantiate(infoDoc, new Vector3(0, 0, 2), Quaternion.identity) as GameObject;
            info.transform.parent = GameObject.Find("GameManager").transform;
            qrAvailable = false;
        }
        else
        {
            Debug.Log("qr code not available");
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
    }
}
