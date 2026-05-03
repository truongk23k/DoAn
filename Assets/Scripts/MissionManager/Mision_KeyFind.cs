using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Mission", menuName = "Missions/Key Mission")]
public class Mision_KeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool keyFound = false;

    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickedUp += PickUpKey;
        
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key);
        enemy.MakeEnemyVIP();
    }

    public override bool MissionCompleted()
    {
        return keyFound;
    }

    private void PickUpKey()
    {
        keyFound = true;
        MissionObject_Key.OnKeyPickedUp -= PickUpKey;
        Debug.Log(" I pick up a key");
    }
    
}
