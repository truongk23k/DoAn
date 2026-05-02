using UnityEngine;

public enum SnapPointType
{
    Enter,
    Exit
}

public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType;

    private void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnValidate()
    {
        gameObject.name = " Snap Point - " + pointType.ToString();
    }
}
