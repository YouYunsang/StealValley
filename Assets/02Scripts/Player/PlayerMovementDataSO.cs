using TMPro;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerMovementData",
    menuName = "ScriptableObjects/Player/PlayerMovementData"
    )]

public class PlayerMovementDataSO : ScriptableObject
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 3.4f;
    [SerializeField] private float _stealthMoveSpeed = 2f;

    [Header("Carry Weight")]
    [SerializeField] private float _moveSpeedDecreasePerCrop = 0.03f;
    [SerializeField] private float _minMoveSpeed = 2f;
    [SerializeField] private float _minStealthMoveSpeed = 1f;

    [Header("Input")]
    [SerializeField] private float _inputDeadZone = 0.15f;

    public float MoveSpeed => _moveSpeed;
    public float StealthMoveSpeed => _stealthMoveSpeed;
    public float MoveSpeedDecreasePerCrop => _moveSpeedDecreasePerCrop;
    public float MinMoveSpeed => _minMoveSpeed;
    public float MinStealthMoveSpeed => _minStealthMoveSpeed;
    public float InputDeadZone => _inputDeadZone;

    private void OnValidate()
    {
        if (_moveSpeed < 0f)
        {
            _moveSpeed = 0f;
        }

        if (_stealthMoveSpeed < 0f)
        {
            _stealthMoveSpeed = 0f;
        }

        if (_moveSpeedDecreasePerCrop < 0f)
        {
            _moveSpeedDecreasePerCrop = 0f;
        }

        if (_minMoveSpeed < 0f)
        {
            _minMoveSpeed = 0f;
        }

        if (_minStealthMoveSpeed < 0f)
        {
            _minStealthMoveSpeed = 0f;
        }

        if (_inputDeadZone < 0f)
        {
            _inputDeadZone = 0f;
        }
    }
}
