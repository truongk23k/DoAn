using UnityEngine;

public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData;

    private Weapon weapon;
    private bool oldWeapon;

    [SerializeField] private BackupWeaponModel[] models;


    private void Start()
    {
        if (!oldWeapon)
            weapon = new Weapon(weaponData);

        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon; // save data drop weapon
        weaponData = weapon.weaponData;
        this.transform.position = transform.position + new Vector3(0, 0f, 0);

        SetupGameObject();
    }

    [ContextMenu("Update Item Model")]
    private void SetupGameObject()
    {
        models = GetComponentsInChildren<BackupWeaponModel>(true);

        gameObject.name = "Pickup_Weapon - " + weaponData.weaponName.ToString() + " - " + weaponData.weaponType.ToString();
        SetupWeaponModel();
    }

    public void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false);

            if (model.weaponData_Backup.weaponName == weaponData.weaponName)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weapon);

        playerInteraction.RemoveClosestInteractable();

        ObjectPool.instance.ReturnObject(gameObject);
    }

}
