using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MeasureManager = class untuk mengelola pengukuran jarak di AR + New Input System
/// </summary>
public class MeasureManager : MonoBehaviour
{
    [Header("Measure Manager Settings")]
    [SerializeField] private GameObject m_cubePrefab; // Prefab untuk menandai titik awal dan akhir pengukuran
    [SerializeField] private GameObject m_distanceUIPrefab; // Prefab untuk menampilkan ui angka jarak
    [SerializeField] private Camera m_arCamera; // Kamera AR untuk orientasi UI

    private PlayerInputActions m_playerInputActions; // Input actions untuk menangani input pengguna (tap)
    private BaseMeasureMode currentMode; // Mode pengukuran saat ini

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
        if (PlaceManager.Instance.HasValidPos && currentMode != null)
        {
            Vector3 pos = PlaceManager.Instance.CurrentPose.position;
            currentMode.OnTap(pos);
        }
    }

    private void Update()
    {
        if (currentMode != null) currentMode.OnUpdate();
    }

    /// <summary>
    /// dipanggil untuk mengganti mode pengukuran   
    /// </summary>
    /// <param name="mode"></param>
    public void SetMode(BaseMeasureMode mode)
    {
        if (currentMode != null)
        {
            currentMode.OnExitMode();
        }

        currentMode = mode;

        if (currentMode != null)
        {
            currentMode.OnEnterMode();
        }
    }



    //[Header("Measure Manager Settings")]
    //[SerializeField] private GameObject m_cubePrefab;
    //[SerializeField] private GameObject m_distanceUIPrefab;
    //[SerializeField] private Camera m_arCamera;

    //private PlayerInputActions m_playerInputActions;
    //public List<GameObject> m_cubes = new List<GameObject>();
    //public List<LineRenderer> m_lines = new List<LineRenderer>();
    //public List<GameObject> m_labels = new List<GameObject>();
    //public LineRenderer m_tempLine;
    //public GameObject m_tempLabel;
    //public bool isDrawingEnabled = true;

    //// Variabel untuk mengelola segment
    //private GameObject m_currentSegmentStartCube; // Cube pertama di segment saat ini
    //private bool m_isNewSegment = true; // Flag untuk menandai segment baru

    //private void Awake()
    //{
    //    m_playerInputActions = new PlayerInputActions();
    //}

    //private void OnEnable()
    //{
    //    m_playerInputActions.Player.Enable();
    //    m_playerInputActions.Player.Tap.performed += OnTapPerformed;
    //}

    //private void OnDisable()
    //{
    //    m_playerInputActions.Player.Tap.performed -= OnTapPerformed;
    //    m_playerInputActions.Player.Disable();
    //}

    //private void OnTapPerformed(InputAction.CallbackContext ctx)
    //{
    //    PlaceCube();
    //}

    //private void PlaceCube()
    //{
    //    if (!PlaceManager.Instance.HasValidPos) return;

    //    Pose pose = PlaceManager.Instance.CurrentPose;
    //    GameObject newCube = Instantiate(m_cubePrefab, pose.position, Quaternion.identity);
    //    m_cubes.Add(newCube);

    //    // Jika ini adalah segment baru, set cube ini sebagai start untuk segment baru
    //    if (m_isNewSegment)
    //    {
    //        m_currentSegmentStartCube = newCube;
    //        m_isNewSegment = false;

    //        // Hapus garis sementara karena ini segment baru
    //        if (m_tempLine != null) Destroy(m_tempLine.gameObject);
    //        if (m_tempLabel != null) Destroy(m_tempLabel);
    //        m_tempLine = null;
    //        m_tempLabel = null;
    //    }
    //    else
    //    {
    //        // Buat garis permanen hanya jika bukan segment baru
    //        CreateLine(m_currentSegmentStartCube, newCube);
    //        m_currentSegmentStartCube = newCube; // Set cube terbaru sebagai start untuk garis berikutnya
    //    }

    //    // Aktifkan drawing sementara untuk garis berikutnya
    //    isDrawingEnabled = true;
    //}

    //private void CreateLine(GameObject start, GameObject end)
    //{
    //    GameObject lineObj = new GameObject("Line");
    //    LineRenderer line = lineObj.AddComponent<LineRenderer>();

    //    line.material = new Material(Shader.Find("Sprites/Default"));
    //    line.startWidth = 0.01f;
    //    line.endWidth = 0.01f;
    //    line.positionCount = 2;
    //    line.useWorldSpace = true;
    //    line.numCapVertices = 4;

    //    m_lines.Add(line);

    //    GameObject labelObj = Instantiate(m_distanceUIPrefab, lineObj.transform);
    //    m_labels.Add(labelObj);

    //    UpdateLine(line, start.transform.position, end.transform.position);
    //}

    //private void UpdateLine(LineRenderer line, Vector3 start, Vector3 end)
    //{
    //    line.SetPosition(0, start);
    //    line.SetPosition(1, end);
    //}

    //private void Update()
    //{
    //    // Update semua line permanen
    //    for (int i = 0; i < m_lines.Count; i++)
    //    {
    //        LineRenderer line = m_lines[i];
    //        GameObject labelObj = m_labels[i];

    //        if (line == null || labelObj == null) continue;

    //        // Cari cube start dan end untuk line ini
    //        // Asumsi: setiap line menghubungkan m_cubes[i] dan m_cubes[i+1]
    //        // (Ini perlu disesuaikan dengan logika pembuatan line Anda)
    //        if (i < m_cubes.Count - 1)
    //        {
    //            Vector3 start = m_cubes[i].transform.position;
    //            Vector3 end = m_cubes[i + 1].transform.position;

    //            UpdateLine(line, start, end);

    //            float distance = Vector3.Distance(start, end);

    //            TextMeshProUGUI textUi = labelObj.GetComponentInChildren<TextMeshProUGUI>();
    //            if (textUi != null)
    //                textUi.text = $"{distance:F2} m";

    //            Vector3 midPoint = (start + end) / 2;
    //            float offset = 0.10f;
    //            labelObj.transform.position = midPoint + Vector3.up * offset;

    //            if (m_arCamera != null)
    //            {
    //                labelObj.transform.rotation = Quaternion.LookRotation(
    //                    labelObj.transform.position - m_arCamera.transform.position
    //                );
    //            }
    //        }
    //    }

    //    // Garis sementara dari cube terakhir di segment saat ini
    //    if (isDrawingEnabled && m_currentSegmentStartCube != null && !m_isNewSegment && PlaceManager.Instance.HasValidPos)
    //    {
    //        Vector3 start = m_currentSegmentStartCube.transform.position;
    //        Vector3 end = PlaceManager.Instance.CurrentPose.position;

    //        if (m_tempLine == null)
    //        {
    //            GameObject tempLineObj = new GameObject("TempLine");
    //            m_tempLine = tempLineObj.AddComponent<LineRenderer>();
    //            m_tempLine.material = new Material(Shader.Find("Sprites/Default"));
    //            m_tempLine.startWidth = 0.01f;
    //            m_tempLine.endWidth = 0.01f;
    //            m_tempLine.positionCount = 2;
    //            m_tempLine.numCapVertices = 4;

    //            m_tempLabel = Instantiate(m_distanceUIPrefab, tempLineObj.transform);
    //        }

    //        UpdateLine(m_tempLine, start, end);

    //        float distance = Vector3.Distance(start, end);

    //        TextMeshProUGUI tempText = m_tempLabel.GetComponentInChildren<TextMeshProUGUI>();
    //        if (tempText != null)
    //            tempText.text = $"{distance:F2} m";

    //        float offsetLabel = 0.10f;
    //        m_tempLabel.transform.position = end + Vector3.up * offsetLabel;

    //        if (m_arCamera != null)
    //        {
    //            m_tempLabel.transform.rotation = Quaternion.LookRotation(
    //                m_tempLabel.transform.position - m_arCamera.transform.position
    //            );
    //        }
    //    }
    //}

    //// Method untuk memulai segment baru
    //public void StartNewSegment()
    //{
    //    m_isNewSegment = true;
    //    m_currentSegmentStartCube = null;
    //    isDrawingEnabled = false;

    //    // Hapus garis sementara
    //    if (m_tempLine != null) Destroy(m_tempLine.gameObject);
    //    if (m_tempLabel != null) Destroy(m_tempLabel);
    //    m_tempLine = null;
    //    m_tempLabel = null;
    //}
}