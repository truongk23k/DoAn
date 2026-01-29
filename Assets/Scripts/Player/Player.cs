using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Transform playerBody;

    public PlayerControlls controls { get; private set; }
    public PlayerAim aim { get; private set; }
    public PlayerMovement movement { get; private set; }

    public PlayerWeaponController weapon { get; private set; }
    public PlayerWeaponVisuals weaponVisuals { get; private set; }
    public PlayerInteraction interaction { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        controls = new PlayerControlls();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeaponController>();
        weaponVisuals = GetComponent<PlayerWeaponVisuals>();
        interaction = GetComponent<PlayerInteraction>();
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
