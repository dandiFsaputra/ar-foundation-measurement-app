using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.Mathematics;

public class SessionManager : MonoBehaviour
{
   public static SessionManager Instance { get; private set; }
   [SerializeField] private GameObject uiScanningPanel;
   [SerializeField] private TextMeshProUGUI textInfo;
   [SerializeField] private int requiredPlaneCount;

   [Header("Camera Speed Settings")]
   [SerializeField] private float speedThreshold = 1.5f;
   [SerializeField] private float rotationThreshold = 100f;
   [SerializeField] private float showDuration = 1.5f;

   public bool IsReadyToPlace { get; set; }
   public ARPlaneManager ArPlaneManager { get; set; }

   private Vector3 lasPosition;
   private quaternion lastRotation;
   private float timer;

   void Awake()
   {
      Instance = this;
      ArPlaneManager = FindAnyObjectByType<ARPlaneManager>();
   }

   void Start()
   {
      HandleUiInfoAr();

      if (Camera.main != null)
      {
         lasPosition = Camera.main.transform.position;
         lastRotation = Camera.main.transform.rotation;
      }
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

   void Update()
   {
      if (Camera.main == null) return;

      Transform cam = Camera.main.transform;
      float speed = (cam.position - lasPosition).magnitude / Time.deltaTime;

      float angle = Quaternion.Angle(cam.rotation, lastRotation);
      float angularSpeed = angle / Time.deltaTime;

      if (speed > speedThreshold || angularSpeed > rotationThreshold)
      {
         textInfo.text = "Pergerakan kamera terlalu cepat, coba gerakan perlahan";
         timer = showDuration;
      }

      if (timer > 0)
      {
         timer -= Time.deltaTime;
         if (timer <= 0)
         {
            HandleUiInfoAr();
         }
      }
      lasPosition = cam.position;
      lastRotation = cam.rotation;
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
