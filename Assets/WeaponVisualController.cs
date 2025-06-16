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
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHand_Target;
    [SerializeField] private float leftHandIKIncreaseStep;
    private bool shouldIncreaseLeftHandIKWeight;

    private Rig rig;

    private bool busyGrabbingWeapon;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        SwitchOn(1);

        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && !busyGrabbingWeapon)
        {
            anim.SetTrigger("Reload");
            PauseRig();
        }

        UpdateRigWeight();

        UpdateLeftHandIKWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKIncreaseStep * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncreaseLeftHandIKWeight = false;
        }
    }

    private void UpdateRigWeight()
    {
        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;

            if (rig.weight >= 1)
                rigShouldBeIncreased = false;
        }
    }

    private void PauseRig() => rig.weight = 0.15f;
    private void PauseLeftHandIK() => leftHandIK.weight = 0f;


    public void PlayWeaponGrabAnimation(GrabType grabType)
    {
        ResetWeightRigAndLeftHandIK();

        anim.SetFloat("WeaponGrabType", ((float)grabType));
        anim.SetTrigger("WeaponGrab");


        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        busyGrabbingWeapon = busy;
        anim.SetBool("BusyGrabbingWeapon", busyGrabbingWeapon);
    }

    public void ResetWeightRigAndLeftHandIK()
    {
        PauseRig();
        PauseLeftHandIK();

        //last Grab can make error, fix it by add event reset weight
        rigShouldBeIncreased = false;
        shouldIncreaseLeftHandIKWeight = false;
    }

    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true;
    public void ReturnLeftHandIKWeightToOne() => shouldIncreaseLeftHandIKWeight = true;

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
        leftHand_Target.localPosition = targetTransform.localPosition;
        leftHand_Target.localRotation = targetTransform.localRotation;
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
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(2);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(3);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(4);
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(5);
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}

public enum GrabType
{
    SideGrab,
    BackGrab
}
