using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;


public class Broadcaster : MonoBehaviour {

    public RawImage rawimage;
    public WebCamTexture webcamTexture;
    WebCamTexture activeCameraTexture;

    public WebCamDevice webCameraDevice;
    WebCamDevice activeCameraDevice;

    private int numDevices = 0;

    void Awake()
    {
    }

    // Use this for initialization
    IEnumerator Start () {

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);
        string firstCam;

        if (Application.HasUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone))
        {
            //WebCamTexture webcamTexture = new WebCamTexture();


            if (WebCamTexture.devices.Length == 0)
            {
                Debug.Log("No devices cameras found");
                //return;
            }
            else
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                for (int i = 0; i < devices.Length; i++)
                {
                    numDevices++;
                    Debug.Log(devices[i].name);

                }
                    
                if(numDevices > 1)
                {
                   firstCam = devices[1].name;
                   webCameraDevice = WebCamTexture.devices[1];
                }
                else
                {
                    firstCam = devices[0].name;
                    webCameraDevice = WebCamTexture.devices[0];
                }

            }
            
            webcamTexture = new WebCamTexture(webCameraDevice.name);
            SetActiveCamera(webcamTexture);
        }
        else
        {
            Debug.Log("Not authorized!");
        }
    }

    public void SetActiveCamera(WebCamTexture cameraToUse)
    {

        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();
            Debug.Log("active camera stopped!");
        }

        activeCameraTexture = cameraToUse;
        activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
            device.name == cameraToUse.deviceName);

        rawimage.texture = activeCameraTexture;
        rawimage.material.mainTexture = activeCameraTexture;

        activeCameraTexture.Play();
    }

    // Update is called once per frame
    void Update ()
    {
        var miniDisplay = GameObject.Find("MiniDisplay");
        var viveCamera = GameObject.Find("Camera (head)");

        miniDisplay.transform.LookAt(viveCamera.transform.position, Vector3.up);
        miniDisplay.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
        miniDisplay.transform.localPosition = new Vector3(-0.2f, 0, 0);
    }

    public void StopCamera()
    {
        activeCameraTexture.Stop();
    }

    void OnDestroy()
    {

        //gameObject.renderer.material.mainTexture.Stop();
        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();

        }
    }
}
