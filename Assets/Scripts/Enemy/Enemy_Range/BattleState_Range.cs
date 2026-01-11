using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10;
    private int bulletsShot = 0;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Player.instance.transform.position);

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
            {
                AttemptToResetWeapon();
            }

            return;
        }

        if (CanShoot())
        {
            Shoot();
        }
    }

    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
    }

    private bool WeaponOnCooldown() => Time.time > lastTimeShoot + enemy.weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShot >= enemy.bulletsToShot;

    private bool CanShoot()
    {
        return Time.time >= lastTimeShoot + 1 / enemy.fireRate;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShoot = Time.time;
        bulletsShot++;
    }
}
