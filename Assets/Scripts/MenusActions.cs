using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusActions : MonoBehaviour
{
	[SerializeField] private Slider sensitivityXSlider = null;
	[SerializeField] private Slider sensitivityYSlider = null;
	[SerializeField] private Toggle invertXToggle = null;
	[SerializeField] private Toggle invertYToggle = null;

	public void ChangeSensitivityX(float value)
	{
		GameManager.Instance.SensitivityX = value;
	}
	
	public void ChangeSensitivityY(float value)
	{
		GameManager.Instance.SensitivityY = value;
	}

	public void ToggleInvertX(bool value)
	{
		GameManager.Instance.InvertX = value;
	}

	public void ToggleInvertY(bool value)
	{
		GameManager.Instance.InvertY = value;
	}

	public void SetOptions()
	{
		sensitivityXSlider.value = GameManager.Instance.SensitivityX;
		sensitivityYSlider.value = GameManager.Instance.SensitivityY;
		invertXToggle.isOn = GameManager.Instance.InvertX;
		invertYToggle.isOn = GameManager.Instance.InvertY;
	}
	
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