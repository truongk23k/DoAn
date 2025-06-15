using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;

    private Transform currentGun;
    [Header("Left hand IK")]
    [SerializeField] private Transform leftHand;

    private void Start()
    {
        SwitchOn(1);
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
        currentGun = gunTransforms[num - 1];

        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        foreach (Transform transform in gunTransforms)
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        leftHand.localPosition = targetTransform.localPosition;
        leftHand.localRotation = targetTransform.localRotation;
    }
}
