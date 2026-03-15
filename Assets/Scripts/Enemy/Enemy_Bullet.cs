using UnityEngine;

public class Enemy_Bullet : Bullet
{
    protected override void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx();
        ReturnBulletToPool();

        Player player = collision.gameObject.GetComponentInParent<Player>();
        /*if (player != null)
        {
            Debug.Log("Shoot player");
        }*/
    }
}
