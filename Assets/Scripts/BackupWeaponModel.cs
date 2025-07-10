using UnityEngine;

public enum HangType
{
    LowBackHang,
    BackHang,
    SideHang
}
public class BackupWeaponModel : MonoBehaviour
{
    public Weapon_Data weaponData_Backup;

    private void Start()
    {
        UpdateGameObject();
    }

    [ContextMenu("Update Backup GameObject")]
    public void UpdateGameObject()
    {
        gameObject.name = "Backup_Weapon - " + weaponData_Backup.weaponName + " - " + weaponData_Backup.weaponType.ToString();
    }

    public void Activate(bool activate) => gameObject.SetActive(activate);

    public bool HangTypeIs(HangType hangType) => weaponData_Backup.hangType == hangType;
}
