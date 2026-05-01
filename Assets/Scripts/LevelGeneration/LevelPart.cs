using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{

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
            Debug.LogError("No snap points of type " + pointType + " found in level part " + name);
            return null;
        }
        return filteredSnapPoints[Random.Range(0, filteredSnapPoints.Count)];
    }
}
