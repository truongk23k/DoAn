using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public string missionName;

    [TextArea]
    public string missionDescription;
    [TextArea]
    public string missionDescription_VN;

    public abstract void StartMission();
    public abstract bool MissionCompleted();

    public virtual void UpdateMission()
    {

    }
}
