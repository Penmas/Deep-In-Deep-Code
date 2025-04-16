using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
	[SerializeField] private Image blackImage;
	[SerializeField] private Image[] gameClearImage;
	[SerializeField] private GameObject restartButtonGameObject;
	[SerializeField] private GameObject clearTriggerGameObject;

	public void ClearZoneEnable()
	{
		clearTriggerGameObject.SetActive(true);
	}

	public void BlakcImageFadeOut()
	{
		GameManager.Instance.IsGameClear = true;
		StartCoroutine("BlakcImageFadeOutCoroutine");
	}

	public void RestartButtonActive()
	{
		restartButtonGameObject.SetActive(true);
	}

	public void Restart()
	{
		SceneManager.LoadScene("TitleScene");
	}

	private IEnumerator BlakcImageFadeOutCoroutine()
	{
		yield return StartCoroutine(FadeOut(blackImage));

		yield return StartCoroutine("GameclearTextImageFadeOutCoroutine");

		yield return new WaitForSeconds(0.1f);

		RestartButtonActive();

		yield return null;
	}

	private IEnumerator GameclearTextImageFadeOutCoroutine()
	{
		for (int i = 0; i < gameClearImage.Length; i++)
		{
			StartCoroutine(FadeOut(gameClearImage[i]));

			yield return new WaitForSeconds(0.12f);
		}
	}


	private IEnumerator FadeOut(Image FadeImage)
	{
		float alpha = 0;
		Color myColor = FadeImage.color;
		while (true)
		{
			FadeImage.color = new Color(myColor.r, myColor.g, myColor.b, alpha);
			alpha += 0.03f;

			if(alpha >= 1f)
			{
				FadeImage.color = new Color(myColor.r, myColor.g, myColor.b, 1);
				break;
			}

			yield return new WaitForSeconds(0.03f);
		}
	}
}
