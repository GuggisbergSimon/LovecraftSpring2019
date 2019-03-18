using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
	[SerializeField] private Vector3 compassAxis = Vector3.forward;
	private void Update()
	{
		transform.forward = compassAxis;
	}
}