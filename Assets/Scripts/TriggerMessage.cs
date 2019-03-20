using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
	[SerializeField] private Message message = null;
	private bool _isRead;

	private void OnTriggerEnter(Collider other)
	{
		if (!_isRead && other.CompareTag("Player"))
		{
			_isRead = true;
			GameManager.Instance.UIManager.PrintPopUp(message);
		}
	}
}