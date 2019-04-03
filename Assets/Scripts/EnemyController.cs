using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private float radiusAlertView = 20.0f;
	[SerializeField] private float radiusEscapeView = 30.0f;
	[SerializeField] private float alertSpeed = 4.0f;
	[SerializeField] private float routineSpeed = 2.0f;
	private Rigidbody _myRigidBody;
	private LevelBuilder.Node _currentTower;
	private List<LevelBuilder.Node> _towers;
	private States _myState;
	private Coroutine _movingCoroutine;

	enum States
	{
		Routine,
		Alert
	}

	private void Start()
	{
		//_towers = GameManager.Instance.LevelBuilder.Towers;
		_myRigidBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		float diff = Vector3.Distance(GameManager.Instance.Player.transform.position, transform.position);
		Vector3 aim = Vector3.zero;
		//set and update currentTower correctly
		_currentTower = _towers[0];
		foreach (var tower in _towers)
		{
			if (Vector3.Distance(transform.position, tower.position) <
			    Vector3.Distance(transform.position, _currentTower.position))
			{
				_currentTower = tower;
			}
		}
		
		switch (_myState)
		{
			case States.Routine:
			{
				if (diff < radiusAlertView)
				{
					_myState = States.Alert;
				}
				else
				{
					//todo move randomly depending on neighbors
				}

				break;
			}
			case States.Alert:
			{
				if (diff > radiusEscapeView)
				{
					_myState = States.Routine;
				}
				else
				{
					//todo determine next node through pathfinding
					aim = GameManager.Instance.Player.transform.position;
				}

				break;
			}
		}

		_myRigidBody.velocity = (aim - transform.position).normalized * alertSpeed;
	}

	private IEnumerator Move(Vector3 aim)
	{
		yield return null;
	}
}