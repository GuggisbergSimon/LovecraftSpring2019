﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float sprintSpeed = 10.0f;
	[SerializeField] private float sensitivityX;
	[SerializeField] private float sensitivityY;
	[SerializeField] private float minimumY = -85;
	[SerializeField] private float maximumY = 80;
	[SerializeField] private float fallingSpeedDeath = 100;
	[SerializeField] private Transform myCamera = null;

	private float _horizontalInput;
	private float _verticalInput;
	private Rigidbody _myRigidBody;
	private bool _canSprint = true;
	private float _actualSpeed;
	private float rotationY = 0.0f;
	private bool _isAlive = true;
	private bool _dieOnImpact;

	private void Start()
	{
		_myRigidBody = GetComponent<Rigidbody>();
		_actualSpeed = moveSpeed;
	}

	private void Update()
	{
		if (_isAlive)
		{
			//handles moving inputs
			_horizontalInput = Input.GetAxis("Horizontal");
			_verticalInput = Input.GetAxis("Vertical");

			//handles sprint inputs
			if (Input.GetButtonDown("Fire1") && _canSprint)
			{
				_actualSpeed = sprintSpeed;
			}
			else if (Input.GetButtonUp("Fire1"))
			{
				_actualSpeed = moveSpeed;
			}

			//handles rotation inputs
			//code from stackoverflow.com/questions/8465323/unity-fps-rotation-camera
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
			transform.localEulerAngles = new Vector3(0, rotationX, 0);
			myCamera.localEulerAngles = new Vector3(-rotationY, 0, 0);

			//checks for maxFallingSpeed
			if (_myRigidBody.velocity.y < 0 && Mathf.Abs(_myRigidBody.velocity.y) > fallingSpeedDeath)
			{
				_dieOnImpact = true;
			}
		}
	}

	private void FixedUpdate()
	{
		//applies moving inputs
		_myRigidBody.velocity =
			_actualSpeed * (transform.right.normalized * _horizontalInput +
			                transform.forward.normalized * _verticalInput) + Vector3.up * _myRigidBody.velocity.y;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (_dieOnImpact)
		{
			Die();
		}
	}

	public void Die()
	{
		if (_isAlive)
		{
			_isAlive = false;
			Debug.Log("Death of the player");
			//todo implement actual death here
		}
	}
}