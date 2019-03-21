using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
	//todo make compass point to origin of player ?
	[SerializeField] private Vector3 compassAxis = Vector3.forward;
	
	private void Update()
	{
		transform.forward = compassAxis;
	}
}