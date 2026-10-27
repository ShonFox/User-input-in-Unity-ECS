using Unity.Entities;
using UnityEngine;

public class CharterMoveSystem : ComponentSystem
{
    private EntityQuery _moveQuery;

    protected override void OnCreate()
    {
        _moveQuery = GetEntityQuery(ComponentType.ReadOnly<InputData>(),
            ComponentType.ReadOnly<MoveData>(),
            ComponentType.ReadOnly<Transform>());
    }

    protected override void OnUpdate()
    {
        Entities.With(_moveQuery).ForEach(
            (Entity entity, Transform transform, ref InputData inputData, ref MoveData move) =>
            {
                // Спринт: множитель применяется, пока зажат пробел
                float speed = move.Speed * (inputData.Sprint > 0f ? move.SprintMultiplier : 1f);

                var dir = new Vector3(inputData.Move.x, 0, inputData.Move.y);

                if (dir.sqrMagnitude > 0.0001f)
                {
                    var pos = transform.position;
                    pos += dir.normalized * speed;
                    transform.position = pos;

                    // Поворот в сторону движения (плавный)
                    var targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, targetRotation, Time.DeltaTime * 10f);
                }
            });
    }
}
