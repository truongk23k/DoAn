using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range : Enemy
{
    [Header("Cover systems")]
    public bool canUseCover = true;
    public CoverPoint lastCover;
    public List<Cover> allCovers = new List<Cover>();

    [Header("Weapon details")]
    public Enemy_RangeWeaponType weaponType;
    public Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [SerializeField] List<Enemy_RangeWeaponData> avalibleWeaponData;

    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
    }

    override protected void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        visuals.SetupLook();

        SetupWeapon();

        allCovers = CollectNearByCovers();
    }

    protected override void Update()
    {
        base.Update();

    }

    #region Cover System
    public Transform AttemptToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

        foreach (Cover cover in allCovers)
        {
            collectedCoverPoints.AddRange(cover.GetCoverPoints());
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
            lastCover = closetCoverPoint;

        return lastCover.transform;
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

        if (canUseCover)
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
