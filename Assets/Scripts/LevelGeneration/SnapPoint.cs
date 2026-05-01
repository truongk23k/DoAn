using UnityEngine;

public enum SnapPointType
{
    Enter,
    Exit
}

public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType;

    public void OnValidate()
    {
        gameObject.name = " Snap Point - " + pointType.ToString();
    }
}
