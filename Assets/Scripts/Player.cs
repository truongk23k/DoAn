using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControlls controls { get; private set; }
    public PlayerAim aim  { get; private set; } 

    private void Awake()
    {
        controls = new PlayerControlls();   
        aim = GetComponent<PlayerAim>();    
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
