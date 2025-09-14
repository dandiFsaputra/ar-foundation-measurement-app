using UnityEngine;

/// <summary>
/// Base class (abstract) untuk semua mode pengukuran.
/// semua mode akan inheritensi dari class ini supaya punya fungsi yang sama
/// </summary>
public abstract class BaseMeasureMode
{
    protected MeasureManager manager; // Reference ke MeasureManager

    public BaseMeasureMode(MeasureManager manager)
    {
        this.manager = manager;
    }

    /// <summary>
    /// dipanggil saat mode dipilih
    /// </summary>
    public virtual void OnEnterMode() { }

    /// <summary>
    /// dipanggil saat mode diganti
    /// </summary>
    public virtual void OnExitMode() { }

    /// <summary>
    /// dipanggil saat user tap di layar
    /// </summary>
    /// <param name="position"></param>
    public abstract void OnTap(Vector3 worldPos);

    /// <summary>
    /// dipanggil di update untuk behavior yang perlu diupdate terus menerus
    /// </summary>
    public abstract void OnUpdate();
}
