using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaceManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private GameObject indicatorPlane;

    public static PlaceManager Instance { get; private set; }

    public bool HasValidPos { get; private set; }
    public Pose CurrentPose { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        bool isSessionReady = SessionManager.Instance != null && SessionManager.Instance.IsReadyToPlace;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        //titik tengah layar
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        //raycast dari titik tengah layar ke point cloud
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            CurrentPose = hits[0].pose;
            HasValidPos = true;

            if (isSessionReady && !indicatorPlane.activeSelf)
            {
                indicatorPlane.SetActive(true);
            }

            indicatorPlane.transform.SetPositionAndRotation(CurrentPose.position, CurrentPose.rotation);
        }
        else
        {
            //kalau raycast tidak kena, sembunyikan indicatorPlane
            if (indicatorPlane.activeSelf)
            {
                HasValidPos = false;
                indicatorPlane.SetActive(false);
            }
        }

        if (!isSessionReady && indicatorPlane.activeSelf)
        {
            indicatorPlane.SetActive(false);
            HasValidPos = false;
        }
    }
}
