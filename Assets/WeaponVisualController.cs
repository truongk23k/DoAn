using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchOn(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchOn(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchOn(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchOn(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchOn(5);

    }

    private void SwitchOn(int num)
    {
        SwitchOffGuns();
        gunTransforms[num - 1].gameObject.SetActive(true);
    }

    private void SwitchOffGuns()
    {
        foreach (Transform transform in gunTransforms)
        {
            transform.gameObject.SetActive(false);
        }
    }
}
