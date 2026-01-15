using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Enemy_MeleeWeaponType
{
    OneHand,
    Throw,
    Unarmed
}

public enum  Enemy_RangeWeaponType
{
    Pistol,
    Revolver,
    Shotgun,
    AutoRifle,
    Rifle
}

public class Enemy_Visuals : MonoBehaviour
{
    public GameObject currentWeaponModel { get; private set; }
    public GameObject hiddenWeaponModel { get; private set; }

    [Header("Corruption visuals")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Rig ref")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private Rig rig;


    private void Start()
    {
    }

    public void EnableWeaponTrail(bool enable)
    {
        if (currentWeaponModel != null)
        {
            Enemy_WeaponModel currentWeaponScript = currentWeaponModel.GetComponent<Enemy_WeaponModel>();
            currentWeaponScript.EnableTrailEffect(enable);
        }
    }

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndexs = new List<int>();
        corruptionCrystals = (GameObject[])CollectCorruptionCrystals();

        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for (int i = 0; i < corruptionAmount; i++)
        {
            if (availableIndexs.Count == 0)
                break;
            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }
    }

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyRange = GetComponent<Enemy_Range>() != null;

        if (thisEnemyRange)
            FindRangeWeaponModel();

        if (thisEnemyIsMelee)
            FindMeleeWeaponModel();

        OverrideAnimatorControllerIfCan();
    }

    private void FindRangeWeaponModel()
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                currentWeaponModel = weaponModel.gameObject;
                currentWeaponModel.SetActive(true);

                SwitchAnimationLayer((int)weaponModel.weaponHoldType);
                SetupLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                break;
            }
        }

    }

    private void FindMeleeWeaponModel()
    {
        Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        Enemy_MeleeWeaponType weaponType = GetComponent<Enemy_Melee>().weaponType;

        List<Enemy_WeaponModel> filteredWeaponModels = new List<Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                filteredWeaponModels.Add(weaponModel);
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        currentWeaponModel = filteredWeaponModels[randomIndex].gameObject;
        currentWeaponModel.SetActive(false);

        //hidden weapon
        hiddenWeaponModel = filteredWeaponModels[randomIndex].weaponHidden;
        hiddenWeaponModel.SetActive(true);
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController overrideController = currentWeaponModel.GetComponent<Enemy_WeaponModel>()?.overrideController;
        if (overrideController != null)
        {
            Animator animator = GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = overrideController;
        }
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);

        newMat.mainTexture = colorTextures[randomIndex];

        skinnedMeshRenderer.material = newMat;
    }

    private Object[] CollectCorruptionCrystals()
    {
        Enemy_CorruptionCrystal[] crystalComponents = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
        GameObject[] corruptionCrystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalComponents[i].gameObject;
        }

        return corruptionCrystals;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator anim = GetComponentInChildren<Animator>();

        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public void EnableIK(bool enable)
    {
        rig.weight = enable ? 1f : 0f;
    }

    private void SetupLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget)
    {
        leftHandIK.localPosition = leftHandTarget.localPosition;
        leftHandIK.localRotation = leftHandTarget.localRotation;

        leftElbowIK.localPosition = leftElbowTarget.localPosition;
        leftElbowIK.localRotation = leftElbowTarget.localRotation;
    }
}
