using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;
    private bool firstTimeAttack = true;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        SetupValuesForFirstAttack();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.visuals.EnableIK(true, true);

        stateTimer = enemy.attackDelay;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer())
            enemy.FaceTarget(enemy.aim.position);

        if(enemy.CanThrowGrenade())
            stateMachine.ChangeState(enemy.throwGrenadeState);

        if (MustAdvancePlayer())
            stateMachine.ChangeState(enemy.advancePlayerState);

        ChangeCoverIfShould();

        if(stateTimer > 0)
            return;

        if (WeaponOutOfBullets())
        {
            if (enemy.IsUnstoppable() && UnstoppableWalkReady())
            {
                enemy.advanceDuration = weaponCooldown;
                stateMachine.ChangeState(enemy.advancePlayerState);
                return;
            }

            if (WeaponOnCooldown())
            {
                AttemptToResetWeapon();
            }

            return;
        }

        if (CanShoot() && enemy.IsAimOnPlayer())
        {
            Shoot();
        }
    }

    private bool MustAdvancePlayer()
    {
        if (enemy.IsUnstoppable())
            return false;

        return !enemy.IsPlayerInAggresionRange() && ReadyToLeaveCover();     
    }

    private bool UnstoppableWalkReady()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, Player.instance.transform.position);
        bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
        bool unstoppableWalkOnCooldown = Time.time < enemy.weaponData.minWeaponCooldown + enemy.advancePlayerState.lastTimeAdvanced;

        return outOfStoppingDistance && !unstoppableWalkOnCooldown;
    }

    #region Cover system region
    private bool ReadyToLeaveCover()
    {
        return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover;
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
            return;

        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = 0.5f; //check every 0.5 seconds

            if (ReadyToChangeCover())
            {
                if (enemy.CanGetCover())
                    stateMachine.ChangeState(enemy.runToCoverState);
            }
        }
    }

    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() || IsPlayerClose();
        bool advanceTimeOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced + enemy.advanceDuration;

        return inDanger && advanceTimeOver;
    }

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, Player.instance.transform.position) < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = Player.instance.transform.position - enemy.transform.position;
        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.collider.gameObject.GetComponentInParent<Player>();
        }

        return false;
    }
    #endregion

    #region Weapon region
    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
    }

    private bool WeaponOnCooldown() => Time.time > lastTimeShoot + weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack;

    private bool CanShoot()
    {
        return Time.time >= lastTimeShoot + 1 / enemy.weaponData.fireRate;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShoot = Time.time;
        bulletsShot++;
    }

    private void SetupValuesForFirstAttack()
    {
        if (firstTimeAttack)
        {
            firstTimeAttack = false;
            bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
            weaponCooldown = enemy.weaponData.GetWeaponCooldown();
        }
    }
    #endregion

}
