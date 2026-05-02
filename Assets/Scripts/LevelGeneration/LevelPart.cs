using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection check")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckColliders;
    [SerializeField] private Transform intersectionCheckParent;

    [ContextMenu("Set static to environment layer")]
    private void AdjustLayerForStaticObjects()
    {
        foreach(Transform childTrans in transform.GetComponentsInChildren<Transform>(true))
        {
            childTrans.gameObject.layer = LayerMask.NameToLayer("Environment");
        }
    }

    private void Start()
    {
        if(intersectionCheckColliders.Length == 0)
            intersectionCheckColliders = intersectionCheckParent.GetComponentsInChildren<Collider>();
    }

    public bool IntersectionDetected()
    {
        Physics.SyncTransforms();

        foreach (var collider in intersectionCheckColliders)
        {
            Collider[] hitColliders =
                Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, intersectionLayer);
            
            foreach(var hit in hitColliders)
            {
                IntersectionCheck intersectionCheck = hit.GetComponentInParent<IntersectionCheck>();

                if (intersectionCheck != null && intersectionCheckParent != intersectionCheck.transform)
                    return true;
            }
        }

        return false;
    }

    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntranceSnapPoint();

        AlignTo(entrancePoint, targetSnapPoint);
        SnapTo(entrancePoint, targetSnapPoint);

    }

    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        var rotationOffset = ownSnapPoint.transform.eulerAngles.y - transform.eulerAngles.y;

        transform.rotation = targetSnapPoint.transform.rotation;

        transform.Rotate(0, 180, 0);
        transform.Rotate(0, -rotationOffset, 0);
    }

    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        var offset = transform.position - ownSnapPoint.transform.position;

        var newPosition = targetSnapPoint.transform.position + offset;

        transform.position = newPosition;
    }

    public SnapPoint GetEntranceSnapPoint() => GetSnapPointOfType(SnapPointType.Enter);

    public SnapPoint GetExitSnapPoint() => GetSnapPointOfType(SnapPointType.Exit);

    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.pointType == pointType)
                filteredSnapPoints.Add(snapPoint);
        }

        if (filteredSnapPoints.Count == 0)
        {
            return null;
        }
        return filteredSnapPoints[Random.Range(0, filteredSnapPoints.Count)];
    }

    public Enemy[] MyEnemies() => GetComponentsInChildren<Enemy>(true);
}
