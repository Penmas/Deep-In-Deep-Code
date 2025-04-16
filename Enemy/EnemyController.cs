using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class EnemyController : MonoBehaviour
{
	[Header("수치")]
	[SerializeField] private int deaultHP;
	[SerializeField] private float invincibilityTime;			//무적시간


	[Space(10)]
	[Header("스프라이트 렌더러")]
	[SerializeField] private SpriteRenderer _spriteRenderer;

	[Space(10)]
	[Header("사망 이벤트")]
	public UnityEvent EnemyDeath;


	private int currentHP;
	private bool isDeath;

	private bool isInvincibilityTime;
	private float time;

	public bool IsDeath
	{
		get => isDeath;
	}

	public SpriteRenderer _SpriteRenderer
	{
		get => _spriteRenderer;
	}



	private void Awake()
	{
		currentHP = deaultHP;
	}

	private void Update()
	{
		if(isDeath)
		{
			return;
		}


		if(currentHP <= 0)
		{
			isDeath = true;
			EnemyDeath.Invoke();
		}
	}



	public void HitEnemy(int power)
	{
        if (isInvincibilityTime)
        {
			return;
        }
		isInvincibilityTime = true;

		int hp = currentHP - power;
		hp = Mathf.Clamp(hp, 0, deaultHP);
		currentHP = hp;

		if(currentHP < 1)
		{
			return;
		}

		StartCoroutine("HitEffect");
		StartCoroutine(Invincibility(invincibilityTime));
	}


	private IEnumerator HitEffect()
	{
		float time = 0;
		float currentColor = 1;

		for(int i =  0; i < 2; ++i)
		{
			currentColor = 1f;

			Debug.Log("붉어짐");
			// 피격시 spriteRenderer를 조작하여 색깔 붉은 색으로 변화
			while (true)
			{
				// 시간 텀
				yield return new WaitForSeconds(0.01f * invincibilityTime);
				currentColor -= (0.02f);

				if (currentColor <= 0.5f)
				{
					_spriteRenderer.color = new Color(1f, currentColor, currentColor, 1f);
					break;
				}
				_spriteRenderer.color = new Color(1f, currentColor, currentColor, 1f);

				yield return null;
			}



			currentColor = 0.5f;

			// 피격시 spriteRenderer를 조작하여 색깔 원래 색으로 변화
			while (true)
			{
				// 시간 텀
				yield return new WaitForSeconds(0.01f);
				currentColor += (0.02f / invincibilityTime);


				if (currentColor >= 1f)
				{
					_spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
					break;
				}
				_spriteRenderer.color = new Color(1f, currentColor, currentColor, 1f);

				yield return null;
			}	
		}
	}


	private IEnumerator Invincibility(float time)
	{
		float currentTime = 0f;

		while(true)
		{
			currentTime += Time.deltaTime;

			if(currentTime >= time)
			{
				break;
			}
			yield return null;
		}

		isInvincibilityTime = false;
	}



	public void DeathDisable()
	{
		StartCoroutine("DeathDisableCoroutine");
	}

	private IEnumerator DeathDisableCoroutine()
	{
		float currentAlpha = 1f;

		while(true)
		{
			yield return new WaitForSeconds(0.01f);

			currentAlpha -= 0.05f;
			_spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);

			if (currentAlpha <= 0f)
			{
				_spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
				break;
			}
		}

		gameObject.SetActive(false);
	}
}
