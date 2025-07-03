using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Player player;

    private Animator anim;
    public bool isEquipingWeapon { get; private set; }

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

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
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();

        UpdateLeftHandIKWeight();
    }

    public void PlayReloadAnimation()
    {
        if (isEquipingWeapon)
            return;

        float reloadSpeed = player.weapon.CurrentWeapon.reloadSpeed;
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = player.weapon.CurrentWeapon.equipmentSpeed;

        ResetWeightRigAndLeftHandIK();

        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", ((float)equipType));
        anim.SetFloat("EquipSpeed", equipmentSpeed);

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        isEquipingWeapon = busy;
        anim.SetBool("BusyEquipingWeapon", isEquipingWeapon);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        SwitchOffWeaponModels();
        
        SwitchOffBackupWeaponModels();
        SwitchOnBackupWeaponModel();

        SwitchAnimationLayer(((int)CurrentWeaponModel().holdType));
        CurrentWeaponModel().gameObject.SetActive(true);

        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        foreach (WeaponModel weapon in weaponModels)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach(BackupWeaponModel backupModel in backupWeaponModels)
        {
            backupModel.gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        if (player.weapon.BackupWeapon() == null)
            return;

        WeaponType weaponType  = player.weapon.BackupWeapon().weaponType;

        foreach(BackupWeaponModel backupModel in backupWeaponModels)
        {
            if(backupModel.weaponType == weaponType)
                backupModel.gameObject.SetActive(true);
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponType weaponType = player.weapon.CurrentWeapon.weaponType;

        foreach (var weapon in weaponModels)
            if (weapon.weaponType == weaponType)
                return weapon;

        return null;
    }

    #region Animation rigging methods
    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHand_Target.localPosition = targetTransform.localPosition;
        leftHand_Target.localRotation = targetTransform.localRotation;
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
    #endregion

}


