using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float _speed = 5f;

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
	}

	public void OnMove(InputValue value)
	{
		_moveInput = value.Get<Vector2>();
		//_moveInput = context.ReadValue<Vector2>();
	}
}
