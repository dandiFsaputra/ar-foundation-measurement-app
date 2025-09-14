using UnityEngine;

public class FlashlightAndroid : MonoBehaviour
{
    private AndroidJavaObject cameraManager;
    private string cameraId;
    private bool isFlashOn = false;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            // Ambil activity Unity
            using (AndroidJavaObject unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity"))
            {
                // Ambil CAMERA_SERVICE (bukan "camera")
                AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");
                cameraManager = context.Call<AndroidJavaObject>("getSystemService", "camera");

                if (cameraManager != null)
                {
                    string[] cameraIdList = cameraManager.Call<string[]>("getCameraIdList");
                    if (cameraIdList.Length > 0)
                        cameraId = cameraIdList[0]; // biasanya kamera belakang
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("⚠️ Gagal init flashlight: " + e.Message);
        }
#endif
    }

    public void ToggleFlashlight()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (cameraManager == null || string.IsNullOrEmpty(cameraId))
        {
            Debug.LogWarning("⚠️ Kamera tidak ditemukan untuk flashlight.");
            return;
        }

        try
        {
            isFlashOn = !isFlashOn;
            cameraManager.Call("setTorchMode", cameraId, isFlashOn);
            Debug.Log("Flashlight: " + (isFlashOn ? "ON" : "OFF"));
        }
        catch (System.Exception e)
        {
            Debug.LogError("⚠️ Gagal toggle flashlight: " + e.Message);
        }
#endif
    }

    private void OnDisable()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (cameraManager != null && !string.IsNullOrEmpty(cameraId))
        {
            try { cameraManager.Call("setTorchMode", cameraId, false); }
            catch { }
        }
#endif
    }
}
