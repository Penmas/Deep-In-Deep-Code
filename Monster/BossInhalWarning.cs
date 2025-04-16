using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossInhalWarning : WarningDisplay
{
	[SerializeField] private SpriteRenderer warningGameObject;

	private GameObject _playerGameobject;

	private void Start()
	{
		_playerGameobject = FindObjectOfType<PlayerController>().gameObject;
	}

	public override IEnumerator AttackWarning()
	{
		warningGameObject.transform.position = new Vector2(0, _playerGameobject.transform.position.y);
		warningGameObject.gameObject.SetActive(true);

		Color myColor = warningGameObject.color;

		float time = 0;
		float myAlpha = 0;


		for (int i = 0; i < 2; i++)
		{
			while (true)
			{
				time += Time.deltaTime;
				myAlpha += 0.05f;

				if (myAlpha >= 0.5)
				{
					warningGameObject.color = new Color(myColor.r, myColor.g, myColor.b, 0.5f);
					break;
				}
				warningGameObject.color = new Color(myColor.r, myColor.g, myColor.b, myAlpha);
				yield return new WaitForSeconds(0.05f);
			}

			time = 0;
			while (true)
			{
				time += Time.deltaTime;
				myAlpha -= 0.05f;

				if (myAlpha <= 0f)
				{
					warningGameObject.color = new Color(myColor.r, myColor.g, myColor.b, 0f);
					break;
				}
				warningGameObject.color = new Color(myColor.r, myColor.g, myColor.b, myAlpha);
				yield return new WaitForSeconds(0.05f);
			}

		}

		warningGameObject.gameObject.SetActive(false);
	}
}
