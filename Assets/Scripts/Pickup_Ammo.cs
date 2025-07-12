using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType
{
    smallBox,
    bigBox
}

public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType boxType;

    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    private List<AmmoData> currentAmmoList = new List<AmmoData>();

    [SerializeField] GameObject[] boxModel;

    private PlayerWeaponController weaponController;
    private PlayerInteraction playerInteraction;

    private void Start()
    {
        SetupBoxModel();

        SetupCurrentDataList();
    }

    private void SetupCurrentDataList()
    {
        currentAmmoList = boxType == AmmoBoxType.smallBox ? smallBoxAmmo : bigBoxAmmo;

        foreach (AmmoData ammo in currentAmmoList)
        {
            SetupBulletAmount(ammo);
        }
    }

    public override void Interaction()
    {
        List<AmmoData> toRemove = new List<AmmoData>();

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponByTypeInSlots(ammo.WeaponType);

            AddBulletsToWeapon(weapon, ammo.amount);

            if (weapon != null)
                toRemove.Add(ammo);
        }

        foreach (AmmoData ammo in toRemove)
        {
            currentAmmoList.Remove(ammo);
        }

        if (currentAmmoList.Count == 0)
        {
            playerInteraction.RemoveClosestInteractable();
            ObjectPool.instance.ReturnObject(gameObject);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();

        if (playerInteraction == null)
            playerInteraction = other.GetComponent<PlayerInteraction>();
    }

    private void SetupBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        ammoData.amount = Mathf.RoundToInt(Random.Range(min, max));
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);

            if (i == ((int)boxType))
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) return;

        weapon.totalReserveAmmo += amount;
    }
}

[System.Serializable]
public class AmmoData
{
    public WeaponType WeaponType;
    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
    public int amount;
}
