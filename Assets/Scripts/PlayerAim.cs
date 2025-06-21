using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControlls controls;

    [Header("Aim control")]
    [SerializeField] private Transform aim;

    [Header("Camera control")]
    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1)]
    [SerializeField] private float minCameraDistance = 1f;
    [Range(1f, 3f)]
    [SerializeField] private float maxCameraDistance = 3f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensitivity = 5;

    [Space]
    [SerializeField] private LayerMask aimLayerMask;
    private RaycastHit lastKnownMouseHit;

    private Vector2 aimInput;

    private void Start()
    {
        player = GetComponent<Player>();

        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = new Vector3(GetMouseHitInfor().point.x, transform.position.y + 1, GetMouseHitInfor().point.z);
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;
        Debug.Log(actualMaxCameraDistance);
        Vector3 desiredCameraPosition = GetMouseHitInfor().point;
        desiredCameraPosition.y = transform.position.y + 1;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(desiredCameraPosition, transform.position);

        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
        desiredCameraPosition = transform.position + aimDirection * clampedDistance;

        return desiredCameraPosition;
    }

    public RaycastHit GetMouseHitInfor()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfor, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfor;
            return hitInfor;
        }

        return lastKnownMouseHit;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }

}
