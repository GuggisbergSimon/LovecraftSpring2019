using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusActions : MonoBehaviour
{
	public void LoadLevelFadeInAndOut(string nameLevel)
	{
		GameManager.Instance.LoadLevelFadeInAndOut(nameLevel);
	}

	public void QuitGame()
	{
		GameManager.Instance.QuitGame();
	}

	public void ChangeTimeScale(float timeScale)
	{
		GameManager.Instance.ChangeTimeScale(timeScale);
	}
}