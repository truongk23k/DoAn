using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Transform[] gunTransforms;

    private Transform currentGun;

    [Header("Rig")]
    [SerializeField] private float rigIncreaseStep;
    private bool rigShouldBeIncreased;

    [Header("Left hand IK")]
    [SerializeField] private Transform leftHand;

    private Rig rig;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        SwitchOn(1);

        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
            rig.weight = 0.15f;
        }

        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;
            
            if(rig.weight >= 1) 
                rigShouldBeIncreased = false;
        }
    }

    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true;

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
    private void CheckWeaponSwitch()
    {
        //note: swap gun -> no reload last gun
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
}
