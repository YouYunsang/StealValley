using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _playerTrans;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -15f);

    private void Start()
    {
        if(_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }

        _playerTrans = _player.transform;
    }

    private void LateUpdate()
    {
        if (_player == null) return;
        Vector3 desiredPosition = _playerTrans.position + _offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
