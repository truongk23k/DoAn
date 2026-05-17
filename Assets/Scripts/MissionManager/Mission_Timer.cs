using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Missions/Timer mission")]
public class Mission_Timer : Mission
{
    public float time;

    private float currentTime;
    public override void StartMission()
    {
        currentTime = time;
    }

    override public void UpdateMission()
    {
        currentTime -= Time.deltaTime;

        if(currentTime < 0)
        {
            //GameManager.instance.GameOver();
        }

        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");

        string missionText = "Get to the evacuation point before plane takes off.";
        string missionDetails = "Time left: " + timeText;

        if(LocalizationManager.instance.currentLanguage == Language.EN){
            missionText = "Get to the evacuation point before plane takes off.";
            missionDetails = "Time left: " + timeText;
        }
        else{
            missionText = "Đến điểm caats trước khi máy bay cất cánh.";
            missionDetails = "Thời gian còn lại: " + timeText;
        }

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    public override bool MissionCompleted()
    {
        return currentTime >= 0;
    }

}
