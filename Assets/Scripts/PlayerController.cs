using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5.0f;

	[SerializeField] private float sprintSpeed = 10.0f;

	//[SerializeField] private float sensitivityX;
	//[SerializeField] private float sensitivityY;
	[SerializeField] private float minimumY = -85;
	[SerializeField] private float maximumY = 80;
	[SerializeField] private float fallingSpeedDeath = 100;
	[SerializeField] private Transform myCamera = null;
	[SerializeField] private GameObject decalPrefab = null;
	[SerializeField] private float maxDistanceDraw = 3.0f;
	[SerializeField] private float maxDistanceInteract = 5.0f;
	[SerializeField] private LayerMask drawLayer = 0;
	[SerializeField] private LayerMask interactibleLayer = 0;
	[SerializeField] private float cooldDownTrace = 5.0f;

	private float _horizontalInput;
	private float _verticalInput;
	private Rigidbody _myRigidBody;
	private bool _canSprint = true;
	private float _actualSpeed;
	private float rotationY = 0.0f;
	private bool _isAlive = true;
	private bool _canMove = true;
	private bool _canTrace = true;

	public bool CanMove
	{
		get => _canMove;
		set => _canMove = value;
	}

	private bool _dieOnImpact;

	private void Start()
	{
		_myRigidBody = GetComponent<Rigidbody>();
		_actualSpeed = moveSpeed;
	}

	private void Update()
	{
		if (_isAlive && _canMove)
		{
			//handles moving inputs
			_horizontalInput = Input.GetAxis("Horizontal");
			_verticalInput = Input.GetAxis("Vertical");

			//handles sprint inputs
			if (Input.GetButtonDown("Sprint") && _canSprint)
			{
				_actualSpeed = sprintSpeed;
			}
			else if (Input.GetButtonUp("Sprint"))
			{
				_actualSpeed = moveSpeed;
			}

			//handles rotation inputs
			//code from stackoverflow.com/questions/8465323/unity-fps-rotation-camera
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") *
			                  GameManager.Instance.SensitivityX * (GameManager.Instance.InvertX ? -1 : 1);
			rotationY += Input.GetAxis("Mouse Y") * GameManager.Instance.SensitivityY *
			             (GameManager.Instance.InvertY ? -1 : 1);
			rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
			transform.localEulerAngles = new Vector3(0, rotationX, 0);
			myCamera.localEulerAngles = new Vector3(-rotationY, 0, 0);

			//checks for maxFallingSpeed
			if (_myRigidBody.velocity.y < 0 && Mathf.Abs(_myRigidBody.velocity.y) > fallingSpeedDeath)
			{
				_dieOnImpact = true;
			}

			//checks for decal
			//code from youtube.com/watch?v=VKP9APfsRAk
			if (Input.GetButtonDown("Mark") && _canTrace)
			{
				StartCoroutine(Trace());
			}
			//handles interaction with items
			else if (Input.GetButtonDown("Interact"))
			{
				RaycastHit hit;
				if (Physics.Raycast(myCamera.position, myCamera.forward, out hit, maxDistanceInteract,
					interactibleLayer))
				{
					//todo fix error here
					hit.transform.GetComponent<Interactive>().Interact();
				}
			}

			//handles pause input
			if (Input.GetButtonDown("Pause"))
			{
				//todo add select default button UI somewhere
				GameManager.Instance.UIManager.TogglePause();
			}

			if (Input.GetButtonDown("Retry"))
			{
				GameManager.Instance.ReloadCurrentScene();
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

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			Die();
		}
	}

	private IEnumerator Trace()
	{
		RaycastHit hit;
		if (Physics.Raycast(myCamera.position, myCamera.forward, out hit, maxDistanceDraw, drawLayer))
		{
			_canTrace = false;
			var decal = Instantiate(decalPrefab, hit.transform);
			decal.transform.position = hit.point;
			decal.transform.forward = hit.normal * -1f;
			decal.transform.Rotate(0, 0, Random.Range(0, 360));
			float timer = cooldDownTrace;
			while (timer > 0)
			{
				timer -= Time.deltaTime;
				GameManager.Instance.UIManager.RefreshCoolDown(timer / cooldDownTrace);
				yield return null;
			}

			_canTrace = true;
		}

		yield return null;
	}

	public void Die()
	{
		if (_isAlive)
		{
			_isAlive = false;
			GameManager.Instance.ReloadCurrentScene();
		}
	}
}