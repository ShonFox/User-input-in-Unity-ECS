using Unity.VisualScripting;
using UnityEngine;

public enum ReactionType
{
    Damage,      // пуля: нанести урон тому, во что попала
    Heal,        // аптечка: вылечить подобравшего
    RicochetPerk // перк: выдать отскок снарядов
}

public class CollisionReactionComponent : MonoBehaviour
{
    [Header("Настройка реакции")]
    public ReactionType Reaction = ReactionType.Damage;

    [Tooltip("С каким тегом срабатывает реакция")]
    public string TargetTag = "Player";

    [Tooltip("Сила эффекта: урон или лечение")]
    public float Value = 10f;

    [HideInInspector] public GameObject Owner; // кто выпустил пулю

    // Пуля летит с Rigidbody, поэтому все её удары приходят сюда.
    private void OnCollisionEnter(Collision collision)
    {
        switch (Reaction)
        {
            case ReactionType.Damage:
                HandleBulletHit(collision);
                break;
                // У поднималок OnCollision не бывает нужды — их ловит триггер
        }
    }

    // Поднималки: персонаж вошёл в зону
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(TargetTag)) return;

        switch (Reaction)
        {
            case ReactionType.Heal:
                var hp = other.GetComponent<HealthComponent>();
                if (hp != null)
                {
                    hp.ChangeHealth(+Value);
                    Destroy(gameObject);
                }
                break;

            case ReactionType.RicochetPerk:
                if (other.GetComponent<RicochetPerkComponent>() == null)
                    other.AddComponent<RicochetPerkComponent>();
                Debug.Log("Перк отскока подобран!");
                Destroy(gameObject);
                break;
        }
    }

    private void HandleBulletHit(Collision collision)
    {
        var hit = collision.gameObject;

        // Попали в персонажа — наносим урон и исчезаем
        if (hit.CompareTag(TargetTag))
        {
            var hp = hit.GetComponent<HealthComponent>();
            if (hp != null) hp.ChangeHealth(-Value);
            Destroy(gameObject);
            return;
        }

        // Попали во что-то другое (стена). Есть перк — отскакиваем
        bool hasRicochet = Owner != null &&
                           Owner.GetComponent<RicochetPerkComponent>() != null;
        if (!hasRicochet)
        {
            Destroy(gameObject);
            return;
        }

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
    }
}
