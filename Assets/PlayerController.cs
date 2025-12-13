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
	private Transform _cameraTransform;

	private Rigidbody _rb;
	private Vector2 _moveInput;

	void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Vector3 move =
			_cameraTransform.forward * _moveInput.y + _cameraTransform.right * _moveInput.x;
		move.y = 0f;
		_rb.AddForce(move.normalized * _speed, ForceMode.VelocityChange);

		Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
		if (horizontalVelocity.magnitude > _maxSpeed) {
			horizontalVelocity = horizontalVelocity.normalized * _maxSpeed;
			_rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);
		}
	}

	public void OnMove(InputValue value)
	{
		_moveInput = value.Get<Vector2>();
	}

	public void OnJump(InputValue value)
	{
		if (value.isPressed && Mathf.Abs(_rb.linearVelocity.y) < 0.01f) {
			_rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
		}
	}
}
