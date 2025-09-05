using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeasureManager : MonoBehaviour
{
    [Header("Measure Manager Settings")]
    [SerializeField] private GameObject m_cubePrefab;
    [SerializeField] private GameObject m_distanceUIPrefab;  // prefab canvas world space (sudah ada TMP text di dalamnya)
    [SerializeField] private Camera m_arCamera;

    private PlayerInputActions m_playerInputActions;
    private List<GameObject> m_cubes = new List<GameObject>();
    private List<LineRenderer> m_lines = new List<LineRenderer>();
    private List<GameObject> m_labels = new List<GameObject>();
    private LineRenderer m_tempLine;
    private GameObject m_tempLabel;

    private void Awake()
    {
        m_playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        m_playerInputActions.Player.Enable();
        m_playerInputActions.Player.Tap.performed += OnTapPerformed;
    }

    private void OnDisable()
    {
        m_playerInputActions.Player.Tap.performed -= OnTapPerformed;
        m_playerInputActions.Player.Disable();
    }

    private void OnTapPerformed(InputAction.CallbackContext ctx)
    {
        PlaceCube();
    }

    private void PlaceCube()
    {
        if (!PlaceManager.Instance.HasValidPos) return;

        Pose pose = PlaceManager.Instance.CurrentPose;

        GameObject newCube = Instantiate(m_cubePrefab, pose.position, Quaternion.identity);
        m_cubes.Add(newCube);

        if (m_cubes.Count > 1)
        {
            GameObject lastCube = m_cubes[m_cubes.Count - 2];
            CreateLine(lastCube, newCube);
        }
    }

    private void CreateLine(GameObject start, GameObject end)
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.numCapVertices = 4;

        m_lines.Add(line);

        // langsung pakai prefab canvas world space kamu
        GameObject labelObj = Instantiate(m_distanceUIPrefab, lineObj.transform);
        m_labels.Add(labelObj);

        UpdateLine(line, start.transform.position, end.transform.position);
    }

    private void UpdateLine(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    private void Update()
    {
        // update semua line permanen
        for (int i = 0; i < m_lines.Count; i++)
        {
            LineRenderer line = m_lines[i];
            GameObject labelObj = m_labels[i];

            if (line == null || labelObj == null) continue;

            Vector3 start = m_cubes[i].transform.position;
            Vector3 end = m_cubes[i + 1].transform.position;

            UpdateLine(line, start, end);

            float distance = Vector3.Distance(start, end);

            TextMeshProUGUI textUi = labelObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textUi != null)
            {
                textUi.text = $"{distance:F2} m";
            }

            // posisi di tengah (garis sudah fix)
            Vector3 midPoint = (start + end) / 2;
            float offset = 0.10f;
            labelObj.transform.position = midPoint + Vector3.up * offset;

            if (m_arCamera != null)
            {
                labelObj.transform.rotation = Quaternion.LookRotation(
                    labelObj.transform.position - m_arCamera.transform.position
                );
            }
        }

        // --- garis sementara (masih ditarik) ---
        if (m_cubes.Count > 0 && PlaceManager.Instance.HasValidPos)
        {
            Vector3 start = m_cubes[m_cubes.Count - 1].transform.position;
            Vector3 end = PlaceManager.Instance.CurrentPose.position;

            if (m_tempLine == null)
            {
                GameObject tempLineObj = new GameObject("TempLine");
                m_tempLine = tempLineObj.AddComponent<LineRenderer>();
                m_tempLine.material = new Material(Shader.Find("Sprites/Default"));
                m_tempLine.startWidth = 0.01f;
                m_tempLine.endWidth = 0.01f;
                m_tempLine.positionCount = 2;
                m_tempLine.numCapVertices = 4;

                m_tempLabel = Instantiate(m_distanceUIPrefab, tempLineObj.transform);
            }

            UpdateLine(m_tempLine, start, end);

            float distance = Vector3.Distance(start, end);

            TextMeshProUGUI tempText = m_tempLabel.GetComponentInChildren<TextMeshProUGUI>();
            if (tempText != null)
            {
                tempText.text = $"{distance:F2} m";
            }

            // posisi di ujung (masih tarik)
            float offset = 0.10f;
            m_tempLabel.transform.position = end + Vector3.up * offset;

            if (m_arCamera != null)
            {
                m_tempLabel.transform.rotation = Quaternion.LookRotation(
                    m_tempLabel.transform.position - m_arCamera.transform.position
                );
            }
        }
    }
}
