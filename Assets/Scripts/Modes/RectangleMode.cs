using System.Collections.Generic;
using UnityEngine;

public class RectangleMode : BaseMeasureMode
{
    private readonly List<GameObject> m_cubes = new();
    private readonly List<LineRenderer> m_lines = new();
    private readonly List<GameObject> m_labels = new();

    private int m_stage = 0;

    public RectangleMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter Rectangle Mode");
        ResetState();
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
            var cube1 = SpawnCube(worldPos);
            m_cubes.Add(cube1);

            m_stage = 1;
        }
        else if (m_stage == 1)
        {
            var cube2 = SpawnCube(worldPos);
            m_cubes.Add(cube2);

            var cube3 = SpawnCube(worldPos);
            var cube4 = SpawnCube(m_cubes[0].transform.position);

            m_cubes.Add(cube3);
            m_cubes.Add(cube4);

            m_stage = 2;
        }
        else if (m_stage == 2)
        {
            CreateRectangleLines();
            m_stage = 3;
        }
        else if (m_stage == 3)
        {
            ResetState();
            OnTap(worldPos);
        }
    }

    public override void OnUpdate()
    {
        if (m_stage == 1 && PlaceManager.Instance.HasValidPos)
        {
            Debug.DrawLine(m_cubes[0].transform.position, PlaceManager.Instance.CurrentPose.position, Color.yellow);
        }

        if (m_stage == 2 && PlaceManager.Instance.HasValidPos)
        {
            UpdateDynamicCubes();
        }

        foreach (var label in m_labels) FaceToCamera(label);
    }

    // ---------- SPAWN & UPDATE ----------

    private GameObject SpawnCube(Vector3 pos)
    {
        var cube = GameObject.Instantiate(manager.m_cubePrefab, pos, Quaternion.identity);
        manager.RegisterObject(cube);
        return cube;
    }

    private void CreateRectangleLines()
    {
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;

            var line = CreateLine($"Line_{i}");
            UpdateLine(line, m_cubes[i].transform.position, m_cubes[next].transform.position);
            m_lines.Add(line);

            var label = CreateLabel(Vector3.zero, "0.00 m");
            UpdateLabel(label, m_cubes[i].transform.position, m_cubes[next].transform.position);
            m_labels.Add(label);
        }
    }

    private void UpdateDynamicCubes()
    {
        Vector3 c1 = m_cubes[0].transform.position;
        Vector3 c2 = m_cubes[1].transform.position;

        Vector3 dir = (c2 - c1).normalized;
        Vector3 camDir = (manager.m_arCamera.transform.position - c1).normalized;

        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
        if (Vector3.Dot(perp, camDir) < 0) perp = -perp;

        float length = Vector3.Distance(c1, c2);

        m_cubes[2].transform.position = c2 + perp * length;
        m_cubes[3].transform.position = c1 + perp * length;
    }

    // ---------- RESET ----------

    private void ResetState()
    {
        m_cubes.Clear();
        m_lines.Clear();
        m_labels.Clear();
        m_stage = 0;
    }
}
