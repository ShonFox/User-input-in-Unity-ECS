using DefaultNamespace.Components.Interfaces;
using UnityEngine;

public class ShootAbility : MonoBehaviour, IAbility
{
    public GameObject bullet;
    public float shootDalay;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f;

    private float _shootTime = float.MinValue;

    public void Execute()
    {
        if (Time.time < _shootTime + shootDalay) return;

        _shootTime = Time.time;

        if (bullet == null)
        {
            Debug.LogError("[SHOOT ABILITY] No bullet prefab link");
            return;
        }

        var t = transform;
        // Спавним чуть впереди персонажа, чтобы пуля не застревала в нём самом
        var newBullet = Instantiate(bullet, t.position + t.forward * 0.7f, t.rotation);

        var rb = newBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = t.forward * bulletSpeed;
        }
        else
        {
            Debug.LogError("[SHOOT ABILITY] Bullet prefab has no Rigidbody");
        }

        Destroy(newBullet, bulletLifetime);
    }
}
