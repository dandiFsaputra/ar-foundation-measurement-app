using UnityEngine;
using System.Collections.Generic;

public class PolylineMode : BaseMeasureMode
{
    private readonly List<GameObject> m_cubes = new();
    private readonly List<LineRenderer> m_lines = new();
    private readonly List<GameObject> m_labels = new();

    private LineRenderer m_currentLine;
    private GameObject m_currentLabel;

    public PolylineMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter Polyline Mode");
        ResetState();
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit Polyline Mode");
        ResetState();
    }

    public override void OnTap(Vector3 worldPos)
    {
        var cube = SpawnCube(worldPos);
        m_cubes.Add(cube);

        if (m_cubes.Count == 1)
        {
            m_currentLine = CreateLine("TempLine");
            m_currentLabel = CreateLabel(Vector3.zero, "0.00 m");
        }
        else
        {
            if (m_currentLine != null && m_currentLabel != null)
            {
                Vector3 start = m_cubes[m_cubes.Count - 2].transform.position;
                Vector3 end = m_cubes[m_cubes.Count - 1].transform.position;

                UpdateLine(m_currentLine, start, end);
                UpdateLabel(m_currentLabel, start, end);

                m_lines.Add(m_currentLine);
                m_labels.Add(m_currentLabel);

                m_currentLine = null;
                m_currentLabel = null;
            }

            m_currentLine = CreateLine("TempLine");
            m_currentLabel = CreateLabel(Vector3.zero, "0.00 m");
        }
    }

    public override void OnUpdate()
    {
        if (m_currentLine != null && m_cubes.Count > 0 && PlaceManager.Instance.HasValidPos)
        {
            Vector3 start = m_cubes[^1].transform.position;
            Vector3 end = PlaceManager.Instance.CurrentPose.position;

            UpdateLine(m_currentLine, start, end);
            UpdateLabel(m_currentLabel, start, end);
        }

        foreach (var label in m_labels) FaceToCamera(label);
        if (m_currentLabel != null) FaceToCamera(m_currentLabel);
    }

    // ---------------- Helper ----------------
    private GameObject SpawnCube(Vector3 pos)
    {
        var cube = GameObject.Instantiate(manager.m_cubePrefab, pos, Quaternion.identity);
        manager.RegisterObject(cube);
        return cube;
    }

    private void ResetState()
    {
        m_cubes.Clear();
        m_lines.Clear();
        m_labels.Clear();
        m_currentLine = null;
        m_currentLabel = null;
    }
}
