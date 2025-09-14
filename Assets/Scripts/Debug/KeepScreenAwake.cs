using UnityEngine;

public class KeepScreenAwake : MonoBehaviour
{
    void Start()
    {
        // Cegah layar HP mati
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnDestroy()
    {
        // Balikin ke default kalau script / scene dihancurkan
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
