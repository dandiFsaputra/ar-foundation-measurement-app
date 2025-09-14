using UnityEngine;
using System.Collections.Generic;

public class WidthHeightMode : BaseMeasureMode
{
    private GameObject m_startCube;
    private GameObject m_endCube;
    private LineRenderer m_currentLine;
    private GameObject m_currentLabel;

    // simpan semua garis permanen
    private List<LineRenderer> m_lines = new List<LineRenderer>();
    private List<GameObject> m_labels = new List<GameObject>();

    public WidthHeightMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter WidthHeight Mode");
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit WidthHeight Mode");

        // kalau mau clear semua line lama, uncomment ini:
        // foreach (var line in m_lines) GameObject.Destroy(line.gameObject);
        // foreach (var label in m_labels) GameObject.Destroy(label);

        m_startCube = null;
        m_endCube = null;
        m_currentLine = null;
        m_currentLabel = null;
    }

    public override void OnTap(Vector3 worldPos)
    {
        if (m_startCube == null)
        {
            m_startCube = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(m_startCube);

            m_currentLine = CreateLine("TempLine");
            m_currentLabel = CreateLabel(worldPos, "0.00 m");
        }
        else if (m_endCube == null)
        {
            m_endCube = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(m_endCube);

            m_lines.Add(m_currentLine);
            m_labels.Add(m_currentLabel);

            m_startCube = null;
            m_endCube = null;
            m_currentLine = null;
            m_currentLabel = null;
        }
    }

    public override void OnUpdate()
    {
        if (m_startCube != null && m_currentLine != null)
        {
            Vector3 start = m_startCube.transform.position;
            Vector3 end = (m_endCube == null) ? PlaceManager.Instance.CurrentPose.position : m_endCube.transform.position;

            UpdateLine(m_currentLine, start, end);
            UpdateLabel(m_currentLabel, start, end);
        }

        foreach (var label in m_labels) FaceToCamera(label);
    }

}
