using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class QRCodeScanner : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    Texture2D screenCaptureTexture = null;
    Texture2D croppedQR;
    int cameraWidth;
    int cameraHeight;

    public Text textbox1;

    void Start()
    {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        cameraWidth = cameraResolution.width;
        cameraHeight = cameraResolution.height;

        screenCaptureTexture = new Texture2D(cameraWidth, cameraHeight);

        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                StartCoroutine(WaitAndPrint());
            });
        });
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        photoCaptureFrame.UploadImageDataToTexture(screenCaptureTexture);
    }

    private IEnumerator WaitAndPrint()
    {
        while (true)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            yield return new WaitForSeconds(0.5f);

            Color[] c = screenCaptureTexture.GetPixels(0, 0, cameraWidth, cameraHeight);
            croppedQR = new Texture2D(cameraWidth, cameraHeight);

            croppedQR.SetPixels(c);
            croppedQR.Apply();

            decodeQR(croppedQR);
            
            yield return new WaitForSeconds(1.0f);
        }
    }

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
        }
    }

    void OnDestroy()
    {
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
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
