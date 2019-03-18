using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
	[SerializeField] private Message message = null;
	private bool _isRead;

	private void OnTriggerEnter(Collider other)
	{
		//todo add check for player (?)
		if (!_isRead)
		{
			GameManager.Instance.UIManager.PrintMessage(message);
			if (message.maxTimeOnScreen > 0)
			{
				StartCoroutine(WillCloseMessageIn(message.maxTimeOnScreen));
			}
		}
	}

	private IEnumerator WillCloseMessageIn(float time)
	{
		yield return new WaitForSeconds(time);
		if (!_isRead)
		{
			CloseMessage();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		//todo add check for player (?)
		CloseMessage();
	}

	private void CloseMessage()
	{
		GameManager.Instance.UIManager.CloseMessage();
		_isRead = true;
	}
}