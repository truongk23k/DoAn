using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn;
    public UI_SelectedWeaponWindow[] selectedWeapon;

    [Header("Warning Info")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float disappearSpeed = 0.25f;
    private float currentWarningAlpha;
    private float targetWarningAlpha;

    private void Start()
    {
        selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= disappearSpeed * Time.deltaTime;
            warningText.color = new Color(1, 1, 1, currentWarningAlpha);
        }
    }

    public void ConfirmWeaponSelection()
    {
        if (AtLeastOneWeaponSelected())
        {
            UI.instance.SwitchTo(nextUIToSwitchOn);
            //UI.instance.StartLevelGeneration();
        }
        else
            if(LocalizationManager.instance.currentLanguage == Language.EN)
                ShowWarningMessage("Please select at least one weapon.");
            else
                ShowWarningMessage("Vui lòng chọn ít nhất một vũ khí.");
    }

    public bool IsNoWeapon()
    {
        if (!AtLeastOneWeaponSelected())
        {
            if(LocalizationManager.instance.currentLanguage == Language.EN)
                ShowWarningMessage("Please select at least one weapon.");
            else
                ShowWarningMessage("Vui lòng chọn ít nhất một vũ khí.");
            return true;
        }
        return false;
    }

    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;

    public List<Weapon_Data> SelectedWeaponData()
    {
        List<Weapon_Data> selectedData = new List<Weapon_Data>();

        foreach (UI_SelectedWeaponWindow weapon in selectedWeapon)
        {
            if (weapon.weaponData != null)
                selectedData.Add(weapon.weaponData);
        }

        return selectedData;
    }

    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        foreach (var slot in selectedWeapon)
        {
            if (slot.IsEmpty)
                return slot;
        }
        return null;
    }

    public UI_SelectedWeaponWindow FindSlotWithWeaponType(Weapon_Data weaponData)
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].weaponData == weaponData)
                return selectedWeapon[i];
        }
        return null;
    }

    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white;
        warningText.text = message;

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0;
    }
}
