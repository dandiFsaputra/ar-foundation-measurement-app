using UnityEngine;

/// <summary>
/// Base class (abstract) untuk semua mode pengukuran.
/// semua mode akan inheritensi dari class ini supaya punya fungsi yang sama
/// </summary>
public abstract class BaseMeasureMode
{
    protected MeasureManager manager;

    public BaseMeasureMode(MeasureManager manager)
    {
        this.manager = manager;
    }

    public virtual void OnEnterMode() { }
    public virtual void OnExitMode() { }
    public abstract void OnTap(Vector3 worldPos);
    public abstract void OnUpdate();

    // ---------------- Helpers ----------------

    protected LineRenderer CreateLine(string name, float width = 0.01f)
    {
        GameObject go = new GameObject(name);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lr.endWidth = width;
        lr.numCapVertices = 4;
        lr.positionCount = 2;

        manager.RegisterObject(go);
        return lr;
    }

    protected void UpdateLine(LineRenderer lr, Vector3 start, Vector3 end)
    {
        if (lr == null) return;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    protected GameObject CreateLabel(Vector3 pos, string textValue)
    {
        GameObject label = GameObject.Instantiate(manager.m_distanceUIPrefab, pos, Quaternion.identity);
        manager.RegisterObject(label);

        var text = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
            text.text = textValue;

        return label;
    }

    protected void UpdateLabel(GameObject label, Vector3 start, Vector3 end, string format = "{0:F2} m")
    {
        if (label == null) return;

        float distance = Vector3.Distance(start, end);
        var text = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
            text.text = string.Format(format, distance);

        Vector3 mid = (start + end) / 2;
        label.transform.position = mid + Vector3.up * 0.05f;

        FaceToCamera(label);
    }

    protected void FaceToCamera(GameObject go)
    {
        if (go == null || manager.m_arCamera == null) return;
        go.transform.rotation = Quaternion.LookRotation(go.transform.position - manager.m_arCamera.transform.position);
    }
}
