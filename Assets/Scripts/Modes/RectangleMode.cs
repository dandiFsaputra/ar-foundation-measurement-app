using System.Collections.Generic;
using UnityEngine;

public class RectangleMode : BaseMeasureMode
{
    private List<GameObject> m_cubes = new List<GameObject>(); // 1–4
    private List<LineRenderer> m_lines = new List<LineRenderer>();
    private List<GameObject> m_labels = new List<GameObject>();

    private int m_stage = 0; // 0=no cube, 1=1 cube, 2=2 cube, 3=rectangle fix

    public RectangleMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter Rectangle Mode");
        m_stage = 0;
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit Rectangle Mode");
        ResetState();
    }

    public override void OnTap(Vector3 worldPos)
    {
        if (m_stage == 0)
        {
            // Tap pertama → cube1
            var cube1 = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            m_cubes.Add(cube1);
            manager.RegisterObject(cube1);

            m_stage = 1;
        }
        else if (m_stage == 1)
        {
            // Tap kedua → cube2
            var cube2 = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            m_cubes.Add(cube2);
            manager.RegisterObject(cube2);

            // Spawn cube3 & cube4 sementara
            var cube3 = GameObject.Instantiate(manager.m_cubePrefab, cube2.transform.position, Quaternion.identity);
            var cube4 = GameObject.Instantiate(manager.m_cubePrefab, m_cubes[0].transform.position, Quaternion.identity);

            m_cubes.Add(cube3);
            m_cubes.Add(cube4);

            manager.RegisterObject(cube3);
            manager.RegisterObject(cube4);

            m_stage = 2; // sekarang cube3 & 4 masih gerak
        }
        else if (m_stage == 2)
        {
            // Tap ketiga → fix rectangle
            CreateRectangleLines();
            m_stage = 3;
        }
        else if (m_stage == 3)
        {
            // Mulai rectangle baru tanpa hapus yang lama
            ResetState();
            OnTap(worldPos); // langsung treat tap sebagai tap pertama rectangle baru
        }
    }

    public override void OnUpdate()
    {
        if (m_stage == 1 && PlaceManager.Instance.HasValidPos)
        {
            // Update line dummy dari cube1 ke kamera
            Vector3 start = m_cubes[0].transform.position;
            Vector3 end = PlaceManager.Instance.CurrentPose.position;
            Debug.DrawLine(start, end, Color.yellow);
        }

        if (m_stage == 2 && PlaceManager.Instance.HasValidPos)
        {
            // Update posisi cube3 & cube4 sesuai arah kamera
            Vector3 c1 = m_cubes[0].transform.position;
            Vector3 c2 = m_cubes[1].transform.position;

            Vector3 dir = (c2 - c1).normalized;
            Vector3 camDir = (manager.m_arCamera.transform.position - c1).normalized;

            // ambil cross product untuk vector tegak lurus bidang
            Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
            if (Vector3.Dot(perp, camDir) < 0) perp = -perp; // pilih arah sesuai kamera

            float length = Vector3.Distance(c1, c2);

            m_cubes[2].transform.position = c2 + perp * length;
            m_cubes[3].transform.position = c1 + perp * length;
        }

        // Label selalu ngadep kamera
        if (manager.m_arCamera != null)
        {
            foreach (var label in m_labels)
            {
                if (label != null)
                {
                    label.transform.rotation = Quaternion.LookRotation(
                        label.transform.position - manager.m_arCamera.transform.position
                    );
                }
            }
        }
    }

    private void CreateRectangleLines()
    {
        // Bikin line permanen + label
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;

            GameObject lineObj = new GameObject($"Line_{i}");
            var line = lineObj.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startWidth = line.endWidth = 0.01f;
            line.positionCount = 2;
            line.SetPosition(0, m_cubes[i].transform.position);
            line.SetPosition(1, m_cubes[next].transform.position);

            manager.RegisterObject(lineObj);
            m_lines.Add(line);

            var label = GameObject.Instantiate(manager.m_distanceUIPrefab, lineObj.transform);
            UpdateLabel(label, m_cubes[i].transform.position, m_cubes[next].transform.position);
            m_labels.Add(label);
            manager.RegisterObject(label);
        }
    }

    private void UpdateLabel(GameObject label, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        var text = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
            text.text = $"{distance:F2} m";

        Vector3 mid = (start + end) / 2;
        label.transform.position = mid + Vector3.up * 0.05f;
    }

    private void ResetState()
    {
        m_cubes.Clear();
        m_lines.Clear();
        m_labels.Clear();
        m_stage = 0;
    }
}
