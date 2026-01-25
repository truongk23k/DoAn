using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("Cover points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>();
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private float zOffset = 1f;

    /*private void Start()
    {
        CreateCoverPoints();
    }*/

    [ContextMenu("Create Cover Points")]
    private void CreateCoverPoints()
    {
        // Xóa các cover points c? tr??c khi t?o m?i
        ClearCoverPoints();

        Vector3[] localCoverPoints =
        {
            new Vector3(0f, yOffset, zOffset),       // Front 
            new Vector3(0f, yOffset, -zOffset),      // Back 
            new Vector3(xOffset, yOffset, 0f),       // Right
            new Vector3(-xOffset, yOffset, 0f)     // Left
        };

        foreach (Vector3 localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint);
            GameObject coverPointObj = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform);
            coverPointObj.name = $"CoverPoint_{coverPoints.Count}";
            CoverPoint coverPoint = coverPointObj.GetComponent<CoverPoint>();
            if (coverPoint != null)
            {
                coverPoints.Add(coverPoint);
            }
        }

        Debug.Log($"Created {coverPoints.Count} cover points for {gameObject.name}");
    }

    [ContextMenu("Clear Cover Points")]
    private void ClearCoverPoints()
    {
        // Xóa các cover points trong scene
        for (int i = coverPoints.Count - 1; i >= 0; i--)
        {
            if (coverPoints[i] != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(coverPoints[i].gameObject);
#else
                Destroy(coverPoints[i].gameObject);
#endif
            }
        }
        coverPoints.Clear();

        Debug.Log($"Cleared all cover points for {gameObject.name}");
    }

    public List<CoverPoint> GetValidCoverPoints(Transform enemy)
    {
        List<CoverPoint> validCoverPoints = new List<CoverPoint>();

        foreach (CoverPoint cp in coverPoints)
        {
            if (cp != null && IsValidCoverPoint(cp, enemy))
            {
                validCoverPoints.Add(cp);
            }
        }

        return validCoverPoints;
    }

    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemy)
    {
        if(coverPoint.occupied)
            return false;

        if (!IsFutherestFromPlayer(coverPoint))
            return false;

        if (IsCoverCloseToPlayer(coverPoint))
            return false;

        if (IsCoverBehindPlayer(coverPoint, enemy))
            return false;

        if (IsCoverCloseToLastCover(coverPoint, enemy))
            return false;

        return true;
    }

    private bool IsFutherestFromPlayer(CoverPoint coverPoint)
    {
        CoverPoint futherestPoint = null;
        float futherestDistance = 0;

        foreach (CoverPoint point in coverPoints)
        {
            float distanceToPlayer = Vector3.Distance(point.transform.position, Player.instance.transform.position);
            if (distanceToPlayer > futherestDistance)
            {
                futherestDistance = distanceToPlayer;
                futherestPoint = point;
            }
        }

        return futherestPoint == coverPoint;
    }

    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemyTransform)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, Player.instance.transform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTransform.position);
        return distanceToPlayer < distanceToEnemy;
    }

    private bool IsCoverCloseToPlayer(CoverPoint coverPoint)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, Player.instance.transform.position);
        return distanceToPlayer < 2f;
    }

    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<Enemy_Range>().lastCover;
        return lastCover != null && Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3f;
    }
}
