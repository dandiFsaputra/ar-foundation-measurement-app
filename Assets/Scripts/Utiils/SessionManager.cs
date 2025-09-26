using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class SessionManager : MonoBehaviour
{
   public static SessionManager Instance { get; private set; }
   [SerializeField] private GameObject uiScanningPanel;
   [SerializeField] private TextMeshProUGUI textInfo;
   [SerializeField] private int requiredPlaneCount;
   public bool IsReadyToPlace { get; set; }



   public ARPlaneManager ArPlaneManager { get; set; }

   void Awake()
   {
      Instance = this;
      ArPlaneManager = FindAnyObjectByType<ARPlaneManager>();
   }

   void Start()
   {
      HandleUiInfoAr();
   }

   void OnEnable()
   {
      ArPlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
   }

   void OnDisable()
   {
      ArPlaneManager.trackablesChanged.RemoveListener(OnPlanesChanged);
   }

   private void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
   {
      HandleUiInfoAr();
   }

   private void HandleUiInfoAr()
   {
      if (ArPlaneManager.trackables.count > requiredPlaneCount)
      {
         IsReadyToPlace = true;

         uiScanningPanel.SetActive(false);
         textInfo.gameObject.SetActive(false);
      }
      else
      {
         IsReadyToPlace = false;

         uiScanningPanel.SetActive(true);
         textInfo.gameObject.SetActive(true);
         textInfo.text = "Arahkan ponsel perlahan untuk mendeteksi permukaan";
      }
   }


}
