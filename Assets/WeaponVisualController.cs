using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Transform[] gunTransforms;

    private Transform currentGun;
    [Header("Left hand IK")]
    [SerializeField] private Transform leftHand;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
        SwitchOn(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(1);
            SwitchAnimationLayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(2);
            SwitchAnimationLayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(3);
            SwitchAnimationLayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(4);
            SwitchAnimationLayer(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(5);
            SwitchAnimationLayer(3);
        }

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

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }
}
