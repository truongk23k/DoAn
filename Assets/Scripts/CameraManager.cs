using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    private float targetCameraDistance;
    [SerializeField] private float distanceChangeRate;

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
        if (Mathf.Abs(targetCameraDistance - distanceChangeRate) < 0.1f)
            return;

        transposer.m_CameraDistance = Mathf.Lerp(transposer.m_CameraDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;

}
