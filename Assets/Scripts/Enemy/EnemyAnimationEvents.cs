using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActivateManualMovement(true);

    public void StopManualMovement() => enemy.ActivateManualMovement(false);

    public void StartManualRotation() => enemy.ActivateManualRotate(true);

    public void StopManualRotation() => enemy.ActivateManualRotate(false);

    public void StopDodge() => (enemy as Enemy_Melee).StopDodge();

    public void AbilityEvent() => (enemy as Enemy_Melee).TriggerAbility();
}
