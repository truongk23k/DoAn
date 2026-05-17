using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hunt Mission", menuName = "Missions/Hunt Mission")]
public class Mission_EnemyHunt : Mission
{
    public int amountToKill = 12;
    public EnemyType enemyType;

    private int killsToGo;

    public override void StartMission()
    {
        killsToGo = amountToKill;
        MissionObject_HuntTarget.OnTargetKilled += EliminateTarget;

        List<Enemy> validEnemies = new List<Enemy>();

        if (enemyType == EnemyType.Random)
        {
            validEnemies = new List<Enemy>(LevelGenerator.instance.GetEnemyList());
        }
        else
        {
            foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
            {
                if (enemy.enemyType == enemyType)
                    validEnemies.Add(enemy);
            }
        }   

        killsToGo = Mathf.Min(amountToKill, validEnemies.Count);
        amountToKill = killsToGo;

        UpdateMissionUI();

        for (int i = 0; i < amountToKill; i++)
        {
            if (validEnemies.Count <= 0)
                return;

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_HuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    private void EliminateTarget()
    {
        killsToGo--;
        UpdateMissionUI();

        if (killsToGo <= 0)
        {
            if(LocalizationManager.instance.currentLanguage == Language.EN)
                UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
            else
                UI.instance.inGameUI.UpdateMissionInfo("Đến điểm thoát hiểm.");

            MissionObject_HuntTarget.OnTargetKilled -= EliminateTarget;
        }

    }

    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + amountToKill +" enemies with signal disruptor.";
        string missionDetails = "Target left: " + killsToGo;
        if(LocalizationManager.instance.currentLanguage == Language.EN){
            missionText = "Eliminate " + amountToKill +" enemies with signal disruptor.";
            missionDetails = "Target left: " + killsToGo;
        }
        else{
            missionText = "Loại bỏ " + amountToKill + " đối thủ có thiết bị phát tín hiệu.";
            missionDetails = "Đối thủ còn lại: " + killsToGo;
        }
        
        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

}
