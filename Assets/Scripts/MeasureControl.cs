using UnityEngine;

public class MeasureControl : MonoBehaviour
{
    [SerializeField] private MeasureManager measureManager;

    // Tombol reset semua
    public void ResetAll()
    {
        if (measureManager == null) return;

        // Hapus semua cube
        foreach (var cube in measureManager.m_cubes)
        {
            if (cube != null) Destroy(cube);
        }
        measureManager.m_cubes.Clear();

        // Hapus semua line
        foreach (var line in measureManager.m_lines)
        {
            if (line != null) Destroy(line.gameObject);
        }
        measureManager.m_lines.Clear();

        // Hapus semua label
        foreach (var label in measureManager.m_labels)
        {
            if (label != null) Destroy(label);
        }
        measureManager.m_labels.Clear();

        // Reset state segment
        measureManager.StartNewSegment();
    }

    // Tombol stop - memulai segment baru
    public void StopDrawing()
    {
        if (measureManager == null) return;

        // Mulai segment baru tanpa menghapus cube/line yang sudah ada
        measureManager.StartNewSegment();
    }
}