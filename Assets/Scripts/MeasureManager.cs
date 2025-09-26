using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// MeasureManager = class untuk mengelola pengukuran jarak di AR + New Input System
/// </summary>
public class MeasureManager : MonoBehaviour
{
    [Header("Measure Manager Settings")]
    public GameObject m_cubePrefab; // Prefab untuk menandai titik awal dan akhir pengukuran
    public GameObject m_distanceUIPrefab; // Prefab untuk menampilkan ui angka jarak
    public Camera m_arCamera; // Kamera AR untuk orientasi UI

    private PlayerInputActions m_playerInputActions; // Input actions untuk menangani input pengguna (tap)
    private BaseMeasureMode currentMode; // Mode pengukuran saat ini

    // Simpan semua cube & line yg dibuat dari semua mode
    public List<GameObject> spawnedObjects = new List<GameObject>();


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
        // --- IGNORE TAP WHEN POINTER IS ON UI ---
        // Untuk mouse/editor:
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Untuk touch (mobile): cek setiap touch apakah berada pada UI
        if (EventSystem.current != null && Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return;
            }
        }
        // ------------------------------------------

        // pastikan ada mode aktif dan ada posisi valid dari PlaceManager
        if (currentMode != null && PlaceManager.Instance != null && PlaceManager.Instance.HasValidPos && SessionManager.Instance.IsReadyToPlace == true)
        {
            Vector3 worldPos = PlaceManager.Instance.CurrentPose.position;
            currentMode.OnTap(worldPos);
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

    /// <summary>
    /// Fungsi untuk mendaftarkan object baru (cube/line) ke daftar
    /// </summary>
    public void RegisterObject(GameObject obj)
    {
        if (!spawnedObjects.Contains(obj))
            spawnedObjects.Add(obj);
    }

    /// <summary>
    /// Fungsi untuk hapus semua cube & line
    /// </summary>
    public void ResetAll()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}