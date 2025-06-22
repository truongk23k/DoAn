using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControlls controls;

    [Header("Aim visual - laser")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim control")]
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisely;
    [SerializeField] private bool isLockingToTarget;

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
        if (Input.GetKeyDown(KeyCode.P))
            isAimingPrecisely = !isAimingPrecisely;

        if(Input.GetKeyDown(KeyCode.L))
            isLockingToTarget = !isLockingToTarget;

        UpdateAimLaer();

        UpdateAimPosition();

        UpdateCameraPosition();
    }

    private void UpdateAimLaer()
    {
        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLenght = 0.5f;
        float laserDistance = 4f;

        Vector3 endPoint = gunPoint.position + laserDirection * laserDistance;

        if(Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, laserDistance))
        {
            endPoint = hit.point;
            laserTipLenght = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLenght);
    }

    public Transform Target()
    {
        Transform target = null;

        if (GetMouseHitInfor().transform.GetComponent<Target>() != null)
            target = GetMouseHitInfor().transform;

        return target;
    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (target != null && isLockingToTarget)
        {
            aim.position = target.position;
            return;
        }

        aim.position = GetMouseHitInfor().point;

        if (!isAimingPrecisely)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }

    public bool CanAimPrecisely()
    {
        return isAimingPrecisely;
    }

    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;
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
