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
            // Tap pertama → cube awal
            m_startCube = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(m_startCube);

            // buat line sementara yang ketarik kamera
            GameObject lineObj = new GameObject("TempLine");
            m_currentLine = lineObj.AddComponent<LineRenderer>();
            m_currentLine.material = new Material(Shader.Find("Sprites/Default"));
            m_currentLine.startWidth = m_currentLine.endWidth = 0.01f;
            m_currentLine.positionCount = 2;
            manager.RegisterObject(lineObj);

            m_currentLabel = GameObject.Instantiate(manager.m_distanceUIPrefab, lineObj.transform);
            manager.RegisterObject(m_currentLabel);
        }
        else if (m_endCube == null)
        {
            // Tap kedua → cube akhir + fix line
            m_endCube = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(m_endCube);

            // simpan line permanen
            m_lines.Add(m_currentLine);
            m_labels.Add(m_currentLabel);

            // reset state supaya siap bikin line baru
            m_startCube = null;
            m_endCube = null;
            m_currentLine = null;
            m_currentLabel = null;
        }
    }

    public override void OnUpdate()
    {
        // Update line sementara (kalau ada)
        if (m_startCube != null && m_currentLine != null)
        {
            Vector3 start = m_startCube.transform.position;
            Vector3 end;

            if (m_endCube == null)
            {
                // kalau cube akhir belum ada → tarik ke posisi kamera/pose AR
                end = PlaceManager.Instance.CurrentPose.position;
            }
            else
            {
                // kalau cube akhir udah ada → fix di posisinya
                end = m_endCube.transform.position;
            }

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
