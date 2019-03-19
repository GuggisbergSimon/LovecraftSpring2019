using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpNote : Interactive
{
	[SerializeField] private Message pickUpMessage = null;
	[SerializeField] private Message noPickUpMessage = null;
	[SerializeField] private GameObject modelToHide = null;
	private bool _isPicked;

	public override void Interact()
	{
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
}