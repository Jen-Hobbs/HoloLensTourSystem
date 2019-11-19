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
    Texture2D capturedObj = null;
    public AudioSource audioSource;
    public Text textbox1;
    public Shader sh;
    public GameObject infoDoc;
    private bool qrAvailable = false;
    void Start()
    {
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        audioSource.Stop();
        capturedObj = new Texture2D(cameraResolution.width, cameraResolution.height);

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
        photoCaptureFrame.UploadImageDataToTexture(capturedObj);
    }

    IEnumerator waiter(float sec)
    {
        yield return new WaitForSeconds(sec);
    }

    // receives an image from url and displays the image 
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            // create a quad and place the received image on it.
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
            quadRenderer.material = new Material(sh);

            Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 1.1f;
            quad.transform.position = new Vector3(pos.x + 0.9f, pos.y, pos.z);
            quad.transform.LookAt(Camera.main.transform);
            quad.transform.rotation = Quaternion.LookRotation(quad.transform.position - Camera.main.transform.position);

            quadRenderer.material.SetTexture("_MainTex", ((DownloadHandlerTexture)request.downloadHandler).texture);

            capturedObj = null;
            capturedObj = new Texture2D(cameraResolution.width, cameraResolution.height);
        }
    }

    // receives an audio from url and plays it 
    // drawbacks - it takes some time until completing downloading and finishing play
    IEnumerator PlayAudio(string MediaUrl)
    {
        UnityWebRequest music = UnityWebRequestMultimedia.GetAudioClip(MediaUrl, AudioType.WAV);
        yield return music.SendWebRequest();

        if (music.isNetworkError)
        {
            Debug.Log(music.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(music);

            if (clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

    }

    // called when user clicked with his finger
    // takes a photo with hololens camera and tries to decode the image
    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("OnInputDown(InputEventData eventData)");
        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        StartCoroutine(waiter(0.5f));
        decodeQR(capturedObj);
    }

    // where decoding process occurs
    // if qr code is decoded successfully,
    // it processes the decoded data.
    // The decoded data would be url.
    // But I used just hardcoded url for now.
    void decodeQR(Texture2D qrToDecode)
    {
        if (qrToDecode != null)
        {
            string resultStr = "";
            byte[] byteArr = ProcessColor32(qrToDecode.GetPixels32());
#if !UNITY_EDITOR
            resultStr = ZxingUwp.Barcode.Read(byteArr, qrToDecode.width, qrToDecode.height);
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
                /* succeeded to decode QR code */

                // receive an image from url and display the image 
                //StartCoroutine(DownloadImage("https://homepages.cae.wisc.edu/~ece533/images/frymire.png"));

                // receive an audio from url and play 
                // but disabled for now due to time needed until fully retrieve data and the end of play time
                // StartCoroutine(PlayAudio("https://www.kozco.com/tech/piano2.wav"));
            }
        }
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
    private static byte[] ProcessColor32(Color32[] colors)
    {
        if (colors == null || colors.Length == 0)
            return null;

        int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
        int length = lengthOfColor32 * colors.Length;
        byte[] bytes = new byte[length];

        GCHandle handle = default(GCHandle);
        try
        {
            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            Marshal.Copy(ptr, bytes, 0, length);
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }

        return bytes;
    }
}
