using System;
using UnityEngine;

/// Компонент здоровья. Вешается на персонажа.
/// Отвечает только за хранение и изменение здоровья — всю логику
/// «кто и как меняет здоровье» делают другие компоненты.
public class HealthComponent : MonoBehaviour
{
    [Header("Здоровье")]
    [Tooltip("Максимальное здоровье")]
    public float MaxHealth = 100f;

    [Tooltip("Текущее здоровье (в инспекторе можно задать стартовое)")]
    public float CurrentHealth;

    // Событие: другие скрипты могут подписаться на смерть персонажа
    public event Action OnDied;

    private void Awake()
    {
        // Если здоровье не задали вручную — стартуем с полного
        if (CurrentHealth <= 0f)
            CurrentHealth = MaxHealth;
    }

    /// Меняет здоровье на переданное значение.
    /// Отрицательное число — урон, положительное — лечение.
    public void ChangeHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
        Debug.Log($"{name}: здоровье {CurrentHealth}/{MaxHealth}");

        if (CurrentHealth <= 0f)
        {
            OnDied?.Invoke();
            Debug.Log($"{name}: погиб!");
        }
    }
}
