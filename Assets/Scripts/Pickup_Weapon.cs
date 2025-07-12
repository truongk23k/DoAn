using UnityEngine;

public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData;
    private PlayerWeaponController weaponController;

    private Weapon weapon;
    private bool oldWeapon;

    [SerializeField] private BackupWeaponModel[] models;

    private PlayerInteraction playerInteraction;

    private void Start()
    {
        if (!oldWeapon)
            weapon = new Weapon(weaponData);

        UpdateGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon; // save data drop weapon
        weaponData = weapon.weaponData;
        this.transform.position = transform.position + new Vector3(0, 0.75f, 0);

        UpdateGameObject();
    }

    [ContextMenu("Update Item Model")]
    public void UpdateGameObject()
    {
        models = GetComponentsInChildren<BackupWeaponModel>(true);

        gameObject.name = "Pickup_Weapon - " + weaponData.weaponName.ToString() + " - " + weaponData.weaponType.ToString();
        UpdateItemModel();
    }

    public void UpdateItemModel()
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

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();

        if(playerInteraction == null)
            playerInteraction = other.GetComponent<PlayerInteraction>();
    }
}
