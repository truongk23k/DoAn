using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI;
    [SerializeField] private Mission mission;
    private TextMeshProUGUI myText;

    private void OnValidate()
    {
        gameObject.name = "Button - Select Mission: " + mission.missionName;
    }

    public override void Start()
    {
        base.Start();
        missionUI = GetComponentInParent<UI_MissionSelection>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        if (myText != null)
            myText.text = mission.missionName;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (LocalizationManager.instance.currentLanguage == Language.EN)
            missionUI.UpdateMissionDescription(mission.missionDescription);
        else
            missionUI.UpdateMissionDescription(mission.missionDescription_VN);
    }

    override public void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (LocalizationManager.instance.currentLanguage == Language.EN)
            missionUI.UpdateMissionDescription("Choose a mission!");
        else
            missionUI.UpdateMissionDescription("Chọn một nhiệm vụ!");
    }

    override public void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        MissionManager.instance.SetCurrentMission(mission);

    }
}
