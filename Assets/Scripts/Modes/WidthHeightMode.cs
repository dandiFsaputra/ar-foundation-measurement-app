using UnityEngine;

/// <summary>
/// mode untuk mengukur lebar dan tinggi (cuma 2 titik dan 1 garis)
///</summary>>
public class WidthHeightMode : BaseMeasureMode
{
    private GameObject m_startCube;
    private GameObject m_endCube;
    private LineRenderer m_lineRenderer;
    private GameObject m_label;

    public WidthHeightMode(MeasureManager manager) : base(manager)
    {
    }

    public override void OnEnterMode()
    {
        Debug.Log("Enter WidthHeight Mode");
    }

    public override void OnExitMode()
    {
        Debug.Log("Exit WidthHeight Mode");
        if (m_lineRenderer != null) GameObject.Destroy(m_lineRenderer.gameObject);
        if (m_label != null) GameObject.Destroy(m_label);
        m_startCube = null;
        m_endCube = null;
    }

    public override void OnTap(Vector3 position)
    {
        if (m_startCube == null)
        {
            // place start cube
            m_startCube = GameObject.Instantiate(manager.m_cubePrefab, position, Quaternion.identity);
        }
        else if (m_endCube == null)
        {
            // place end cube and create line
            m_endCube = GameObject.Instantiate(manager.m_cubePrefab, position, Quaternion.identity);
            GameObject lineObj = new GameObject("WidthHeightLine");
            m_lineRenderer = lineObj.AddComponent<LineRenderer>();
            m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_lineRenderer.startWidth = m_lineRenderer.endWidth = 0.01f;
            m_lineRenderer.positionCount = 2;

            m_label = GameObject.Instantiate(manager.m_distanceUIPrefab, m_lineRenderer.transform);
        }
        else
        {
            // reset
            OnExitMode();
            OnTap(position);
        }
    }

    public override void OnUpdate()
    {
        if (m_startCube != null && m_endCube != null && m_lineRenderer != null)
        {
            // update line
            Vector3 start = m_startCube.transform.position;
            Vector3 end = m_endCube.transform.position;
            m_lineRenderer.SetPosition(0, start);
            m_lineRenderer.SetPosition(1, end);

            // update label
            float distance = Vector3.Distance(start, end);
            var text = m_label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{distance:F2} m";
            }

            Vector3 mid = (start + end) / 2;
            m_label.transform.position = mid + Vector3.up * 0.1f;

            if (manager.m_arCamera != null)
            {
                m_label.transform.rotation = Quaternion.LookRotation(m_label.transform.position - manager.m_arCamera.transform.position);
            }
        }
    }

}
