using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TransparentOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Dictionary<Image, Color> originalImageColors = new Dictionary<Image, Color>();
    private Dictionary<TextMeshProUGUI, Color> originalTextColors = new Dictionary<TextMeshProUGUI, Color>();

    private bool hasUIWeaponSlots;
    private Player_WeaponController playerWeaponController;

    private void Start()
    {
        hasUIWeaponSlots = GetComponentInChildren<UI_WeaponSlot>();
        if (hasUIWeaponSlots)
            playerWeaponController = Player.instance.GetComponent<Player_WeaponController>();

        foreach (Image img in GetComponentsInChildren<Image>(true))
        {
            originalImageColors[img] = img.color;
        }

        foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            originalTextColors[text] = text.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach(Image img in originalImageColors.Keys)
        {
            Color color = img.color;
            color.a = 0.15f; // Set alpha to 15%
            img.color = color;
        }

        foreach (TextMeshProUGUI text in originalTextColors.Keys)
        {
            Color color = text.color;
            color.a = 0.15f; // Set alpha to 15%
            text.color = color;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (Image img in originalImageColors.Keys)
        {
            img.color = originalImageColors[img];
        }

        foreach (TextMeshProUGUI text in originalTextColors.Keys)
        {
            text.color = originalTextColors[text];
        }

        playerWeaponController?.UpdateWeaponUI();
    }

    
}
