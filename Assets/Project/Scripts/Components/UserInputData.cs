using DefaultNamespace.Components.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UserInputData : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed;
    public float sprintMultiplier = 2f;
    public MonoBehaviour ShootAction;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InputData());
        dstManager.AddComponentData(entity, new MoveData
        {
            Speed = speed / 100,
            SprintMultiplier = sprintMultiplier
        });
        if (ShootAction != null && ShootAction is IAbility)
        {
            dstManager.AddComponentData(entity, new ShootData());
        }
    }
}

public struct InputData : IComponentData
{
    public float2 Move;
    public float Shoot;
    public float Sprint;
}

public struct MoveData : IComponentData
{
    public float Speed;
    public float SprintMultiplier;
}

public struct ShootData : IComponentData
{
}
