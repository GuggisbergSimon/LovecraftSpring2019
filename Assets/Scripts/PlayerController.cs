using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float sprintSpeed = 10.0f;
	[SerializeField] private float rotationSpeed = 10.0f;
	[SerializeField] private float angleLimit = 80.0f;
	[SerializeField] private Transform myCamera = null;

	private float _horizontalInput;
	private float _verticalInput;
	private Rigidbody _myRigidBody;
	private bool _canSprint = true;
	private float _actualSpeed;

	private void Start()
	{
		_myRigidBody = GetComponent<Rigidbody>();
		_actualSpeed = moveSpeed;
	}

	private void Update()
	{
		_horizontalInput = Input.GetAxis("Horizontal");
		_verticalInput = Input.GetAxis("Vertical");

		if (Input.GetButtonDown("Fire1") && _canSprint)
		{
			_actualSpeed = sprintSpeed;
		}
		else if (Input.GetButtonUp("Fire1"))
		{
			_actualSpeed = moveSpeed;
		}
		
		
		//calculate the angle between the player and the laser
		Vector2 diff = transform.right*Input.GetAxis("Mouse X");
		Quaternion focusAngle = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, focusAngle, rotationSpeed * Time.deltaTime);

	}

	private void FixedUpdate()
	{
		_myRigidBody.velocity = _actualSpeed * (transform.right.normalized * _horizontalInput +
												transform.forward.normalized * _verticalInput);
	}
}