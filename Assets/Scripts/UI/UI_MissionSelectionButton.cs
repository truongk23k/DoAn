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

        missionUI.UpdateMissionDescription(mission.missionDescription);
    }

    override public void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        missionUI.UpdateMissionDescription("Choose a mission!");
    }

    override public void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        MissionManager.instance.SetCurrentMission(mission);

    }
}
