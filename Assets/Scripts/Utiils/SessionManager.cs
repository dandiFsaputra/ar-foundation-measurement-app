using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class SessionManager : MonoBehaviour
{
   [SerializeField] private GameObject uiScanningPanel;
   [SerializeField] private TextMeshProUGUI textInfo;
   [SerializeField] private int requiredPlaneCount = 2;

   private ARPlaneManager arPlaneManager;

   void Awake()
   {
      arPlaneManager = FindAnyObjectByType<ARPlaneManager>();
   }

   void Start()
   {
      HandleUiInfoAr();
   }

   void OnEnable()
   {
      arPlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
   }

   void OnDisable()
   {
      arPlaneManager.trackablesChanged.RemoveListener(OnPlanesChanged);
   }

   private void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
   {
      HandleUiInfoAr();
   }

   private void HandleUiInfoAr()
   {
      if (arPlaneManager.trackables.count > requiredPlaneCount)
      {
         uiScanningPanel.SetActive(false);
         textInfo.gameObject.SetActive(false);
      }
      else
      {
         uiScanningPanel.SetActive(true);
         textInfo.gameObject.SetActive(true);
         textInfo.text = "Arahkan ponsel perlahan untuk mendeteksi permukaan";
      }
   }
}
