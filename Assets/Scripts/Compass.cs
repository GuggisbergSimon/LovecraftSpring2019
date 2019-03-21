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
				_goal = transform.position - _initPos;
				break;
			}
		}

		//transform.forward = _goal;
		//todo to correct that for 3D
		Quaternion targetRot = Quaternion.Euler(0f, Mathf.Atan2(_goal.z, _goal.x) * Mathf.Rad2Deg - 90, 0f);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 360);
	}
}