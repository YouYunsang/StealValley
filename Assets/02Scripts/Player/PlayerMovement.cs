using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    private Vector2 _moveDirection;
    private float _currentMoveSpeed;

    public Vector2 MoveDirection => _moveDirection;
    public bool IsMoving => _moveDirection.sqrMagnitude > 0f;
    public float CurrentMoveSpeed => _currentMoveSpeed;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        _currentMoveSpeed = moveSpeed;
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _moveDirection = moveDirection;
    }

    private void Move()
    {
        // Rigidbody2D 기반 탑다운 이동 처리
        Vector2 nextPosition = _rigidbody2D.position + (_moveDirection * _currentMoveSpeed * Time.fixedDeltaTime);
        _rigidbody2D.MovePosition(nextPosition);
    }
}
