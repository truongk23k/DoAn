using System.Collections.Generic;
using UnityEngine;

public enum Enemy_MeleeWeaponType
{
    OneHand,
    Throw
}


public class Enemy_Visuals : MonoBehaviour
{
    [Header("Weapon model")]
    [SerializeField] private Enemy_WeaponModel[] weaponModels;
    [SerializeField] private Enemy_MeleeWeaponType weaponType;

    public GameObject currentWeaponModel { get; private set; }
    public GameObject hiddenWeaponModel { get; private set; }

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Start()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
    }

    public void SetupWeaponType(Enemy_MeleeWeaponType type) => weaponType = type;

    private void SetupRandomWeapon()
    {
        foreach (var weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }

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

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);

        newMat.mainTexture = colorTextures[randomIndex];

        skinnedMeshRenderer.material = newMat;
    }
}
