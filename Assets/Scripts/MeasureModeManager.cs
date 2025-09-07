using UnityEngine;
using UnityEngine.UI;
using TMPro; // biar bisa akses TMP Text

/// <summary>
/// Manager untuk mengatur mode measure pakai 3 tombol manual di UI
/// </summary>
public class MeasureModeManager : MonoBehaviour
{
    [Header("Setup")]
    public MeasureManager measureManager;

    [Header("Tombol UI")]
    public Button widthHeightButton;
    public Button polylineButton;
    public Button angleButton;

    [Header("Mode Data")]
    public MeasureModeData widthHeightData;
    public MeasureModeData polylineData;
    public MeasureModeData angleData;

    private void Start()
    {
        // set teks button dari scriptableobject
        if (widthHeightButton != null && widthHeightData != null)
        {
            widthHeightButton.onClick.AddListener(() => OnModeSelected(widthHeightData));
            SetButtonText(widthHeightButton, widthHeightData.modeName);
        }

        if (polylineButton != null && polylineData != null)
        {
            polylineButton.onClick.AddListener(() => OnModeSelected(polylineData));
            SetButtonText(polylineButton, polylineData.modeName);
        }

        if (angleButton != null && angleData != null)
        {
            angleButton.onClick.AddListener(() => OnModeSelected(angleData));
            SetButtonText(angleButton, angleData.modeName);
        }
    }

    private void SetButtonText(Button button, string text)
    {
        // Cari TMP_Text di anak tombol
        var tmp = button.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.text = text;
    }

    public void OnModeSelected(MeasureModeData data)
    {
        BaseMeasureMode newMode = CreateModeFromData(data);
        measureManager.SetMode(newMode);
        Debug.Log($"Mode di-set: {data.modeName}");
    }

    private BaseMeasureMode CreateModeFromData(MeasureModeData data)
    {
        switch (data.modeType)
        {
            case MeasureModeData.ModeType.WidthHeight:
                return new WidthHeightMode(measureManager);

            case MeasureModeData.ModeType.Polyline:
                return new PolylineMode(measureManager);

            case MeasureModeData.ModeType.Angle:
                return new AngleMode(measureManager);

            default:
                Debug.LogWarning("Mode type belum di-handle: " + data.modeType);
                return null;
        }
    }
}
