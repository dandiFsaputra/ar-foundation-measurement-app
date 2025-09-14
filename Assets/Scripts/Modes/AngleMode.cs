using UnityEngine;

public class AngleMode : BaseMeasureMode
{
    private GameObject pA;
    private GameObject pB;
    private GameObject pC;

    private LineRenderer lineAB;
    private LineRenderer lineBC;
    private LineRenderer arcRenderer;

    private GameObject angleLabel;

    private float arcRadius = 0.2f;

    public AngleMode(MeasureManager manager) : base(manager) { }

    public override void OnEnterMode()
    {
        Debug.Log("Enter Angle Mode");
        ResetState();
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit Angle Mode");
        ResetState();
    }

    public override void OnTap(Vector3 worldPos)
    {
        if (pA == null)
        {
            pA = SpawnCube(worldPos);
            return;
        }

        if (pB == null)
        {
            pB = SpawnCube(worldPos);
            lineAB = CreateLine("Line_AB");
            UpdateLine(lineAB, pA.transform.position, pB.transform.position);
            return;
        }

        if (pC == null)
        {
            pC = SpawnCube(worldPos);
            lineBC = CreateLine("Line_BC");
            UpdateLine(lineBC, pB.transform.position, pC.transform.position);

            DrawArcAndLabel();
            return;
        }

        ResetState();
        pA = SpawnCube(worldPos);
    }

    public override void OnUpdate()
    {
        if (PlaceManager.Instance.HasValidPos)
        {
            Vector3 cur = PlaceManager.Instance.CurrentPose.position;

            if (pA != null && pB == null)
            {
                if (lineAB == null) lineAB = CreateLine("Temp_LineAB");
                UpdateLine(lineAB, pA.transform.position, cur);
            }
            else if (pB != null && pC == null)
            {
                if (lineBC == null) lineBC = CreateLine("Temp_LineBC");
                UpdateLine(lineBC, pB.transform.position, cur);
            }
        }

        if (angleLabel != null) FaceToCamera(angleLabel);
    }

    // ---------------- Helpers ----------------
    private GameObject SpawnCube(Vector3 pos)
    {
        var cube = GameObject.Instantiate(manager.m_cubePrefab, pos, Quaternion.identity);
        manager.RegisterObject(cube);
        return cube;
    }

    private void DrawArcAndLabel()
    {
        if (pA == null || pB == null || pC == null) return;

        Vector3 A = pA.transform.position;
        Vector3 B = pB.transform.position;
        Vector3 C = pC.transform.position;

        Vector3 dir1 = (A - B).normalized;
        Vector3 dir2 = (C - B).normalized;

        float angle = Vector3.Angle(dir1, dir2);
        if (angle < 1f) return;

        Vector3 axis = Vector3.Cross(dir1, dir2).normalized;
        if (axis.magnitude < 0.001f) axis = Vector3.up;

        if (arcRenderer != null) GameObject.Destroy(arcRenderer.gameObject);
        var arcObj = new GameObject("AngleArc");
        arcRenderer = arcObj.AddComponent<LineRenderer>();
        arcRenderer.material = new Material(Shader.Find("Sprites/Default"));
        arcRenderer.startWidth = arcRenderer.endWidth = 0.008f;
        manager.RegisterObject(arcObj);

        int seg = Mathf.Clamp(Mathf.CeilToInt(angle / 4f), 8, 64);
        arcRenderer.positionCount = seg + 1;

        for (int i = 0; i <= seg; i++)
        {
            float t = i / (float)seg;
            float stepAngle = t * angle;
            Vector3 rotated = Quaternion.AngleAxis(stepAngle, axis) * dir1;
            arcRenderer.SetPosition(i, B + rotated * arcRadius);
        }

        Vector3 midDir = Quaternion.AngleAxis(angle * 0.5f, axis) * dir1;
        Vector3 midPos = B + midDir * arcRadius;

        if (angleLabel != null) GameObject.Destroy(angleLabel);
        angleLabel = CreateLabel(midPos, $"{angle:F1}°");
    }

    private void ResetState()
    {
        pA = pB = pC = null;
        lineAB = lineBC = arcRenderer = null;
        angleLabel = null;
    }
}
