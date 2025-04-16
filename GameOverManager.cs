using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
	[SerializeField] private Image blackImage;
	[SerializeField] private Image[] gameoverImage;
	[SerializeField] private GameObject restartButtonGameObject;


	public void BlakcImageFadeOut()
	{
		StartCoroutine("BlakcImageFadeOutCoroutine");
	}

	public void RestartButtonActive()
	{
		restartButtonGameObject.SetActive(true);
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private IEnumerator BlakcImageFadeOutCoroutine()
	{
		yield return StartCoroutine(FadeOut(blackImage));

		yield return StartCoroutine("GameoverTextImageFadeOutCoroutine");

		yield return new WaitForSeconds(0.1f);

		RestartButtonActive();

		yield return null;
	}

	private IEnumerator GameoverTextImageFadeOutCoroutine()
	{
		for (int i = 0; i < gameoverImage.Length; i++)
		{
			StartCoroutine(FadeOut(gameoverImage[i]));

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
