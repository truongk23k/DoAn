using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings")]
    public bool friendlyFire;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    public void GameStart()
    {
        SetDefaultWeaponsForPlayer();
        LevelGenerator.instance.InitializedGeneration();

        //we start selected mission in a LevelGenerator script, after we done with level creation
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelectionUI.SelectedWeaponData();
        Player.instance.weapon.SetDefaultWeapon(newList);
    }
}
