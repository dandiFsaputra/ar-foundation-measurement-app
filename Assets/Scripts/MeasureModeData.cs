using UnityEngine;

/// <summary>
/// scriptableobject untuk menyimpan data properti dari tiap mode measure
/// Tujuannya supaya mode bisa diatur lewat asset, gampang ditambah tanpa ubah kode.
/// </summary>
[CreateAssetMenu(fileName = "MeasureModeData", menuName = "Scriptable Objects/MeasureModeData")]
public class MeasureModeData : ScriptableObject
{
    public enum ModeType 
    { 
        WidthHeight, 
        Polyline,
        Angle
    }

    [Header("Mode Properties")]
    public ModeType modeType;
    public string modeName;
    public Sprite icon;
    public Sprite image;
    [TextArea] public string description;
}
