using UnityEngine;
using System.Collections.Generic;

public class PolylineMode : BaseMeasureMode
{
    private List<GameObject> m_cubes = new List<GameObject>();
    private LineRenderer m_currentLine;
    private GameObject m_currentLabel;

    // simpan garis + label permanen
    private List<LineRenderer> m_lines = new List<LineRenderer>();
    private List<GameObject> m_labels = new List<GameObject>();

    public PolylineMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter Polyline Mode");
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit Polyline Mode");

        m_cubes.Clear();
        m_currentLine = null;
        m_currentLabel = null;
    }

    public override void OnTap(Vector3 worldPos)
    {
        // Buat cube baru di titik tap
        var cube = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
        m_cubes.Add(cube);
        manager.RegisterObject(cube);

        if (m_cubes.Count == 1)
        {
            // Tap pertama → langsung bikin line sementara dari cube1 ke AR pose
            GameObject lineObj = new GameObject("TempLine");
            m_currentLine = lineObj.AddComponent<LineRenderer>();
            m_currentLine.material = new Material(Shader.Find("Sprites/Default"));
            m_currentLine.startWidth = m_currentLine.endWidth = 0.01f;
            m_currentLine.positionCount = 2;

            manager.RegisterObject(m_currentLine.gameObject);


            m_currentLabel = GameObject.Instantiate(manager.m_distanceUIPrefab, lineObj.transform);
            manager.RegisterObject(m_currentLabel);
        }
        else
        {
            // Kalau sudah ada line sementara → finalize dulu jadi permanen
            if (m_currentLine != null && m_currentLabel != null)
            {
                // Set akhir line ke cube terakhir
                m_currentLine.SetPosition(0, m_cubes[m_cubes.Count - 2].transform.position);
                m_currentLine.SetPosition(1, cube.transform.position);

                float distance = Vector3.Distance(
                    m_cubes[m_cubes.Count - 2].transform.position,
                    cube.transform.position
                );
                var text = m_currentLabel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                    text.text = $"{distance:F2} m";

                // Simpan permanen
                m_lines.Add(m_currentLine);
                m_labels.Add(m_currentLabel);

                m_currentLine = null;
                m_currentLabel = null;
            }

            // Setelah tap kedua / ketiga dst → bikin line sementara baru dari cube terakhir ke AR pose
            GameObject lineObj = new GameObject("TempLine");
            m_currentLine = lineObj.AddComponent<LineRenderer>();
            m_currentLine.material = new Material(Shader.Find("Sprites/Default"));
            m_currentLine.startWidth = m_currentLine.endWidth = 0.01f;
            m_currentLine.positionCount = 2;
            manager.RegisterObject(m_currentLine.gameObject);

            m_currentLabel = GameObject.Instantiate(manager.m_distanceUIPrefab, lineObj.transform);
        }
    }

    public override void OnUpdate()
    {
        if (m_currentLine != null && m_cubes.Count > 0 && PlaceManager.Instance.HasValidPos)
        {
            Vector3 start = m_cubes[m_cubes.Count - 1].transform.position;
            Vector3 end = PlaceManager.Instance.CurrentPose.position;

            m_currentLine.SetPosition(0, start);
            m_currentLine.SetPosition(1, end);

            float distance = Vector3.Distance(start, end);
            var text = m_currentLabel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = $"{distance:F2} m";

            Vector3 mid = (start + end) / 2;
            m_currentLabel.transform.position = mid + Vector3.up * 0.05f;

            if (manager.m_arCamera != null)
                m_currentLabel.transform.rotation =
                    Quaternion.LookRotation(m_currentLabel.transform.position - manager.m_arCamera.transform.position);
        }

        // selalu menghadap kamera
        if (manager.m_arCamera != null)
        {
            foreach (var label in m_labels)
            {
                if (label != null)
                {
                    label.transform.rotation =
                        Quaternion.LookRotation(label.transform.position - manager.m_arCamera.transform.position);
                }
            }

            if (m_currentLabel != null)
            {
                m_currentLabel.transform.rotation =
                    Quaternion.LookRotation(m_currentLabel.transform.position - manager.m_arCamera.transform.position);
            }
        }

    }
}
