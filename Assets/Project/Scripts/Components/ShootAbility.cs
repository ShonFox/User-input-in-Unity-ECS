using DefaultNamespace.Components.Interfaces;
using UnityEngine;

public class ShootAbility : MonoBehaviour, IAbility
{
    public GameObject bullet;
    public float shootDalay;
    public float bulletSpeed = 10f; // скорость полёта пули

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
        var newBullet = Instantiate(bullet, t.position + t.forward * 0.7f, t.rotation);

        // Задаём пуле скорость по направлению взгляда персонажа
        var rb = newBullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = t.forward * bulletSpeed;

        // Передаём пуле владельца — для проверки перка отскока
        var reaction = newBullet.GetComponent<CollisionReactionComponent>();
        if (reaction != null)
            reaction.Owner = gameObject;

        // Пуля живёт не вечно, иначе сцена забьётся
        Destroy(newBullet, 5f);
    }
}

