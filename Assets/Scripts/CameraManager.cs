using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance)
            return;

        if (Mathf.Abs(targetCameraDistance - distanceChangeRate) < 0.1f)
            return;

        transposer.m_CameraDistance = Mathf.Lerp(transposer.m_CameraDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;

}
