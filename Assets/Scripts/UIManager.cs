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
	[SerializeField] private GameObject pausePanel = null;
	private Coroutine _currentDialogueTop;
	private Coroutine _currentDialogueBottom;
	private bool _isFadingToBlack;
	public bool IsFadingToBlack => _isFadingToBlack;

	public void FadeToBlack(bool value)
	{
		StartCoroutine(FadingToBlack(value, fadingToBlackTime));
	}

	private IEnumerator FadingToBlack(bool value, float time)
	{
		_isFadingToBlack = true;
		blackPanel.gameObject.SetActive(true);
		float timer = 0.0f;
		Color tempColor = blackPanel.color;
		while (timer < time)
		{
			timer += Time.unscaledDeltaTime;
			tempColor.a = Mathf.Lerp(value ? 0.0f : 1.0f, value ? 1.0f : 0.0f, timer / time);
			blackPanel.color = tempColor;
			yield return null;
		}

		blackPanel.gameObject.SetActive(value);
		_isFadingToBlack = false;
	}

	public void PrintPopUp(Message message)
	{
		dialoguePanelTop.SetActive(true);
		if (_currentDialogueTop != null)
		{
			StopCoroutine(_currentDialogueTop);
		}

		textDisplayedTop.color = message.color;
		if (message.timeBetweenLetters.CompareTo(0) != 0)
		{
			_currentDialogueTop = StartCoroutine(PrintLetterByLetter(textDisplayedTop, message));
		}
		else
		{
			PrintAll(textDisplayedTop, message);
		}

		if (message.maxTimeOnScreen > 0)
		{
			Invoke("ClosePopUp", message.maxTimeOnScreen);
		}
	}

	public void PrintMessage(Message message)
	{
		dialoguePanelBottom.SetActive(true);
		if (_currentDialogueBottom != null)
		{
			StopCoroutine(_currentDialogueBottom);
		}

		textDisplayedBottom.color = message.color;
		if (message.timeBetweenLetters.CompareTo(0) != 0)
		{
			_currentDialogueBottom = StartCoroutine(PrintLetterByLetter(textDisplayedBottom, message));
		}
		else
		{
			PrintAll(textDisplayedBottom, message);
		}


		if (message.maxTimeOnScreen > 0)
		{
			Invoke("CloseMessage", message.maxTimeOnScreen);
		}
	}

	private void ClosePopUp()
	{
		dialoguePanelTop.SetActive(false);
	}

	public void CloseMessage()
	{
		dialoguePanelBottom.SetActive(false);
	}

	public void TogglePause()
	{
		pausePanel.SetActive(!pausePanel.activeSelf);
		GameManager.Instance.Player.CanMove = !pausePanel.activeSelf;
		GameManager.Instance.ChangeTimeScale(pausePanel.activeSelf ? 0.0f : 1.0f);
	}

	private IEnumerator PrintLetterByLetter(TextMeshProUGUI textDisplayed, Message currentMessage)
	{
		textDisplayed.text = "";
		for (int i = 0; i < currentMessage.text.Length; i++)
		{
			textDisplayed.text += currentMessage.text[i];
			yield return new WaitForSeconds(currentMessage.timeBetweenLetters);
		}
	}

	private void PrintAll(TextMeshProUGUI textDisplayed, Message currentMessage)
	{
		textDisplayed.text = currentMessage.text;
	}
}