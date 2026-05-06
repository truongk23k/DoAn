using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings")]
    public bool friendlyFire;
    [Space]
    public bool quickStart;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

    }

    public void GameStart()
    {
        UI.instance.inGameUI.InitSlots();
        SetDefaultWeaponsForPlayer();
        LevelGenerator.instance.InitializedGeneration();

        TimeManager.instance.ResumeTime();

        //we start selected mission in a LevelGenerator script, after we done with level creation
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameCompleted()
    {
        UI.instance.ShowVictoryScreen();
        ControlsManager.instance.controls.Character.Disable();
        Player.instance.health.currentHealth += 99999; //so player won't die in last second
    }

    public void GameOver()
    {
        TimeManager.instance.SlowMotionFor(1.5f);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(5);
    }

    public void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelectionUI.SelectedWeaponData();
        Player.instance.weapon.SetDefaultWeapon(newList);
    }
}
