using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();

        enemy.visuals.EnableIK(true, true);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.visuals.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer())
            enemy.FaceTarget(enemy.aim.position);

        if (!enemy.IsPlayerInAggresionRange())
            stateMachine.ChangeState(enemy.advancePlayerState);

        ChangeCoverIfShould();


        if (WeaponOutOfBullets())
        {
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

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
            return;

        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = 0.5f; //check every 0.5 seconds

            if (IsPlayerInClearSight() || IsPlayerClose())
            {
                if (enemy.CanGetCover())
                    stateMachine.ChangeState(enemy.runToCoverState);
            }
        }
    }

    #region Cover system region

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
    #endregion

}
