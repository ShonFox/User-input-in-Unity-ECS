using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputSystem : ComponentSystem
{
    private EntityQuery _inputQuery;

    private InputAction _moveAction;
    private InputAction _shootAction;
    private InputAction _sprintAction;

    private float2 _keyboardInput;
    private float2 _joystickInput;
    private float _shootInput;
    private float _sprintInput;


    protected override void OnCreate()
    {
        _inputQuery = GetEntityQuery(ComponentType.ReadOnly<InputData>());
    }

    protected override void OnStartRunning()
    {
        _moveAction = new InputAction("move", binding: "<Gamepad>/leftStick");
        _moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        _moveAction.performed += context => { _keyboardInput = context.ReadValue<Vector2>(); };
        _moveAction.started += context => { _keyboardInput = context.ReadValue<Vector2>(); };
        _moveAction.canceled += context => { _keyboardInput = context.ReadValue<Vector2>(); };
        _moveAction.Enable();

        _shootAction = new InputAction("shoot", binding: "<Keyboard>/e");
        _shootAction.performed += context => { _shootInput = context.ReadValue<float>(); };
        _shootAction.started += context => { _shootInput = context.ReadValue<float>(); };
        _shootAction.canceled += context => { _shootInput = context.ReadValue<float>(); };
        _shootAction.Enable();

        _sprintAction = new InputAction("sprint", binding: "<Keyboard>/space");
        _sprintAction.performed += context => { _sprintInput = context.ReadValue<float>(); };
        _sprintAction.started += context => { _sprintInput = context.ReadValue<float>(); };
        _sprintAction.canceled += context => { _sprintInput = context.ReadValue<float>(); };
        _sprintAction.Enable();
    }

    protected override void OnStopRunning()
    {
        _moveAction.Disable();
        _shootAction.Disable();
        _sprintAction.Disable();
    }

    protected override void OnUpdate()
    {
        // Джойстик читаем напрямую каждый кадр
        var pad = UnityEngine.InputSystem.Gamepad.current;
        if (pad != null)
        {
            Vector2 stick = pad.leftStick.ReadValue();
            _joystickInput = math.lengthsq(stick) > 0.0001f
                ? new float2(stick.x, stick.y)
                : float2.zero;
        }
        else
        {
            _joystickInput = float2.zero;
        }

        // Клавиатура лежит в _keyboardInput и обновляется событиями действия
        bool hasJoystick = math.any(_joystickInput != float2.zero);
        float2 move = hasJoystick ? _joystickInput : _keyboardInput;

        Entities.With(_inputQuery).ForEach(
            (Entity entity, ref InputData inputData) =>
            {
                inputData.Move = move;
                inputData.Shoot = _shootInput;
                inputData.Sprint = _sprintInput;
            });
    }
}
