using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[DefaultExecutionOrder(-100)]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 5f;
    [SerializeField]
    private float m_maxSpeed = 7f;
    [SerializeField]
    private float m_jumpForce = 5f;
    [SerializeField]
    private float m_lookSensitivity = 0.5f;
    [SerializeField]
    private float m_deceleration = 20f;
    [SerializeField]
    private float m_airDrag = 3f;

    [SerializeField]
    private Transform m_cameraTransform;

    private Rigidbody m_rb;
    private Vector2 m_moveInput;
    private Vector2 m_lookInput;
    private float m_xRotation = 0f;

    private bool m_isGrounded;
    private bool m_isSliding;
    private bool m_isTouchingWall;
    private Vector3 m_steepNormal = Vector3.zero;
    private Vector3 m_groundNormal = Vector3.up;
    private Vector3 m_wallNormal = Vector3.zero;

    private const float k_Epsilon = 0.001f;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = m_lookInput.x * m_lookSensitivity;
        float mouseY = m_lookInput.y * m_lookSensitivity;

        m_xRotation -= mouseY;
        m_xRotation = Mathf.Clamp(m_xRotation, -90f, 90f);

        if (m_cameraTransform != null)
        {
            m_cameraTransform.localRotation = Quaternion.Euler(m_xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
        Vector3 move =
            m_cameraTransform.forward * m_moveInput.y + m_cameraTransform.right * m_moveInput.x;
        move.y = 0f;
        Vector3 moveDir = move.normalized;

        if (moveDir.magnitude > 0)
        {
            AdjustForSlope(ref moveDir);

            if (m_isTouchingWall && !m_isGrounded)
            {
                float dot = Vector3.Dot(moveDir, m_wallNormal);
                if (dot < 0) moveDir -= m_wallNormal * dot;
                if (moveDir.sqrMagnitude > k_Epsilon) moveDir.Normalize();
            }

            if (m_isGrounded) moveDir = Vector3.ProjectOnPlane(moveDir, m_groundNormal).normalized;

            m_rb.AddForce(moveDir * m_speed, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 hVel = new Vector3(m_rb.velocity.x, 0f, m_rb.velocity.z);
            hVel = Vector3.MoveTowards(hVel, Vector3.zero, m_deceleration * Time.fixedDeltaTime);
            m_rb.velocity = new Vector3(hVel.x, m_rb.velocity.y, hVel.z);
        }

        if (!m_isGrounded)
        {
            Vector3 vel = m_rb.velocity;
            float yVel = vel.y;
            vel -= vel * m_airDrag * Time.fixedDeltaTime;
            if (yVel < 0f) vel.y = yVel;
            m_rb.velocity = vel;
        }

        Vector3 horizontalVelocity = new Vector3(m_rb.velocity.x, 0f, m_rb.velocity.z);
        if (horizontalVelocity.magnitude > m_maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * m_maxSpeed;
            m_rb.velocity = new Vector3(horizontalVelocity.x, m_rb.velocity.y, horizontalVelocity.z);
        }

        m_isGrounded = false;
        m_isSliding = false;
        m_isTouchingWall = false;
        m_groundNormal = Vector3.up;
        m_wallNormal = Vector3.zero;
    }

    private void AdjustForSlope(ref Vector3 moveDir)
    {
        if (!m_isSliding) return;

        Vector3 horizontalNormal = new Vector3(m_steepNormal.x, 0f, m_steepNormal.z);
        if (horizontalNormal.sqrMagnitude <= k_Epsilon) return;

        horizontalNormal.Normalize();
        float dot = Vector3.Dot(moveDir, horizontalNormal);
        if (dot >= 0) return;

        moveDir -= horizontalNormal * dot;
        if (moveDir.sqrMagnitude > k_Epsilon) moveDir.Normalize();
    }

    public void OnMove(InputValue value)
    {
        m_moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        m_lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && m_isGrounded)
        {
            m_rb.AddForce(Vector3.up * m_jumpForce, ForceMode.VelocityChange);
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
                m_isGrounded = true;
                m_groundNormal = normal;
            }
            else
            {
                m_isTouchingWall = true;
                m_wallNormal += normal;
            }
        }

        if (m_isTouchingWall)
            m_wallNormal.Normalize();

        if (collision.gameObject.CompareTag("Slideable"))
        {
            HandleSlideable(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        m_isGrounded = false;
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
        m_isSliding = true;
        m_steepNormal = steepNormal;

        Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, steepNormal).normalized;

        m_rb.AddForce(slideDirection * 20f, ForceMode.Acceleration);

        if (m_rb.velocity.y > 0)
        {
            m_rb.velocity = new Vector3(m_rb.velocity.x, 0f, m_rb.velocity.z);
        }
    }

}
