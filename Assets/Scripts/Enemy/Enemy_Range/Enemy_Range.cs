using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk
{
    Unvalible,
    CanTakeCover,
    CanTakeAndChangeCover
}
public class Enemy_Range : Enemy
{
    [Header("Enemy perks")]
    public CoverPerk coverPerk;

    [Header("Advance perk")]
    public float advanceSpeed;
    public float advanceStoppingDistance;

    [Header("Cover systems")]
    public float safeDistance;
    public CoverPoint currentCover { get; private set; }
    public CoverPoint lastCover { get; private set;}

    [Header("Weapon details")]
    public Enemy_RangeWeaponType weaponType;
    public Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [SerializeField] List<Enemy_RangeWeaponData> avalibleWeaponData;

    #region States
    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }
    public AdvancePlayer_Range advancePlayerState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayer_Range(this, stateMachine, "Advance");
    }

    override protected void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        visuals.SetupLook();

        SetupWeapon();
    }

    protected override void Update()
    {
        base.Update();

    }

    #region Cover System
    public bool CanGetCover()
    {
        if(coverPerk == CoverPerk.Unvalible)
            return false;

        currentCover = AttemptToFindCover()?.GetComponent<CoverPoint>();

        if (currentCover != null && lastCover != currentCover)
            return true;

        Debug.Log("No cover found");
        return false;
    }

    private Transform AttemptToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

        foreach (Cover cover in CollectNearByCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint closetCoverPoint = null;
        float closestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closetCoverPoint = coverPoint;
            }
        }

        if (closetCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = closetCoverPoint;
            currentCover.SetOccupied(true);

            return currentCover.transform;
        }

        return null;
    }

    private List<Cover> CollectNearByCovers()
    {
        float coverRadiusCheck = 30f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<Cover> collectedCovers = new List<Cover>();

        foreach (Collider collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();
            if (cover != null && !collectedCovers.Contains(cover))
                collectedCovers.Add(cover);
        }

        return collectedCovers;
    }
    #endregion

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");

        Vector3 bulletsDirection = ((Player.instance.transform.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        Enemy_Bullet bulletScript = newBullet.GetComponent<Enemy_Bullet>();
        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);
        bulletScript.BulletSetup(bulletDirectionWithSpread, weaponData.bulletSpeed);
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        if (CanGetCover())
            stateMachine.ChangeState(runToCoverState);
        else
            stateMachine.ChangeState(battleState);
    }

    private void SetupWeapon()
    {
        if (avalibleWeaponData.Count == 0)
            return;

        List<Enemy_RangeWeaponData> filteredData = new List<Enemy_RangeWeaponData>();

        foreach (Enemy_RangeWeaponData weaponData in avalibleWeaponData)
        {
            if (weaponData.weaponType == weaponType)
                filteredData.Add(weaponData);
        }

        if (filteredData.Count == 0)
            return;

        int randomIndex = Random.Range(0, filteredData.Count);

        weaponData = filteredData[randomIndex];

        gunPoint = visuals.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;
    }

}
