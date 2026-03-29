using TMPro;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerMovementData",
    menuName = "ScriptableObjects/Player/PlayerMovementData"
    )]

public class PlayerMovementDataSO : ScriptableObject

{
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _stealthMoveSpeed = 2.5f;

    [Header("Input")]
    [SerializeField] private float _inputDeadZone = 0.15f;

    public float MoveSpeed => _moveSpeed;
    public float StealthMoveSpeed => _stealthMoveSpeed;
    public float InputDeadZone => _inputDeadZone;
}
