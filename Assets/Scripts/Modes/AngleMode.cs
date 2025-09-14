using UnityEngine;

/// <summary>
/// AngleMode: pilih 3 titik (A, B, C), gambar garis AB & BC,
/// gambar lengkungan (arc) di sudut di titik B, dan tampilkan label
/// sudut di tengah arc yang selalu menghadap kamera.
/// </summary>
public class AngleMode : BaseMeasureMode
{
    // titik-titik
    private GameObject pA;
    private GameObject pB;
    private GameObject pC;

    // garis
    private LineRenderer lineAB;
    private LineRenderer lineBC;

    // arc renderer
    private LineRenderer arcRenderer;
    private float arcRadius = 0.2f;    // jarak arc dari titik B (atur sesuai kebutuhan)

    // label UI untuk menampilkan derajat sudut
    private GameObject angleLabel;

    public AngleMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter Angle Mode");
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit Angle Mode");

        // cukup reset state aja
        pA = pB = pC = null;
        lineAB = lineBC = arcRenderer = null;
        angleLabel = null;
    }

    /// <summary>
    /// Dipanggil saat user tap (dengan worldPos dari PlaceManager)
    /// </summary>
    public override void OnTap(Vector3 worldPos)
    {
        if (pA == null)
        {
            // Titik A
            pA = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(pA);
            return;
        }

        if (pB == null)
        {
            // Titik B
            pB = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(pB);

            lineAB = CreateLine("Line_AB");
            UpdateLinePositions(lineAB, pA.transform.position, pB.transform.position);
            return;
        }

        if (pC == null)
        {
            // Titik C
            pC = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
            manager.RegisterObject(pC);

            lineBC = CreateLine("Line_BC");
            UpdateLinePositions(lineBC, pB.transform.position, pC.transform.position);

            // Buat arc + label
            DrawArcAndLabel();
            return;
        }

        // --- Kalau sudah ada 3 titik ---
        // Reset state untuk set berikutnya
        pA = GameObject.Instantiate(manager.m_cubePrefab, worldPos, Quaternion.identity);
        manager.RegisterObject(pA);

        pB = null;
        pC = null;
        lineAB = null;
        lineBC = null;
        arcRenderer = null;
        angleLabel = null;
    }

    /// <summary>
    /// Update runtime: tarik garis sementara dari titik terakhir ke pose AR saat user belum men-tap titik berikutnya
    /// </summary>
    public override void OnUpdate()
    {
        // jika cuma pA ada -> tarik garis sementara AB ke pose AR
        if (pA != null && pB == null)
        {
            Vector3 cur = PlaceManager.Instance.CurrentPose.position;
            if (lineAB == null)
            {
                lineAB = CreateLine("Temp_LineAB");
            }
            UpdateLinePositions(lineAB, pA.transform.position, cur);
        }
        // jika pB ada dan pC belum -> tarik garis sementara BC
        else if (pB != null && pC == null)
        {
            Vector3 cur = PlaceManager.Instance.CurrentPose.position;
            if (lineBC == null)
            {
                lineBC = CreateLine("Temp_LineBC");
            }
            UpdateLinePositions(lineBC, pB.transform.position, cur);
        }

        // selalu menghadap kamera
        if (manager.m_arCamera != null && angleLabel != null)
        {
            angleLabel.transform.forward = manager.m_arCamera.transform.forward;
        }
    }

    // ---------------- Helpers ----------------

    // buat LineRenderer default
    private LineRenderer CreateLine(string name)
    {
        GameObject go = new GameObject(name);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lr.endWidth = 0.01f;
        lr.positionCount = 2;
        lr.numCapVertices = 4;

        // REGISTER ke manager supaya ikut kehapus saat reset
        manager.RegisterObject(go);

        return lr;
    }

    private void UpdateLinePositions(LineRenderer lr, Vector3 start, Vector3 end)
    {
        if (lr == null) return;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    // gambar arc antara BA -> BC di sekitar titik B
    private void DrawArcAndLabel()
    {
        if (pA == null || pB == null || pC == null) return;

        Vector3 A = pA.transform.position;
        Vector3 B = pB.transform.position;
        Vector3 C = pC.transform.position;

        Vector3 dir1 = (A - B).normalized; // dari B ke A
        Vector3 dir2 = (C - B).normalized; // dari B ke C

        // jika segaris -> gak bisa gambar arc
        float angle = Vector3.Angle(dir1, dir2); // 0..180
        if (angle < 1f) return;

        // axis rotasi
        Vector3 axis = Vector3.Cross(dir1, dir2).normalized;
        if (axis.magnitude < 0.001f) axis = Vector3.up;

        // buat/bersihkan arcRenderer
        if (arcRenderer != null) GameObject.Destroy(arcRenderer.gameObject);
        GameObject arcObj = new GameObject("AngleArc");
        arcRenderer = arcObj.AddComponent<LineRenderer>();
        arcRenderer.material = new Material(Shader.Find("Sprites/Default"));
        arcRenderer.startWidth = arcRenderer.endWidth = 0.008f;

        // register arc
        manager.RegisterObject(arcObj);

        // jumlah segmen dinamis berdasarkan sudut
        int seg = Mathf.Clamp(Mathf.CeilToInt(angle / 4f), 8, 64);
        arcRenderer.positionCount = seg + 1;

        // tentukan radius
        float r = arcRadius;

        // rotate dari dir1 ke dir2
        for (int i = 0; i <= seg; i++)
        {
            float t = (float)i / seg;
            float stepAngle = t * angle; // in degrees
            Vector3 rotated = Quaternion.AngleAxis(stepAngle, axis) * dir1;
            Vector3 pos = B + rotated * r;
            arcRenderer.SetPosition(i, pos);
        }

        // label: tempatkan di titik tengah arc
        float midAngle = angle * 0.5f;
        Vector3 midDir = Quaternion.AngleAxis(midAngle, axis) * dir1;
        Vector3 midPos = B + midDir * r;

        if (angleLabel != null) GameObject.Destroy(angleLabel);
        angleLabel = GameObject.Instantiate(manager.m_distanceUIPrefab, midPos, Quaternion.identity);

        // register label
        manager.RegisterObject(angleLabel);

        var text = angleLabel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
            text.text = $"{angle:F1}°";

        // biar panel/label menghadap camera
        if (manager.m_arCamera != null)
            angleLabel.transform.rotation = Quaternion.LookRotation(angleLabel.transform.position - manager.m_arCamera.transform.position);
    }
}
