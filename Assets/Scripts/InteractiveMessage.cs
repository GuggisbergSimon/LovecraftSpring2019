using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveMessage : Interactive
{
	[SerializeField] private Message message = null;

	public override void Interact()
	{
		GameManager.Instance.UIManager.PrintMessage(message);
	}
}