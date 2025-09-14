using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DebugInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private MeasureManager measureManager;
    [SerializeField] private ARPlaneManager arPlaneManager;

    private string arCoreSupport = "Cheking";

    private void Start()
    {
        StartCoroutine(CheckARCoreSupport());
    }
    private void Update()
    {
        //if (debugText == null) return;

        //string deviceName = SystemInfo.deviceName;
        //string deviceModel = SystemInfo.deviceModel;  
        //string deviceOS = SystemInfo.operatingSystem;   
        //string deviceArCore = SystemInfo.supportarcore

        if (debugText == null || measureManager == null) return;

        debugText.text =
            $"Device Name: {SystemInfo.deviceName}\n" +
            $"Device : {SystemInfo.deviceModel}\n" +
            $"OS     : {SystemInfo.operatingSystem}\n" +
            $"ARCore : {arCoreSupport}\n" +
            $"Objek yang Muncul  : {measureManager.spawnedObjects.Count}\n" +
            $"Jumlah Lantai/Dinding terdeteksi : {(arPlaneManager != null ? arPlaneManager.trackables.count : 0)}";
    }

    private IEnumerator CheckARCoreSupport()
    {
        if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            arCoreSupport = "Tidak Support";
        }
        else if (ARSession.state == ARSessionState.Ready || ARSession.state == ARSessionState.SessionInitializing || ARSession.state == ARSessionState.SessionTracking)
        {
            arCoreSupport = "Support";
        }
        else
        {
            arCoreSupport = "Cheking";
        }
    }
}
