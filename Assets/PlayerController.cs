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

    private bool _isSliding = false;
    private Vector3 _steepNormal = Vector3.zero;

    private const float _epsilon = 0.001f;

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

        if (_cameraTransform != null)
        {
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
        Vector3 move =
            _cameraTransform.forward * _moveInput.y + _cameraTransform.right * _moveInput.x;
        move.y = 0f;
        Vector3 moveDir = move.normalized;

        if (moveDir.magnitude > 0)
        {
            AdjustForSlope(ref moveDir);
            _rb.AddForce(moveDir * _speed, ForceMode.VelocityChange);
        }

        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (horizontalVelocity.magnitude > _maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * _maxSpeed;
            _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
        }

        _isSliding = false;
    }

    private void AdjustForSlope(ref Vector3 moveDir)
    {
        if (!_isSliding) return;

        Vector3 horizontalNormal = new Vector3(_steepNormal.x, 0f, _steepNormal.z);
        if (horizontalNormal.sqrMagnitude <= _epsilon) return;

        horizontalNormal.Normalize();
        float dot = Vector3.Dot(moveDir, horizontalNormal);
        if (dot >= 0) return;

        moveDir -= horizontalNormal * dot;
        if (moveDir.sqrMagnitude > _epsilon) moveDir.Normalize();
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
        if (value.isPressed && Mathf.Abs(_rb.velocity.y) < _epsilon)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Slideable")) return;


        Vector3 steepNormal = Vector3.zero;
        bool hasSteepSlope = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float angle = Vector3.Angle(Vector3.up, normal);

            if (angle > 45f && angle < 89.9f)
            {
                hasSteepSlope = true;
                steepNormal += normal;
            }
        }

        if (!hasSteepSlope) return;

        steepNormal.Normalize();
        _isSliding = true;
        _steepNormal = steepNormal;

        Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, steepNormal).normalized;

        _rb.AddForce(slideDirection * 20f, ForceMode.Acceleration);

        if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        }
    }

}
