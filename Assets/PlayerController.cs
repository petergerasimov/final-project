using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[DefaultExecutionOrder(-100)]
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
    private float _deceleration = 20f;
    [SerializeField]
    private float _airDrag = 3f;

    [SerializeField]
    private Transform _cameraTransform;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _xRotation = 0f;

    private bool _isGrounded = false;
    private bool _isSliding = false;
    private bool _isTouchingWall = false;
    private Vector3 _steepNormal = Vector3.zero;
    private Vector3 _groundNormal = Vector3.up;
    private Vector3 _wallNormal = Vector3.zero;

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

            if (_isTouchingWall && !_isGrounded)
            {
                float dot = Vector3.Dot(moveDir, _wallNormal);
                if (dot < 0) moveDir -= _wallNormal * dot;
                if (moveDir.sqrMagnitude > _epsilon) moveDir.Normalize();
            }

            if (_isGrounded) moveDir = Vector3.ProjectOnPlane(moveDir, _groundNormal).normalized;

            _rb.AddForce(moveDir * _speed, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 hVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            hVel = Vector3.MoveTowards(hVel, Vector3.zero, _deceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector3(hVel.x, _rb.velocity.y, hVel.z);
        }

        if (!_isGrounded)
        {
            Vector3 vel = _rb.velocity;
            float yVel = vel.y;
            vel -= vel * _airDrag * Time.fixedDeltaTime;
            if (yVel < 0f) vel.y = yVel;
            _rb.velocity = vel;
        }

        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (horizontalVelocity.magnitude > _maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * _maxSpeed;
            _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
        }

        _isGrounded = false;
        _isSliding = false;
        _isTouchingWall = false;
        _groundNormal = Vector3.up;
        _wallNormal = Vector3.zero;
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
        if (value.isPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float angle = Vector3.Angle(Vector3.up, normal);
            if (angle < 45f)
            {
                _isGrounded = true;
                _groundNormal = normal;
            }
            else
            {
                _isTouchingWall = true;
                _wallNormal += normal;
            }
        }

        if (_isTouchingWall)
            _wallNormal.Normalize();

        if (collision.gameObject.CompareTag("Slideable"))
        {
            HandleSlideable(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }

    private void HandleSlideable(Collision collision)
    {
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
