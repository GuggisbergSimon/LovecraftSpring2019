using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpNote : Interactive
{
	[SerializeField] private Message pickUpMessage = null;
	[SerializeField] private Message noPickUpMessage = null;
	[SerializeField] private GameObject modelToHide = null;
	private bool _isPicked;
	private bool _isReading;

	public override void Interact()
	{
		_isReading = true;
		if (!_isPicked)
		{
			_isPicked = true;
			GameManager.Instance.UIManager.PrintMessage(pickUpMessage);
			GameManager.Instance.CurrentNumberNotes++;
			modelToHide.SetActive(false);
		}
		else
		{
			GameManager.Instance.UIManager.PrintMessage(noPickUpMessage);
		}
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