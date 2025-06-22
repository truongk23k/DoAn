using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator anim;
    public bool isGrabbingWeapon {  get; private set; }

    #region Gun Transform region
    [SerializeField] private Transform[] gunTransforms;

    private Transform currentGun;
    #endregion

    [Header("Rig")]
    [SerializeField] private float rigWeigtIncreaseRate;
    private bool shouldIncrease_RigWeight;
    private Rig rig;

    [Header("Left hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHand_Target;
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    private bool shouldIncrease_LeftHandIKWeight;



    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        rig = GetComponentInChildren<Rig>();

        SwitchOn(1);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && !isGrabbingWeapon)
        {
            anim.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();

        UpdateLeftHandIKWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncrease_LeftHandIKWeight = false;
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeigtIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
                shouldIncrease_RigWeight = false;
        }
    }

    private void ReduceRigWeight() => rig.weight = 0.15f;
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
        isGrabbingWeapon = busy;
        anim.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }

    public void ResetWeightRigAndLeftHandIK()
    {
        ReduceRigWeight();
        PauseLeftHandIK();

        //last Grab can make error, fix it by add event reset weight
        shouldIncrease_RigWeight = false;
        shouldIncrease_LeftHandIKWeight = false;
    }

    public void MaximizeRigWeight() => shouldIncrease_RigWeight = true;
    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandIKWeight = true;

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
