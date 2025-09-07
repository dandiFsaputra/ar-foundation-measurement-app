using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manager untuk mengatur mode measure (scriptableobject + ui)
/// </summary>
public class MeasureModeManager : MonoBehaviour
{
    [Header("Measure Mode Data")]
    public MeasureManager measureManager;
    public Transform modePanelParent;
    public GameObject modeButtonPrefab;
    public List<MeasureModeData> modeDataList;

    private void Start()
    {
        foreach (var data in modeDataList)
        {
            GameObject btnObj = Instantiate(modeButtonPrefab, modePanelParent);

            btnObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = data.modeName;
            btnObj.GetComponentInChildren<Image>().sprite = data.icon;

            var button = btnObj.GetComponent<Button>();
            MeasureModeData capturedData = data;
            button.onClick.AddListener(() => OnModeSelected(capturedData));
        }

    }

    public void OnModeSelected(MeasureModeData data)
    {
        BaseMeasureMode newMode = CreateModeFromData(data);
        measureManager.SetMode(newMode);
    }

    private BaseMeasureMode CreateModeFromData(MeasureModeData data)
    {
        switch (data.modeType)
        {
            case MeasureModeData.ModeType.WidthHeight:
                return new WidthHeightMode(measureManager);
            default:
                return null;
        }
    }

}
