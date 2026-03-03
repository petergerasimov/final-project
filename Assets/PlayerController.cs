using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _maxSpeed = 7f;
    [SerializeField]
    private float _jumpForce = 5f;
    [SerializeField]
    private float _lookSensitivity = 0.5f;

    [SerializeField]
    private Transform _cameraTransform;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _xRotation = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = _lookInput.x * _lookSensitivity;
        float mouseY = _lookInput.y * _lookSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        if (_cameraTransform != null) {
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
        Vector3 move =
            _cameraTransform.forward * _moveInput.y + _cameraTransform.right * _moveInput.x;
        move.y = 0f;
        _rb.AddForce(move.normalized * _speed, ForceMode.VelocityChange);

        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (horizontalVelocity.magnitude > _maxSpeed) {
            horizontalVelocity = horizontalVelocity.normalized * _maxSpeed;
            _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
        }
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && Mathf.Abs(_rb.velocity.y) < 0.01f) {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        }
    }
}
