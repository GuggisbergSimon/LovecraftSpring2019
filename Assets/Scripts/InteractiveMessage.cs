using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveMessage : Interactive
{
	[SerializeField] private Message message = null;
	private bool _isReading;

	public override void Interact()
	{
		_isReading = true;
		GameManager.Instance.UIManager.PrintMessage(message);
	}

	private void OnTriggerExit(Collider other)
	{
		if (_isReading && other.CompareTag("Player"))
		{
			_isReading = false;
			GameManager.Instance.UIManager.CloseMessage();
		}
	}
}