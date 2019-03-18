using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadScene : MonoBehaviour
{
	[SerializeField] private string nameLevel = null;

	private void Start()
	{
		//todo loading scene frame no ui then change second value to true
		GameManager.Instance.LoadLevel(nameLevel, false, false);
	}
}