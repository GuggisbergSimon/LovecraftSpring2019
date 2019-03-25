using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
	[SerializeField] private CompassMode compassMode = 0;
	[SerializeField] private Vector3 northDirection = Vector3.forward;
	private Vector3 _goal;
	private Vector3 _initPos;

	private enum CompassMode
	{
		North,
		OriginPlayer,
		ClosestPickUp
	}

	private void Start()
	{
		_initPos = transform.position;
	}

	private void Update()
	{
		switch (compassMode)
		{
			case CompassMode.North:
			{
				_goal = northDirection;
				break;
			}
			case CompassMode.OriginPlayer:
			{
				_goal = _initPos - transform.position;
				_goal = _goal.x * Vector3.right + _goal.z * Vector3.forward;
				break;
			}
		}

		if (!_goal.Equals(Vector3.zero))
		{
			transform.forward = _goal;
		}
	}
}