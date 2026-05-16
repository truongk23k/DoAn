using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data weaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponInfo;

    private void Start()
    {
        weaponData = null;
        UpdateSlotInfo(null);
    }

    public void SetWeaponSlot(Weapon_Data weaponData)
    {
        this.weaponData = weaponData;
        UpdateSlotInfo(weaponData);
    }

    public void UpdateSlotInfo(Weapon_Data weaponData)
    {
        if (weaponData == null)
        {
            weaponIcon.color = Color.clear;
            if(LocalizationManager.instance.currentLanguage == Language.EN)
                weaponInfo.text = "Select a weapon...";
            else
                weaponInfo.text = "Chọn một vũ khí...";
            return;
        }

        weaponIcon.color = Color.white;
        weaponIcon.sprite = weaponData.weaponIcon;
        if (LocalizationManager.instance.currentLanguage == Language.EN)
            weaponInfo.text = weaponData.weaponInfo;
        else
            weaponInfo.text = weaponData.weaponInfo_VN;
    }

    public bool IsEmpty => weaponData == null;
}
