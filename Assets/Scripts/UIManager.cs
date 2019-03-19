using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private float fadingToBlackTime = 0.5f;
	[SerializeField] private Image blackPanel = null;
	[SerializeField] private GameObject dialoguePanelBottom = null;
	[SerializeField] private TextMeshProUGUI textDisplayedBottom = null;
	[SerializeField] private GameObject dialoguePanelTop = null;
	[SerializeField] private TextMeshProUGUI textDisplayedTop = null;
	private Coroutine _currentDialogueTop;
	private Coroutine _currentDialogueBottom;
	private Message _currentMessage;
	private bool _isFadingToBlack;
	public bool IsFadingToBlack => _isFadingToBlack;

	public enum DialoguePosition
	{
		Top,
		Bottom
	}

	public void FadeToBlack(bool value)
	{
		StartCoroutine(FadingToBlack(value, fadingToBlackTime));
	}

	private IEnumerator FadingToBlack(bool value, float time)
	{
		_isFadingToBlack = true;
		//blackPanel.gameObject.SetActive(true);
		float timer = 0.0f;
		Color tempColor = blackPanel.color;
		while (timer < time)
		{
			timer += Time.unscaledDeltaTime;
			tempColor.a = Mathf.Lerp(value ? 0.0f : 1.0f, value ? 1.0f : 0.0f, timer / time);
			blackPanel.color = tempColor;
			yield return null;
		}

		//blackPanel.gameObject.SetActive(value);
		_isFadingToBlack = false;
	}

	public void PrintMessage(Message message)
	{
		PrintMessage(message, DialoguePosition.Bottom);
	}

	public void PrintMessage(Message message, DialoguePosition dialoguePosition)
	{
		GameObject dialoguePanel;
		TextMeshProUGUI textDisplayed;
		Coroutine tempCoroutine;
		if (dialoguePosition == DialoguePosition.Top)
		{
			dialoguePanel = dialoguePanelTop;
			textDisplayed = textDisplayedTop;
			tempCoroutine = _currentDialogueTop;
		}
		else
		{
			dialoguePanel = dialoguePanelBottom;
			textDisplayed = textDisplayedBottom;
			tempCoroutine = _currentDialogueBottom;
		}

		dialoguePanel.SetActive(true);
		if (tempCoroutine != null)
		{
			StopCoroutine(tempCoroutine);
		}

		_currentMessage = message;

		textDisplayed.color = _currentMessage.color;
		if (message.timeBetweenLetters.CompareTo(0) != 0)
		{
			tempCoroutine = StartCoroutine(PrintLetterByLetter(dialoguePosition));
		}
		else
		{
			PrintAll(dialoguePosition);
		}
	}

	public void CloseMessage()
	{
		CloseMessage(DialoguePosition.Bottom);
	}

	public void CloseMessage(DialoguePosition dialoguePosition)
	{
		GameObject dialoguePanel;
		if (dialoguePosition == DialoguePosition.Top)
		{
			dialoguePanel = dialoguePanelTop;
		}
		else
		{
			dialoguePanel = dialoguePanelBottom;
		}

		dialoguePanel.SetActive(false);
	}

	private IEnumerator PrintLetterByLetter(DialoguePosition dialoguePosition)
	{
		TextMeshProUGUI textDisplayed;
		if (dialoguePosition == DialoguePosition.Top)
		{
			textDisplayed = textDisplayedTop;
		}
		else
		{
			textDisplayed = textDisplayedBottom;
		}

		textDisplayed.text = "";
		for (int i = 0; i < _currentMessage.text.Length; i++)
		{
			textDisplayed.text += _currentMessage.text[i];
			yield return new WaitForSeconds(_currentMessage.timeBetweenLetters);
		}
	}

	private void PrintAll(DialoguePosition dialoguePosition)
	{
		TextMeshProUGUI textDisplayed;
		if (dialoguePosition == DialoguePosition.Top)
		{
			textDisplayed = textDisplayedTop;
		}
		else
		{
			textDisplayed = textDisplayedBottom;
		}

		textDisplayed.text = _currentMessage.text;
	}
}