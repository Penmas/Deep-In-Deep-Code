using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
	[SerializeField] private bool isGrappling;
	[SerializeField] private bool isUnGrappling;

	[SerializeField] private bool isEnemyAttack;
	[SerializeField] private bool hookEventEnd;              // 갈고리를 사용한 상태인가(몬스터에게 맞히거나, 오브젝트에 박히면 true)



	private EnemyController attackEnemy;

	[SerializeField] private LayerMask callBackGrapple;
	[SerializeField] private LayerMask callBackUnGrapple;


	[Space(10)]
	[SerializeField] private AudioSource hookAudioSource;

	public bool IsGrappling
	{
		set => isGrappling = value;
		get => isGrappling;
	}

	public bool IsUnGrappling
	{
		set=> isUnGrappling = value;
		get => isUnGrappling;
	}

	public bool IsEnemyAttack
	{
		set => isEnemyAttack = value;
		get => isEnemyAttack;
	}

	public bool HookEventEnd
	{
		set => hookEventEnd = value;
		get => hookEventEnd;
	}


	public EnemyController AttackEnemy
	{
		set => isEnemyAttack = value;	
		get => attackEnemy;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(hookEventEnd)
		{
			return;
		}

		if(collision.callbackLayers == callBackGrapple)
		{
			hookAudioSource.Play();
			hookEventEnd = true;
			isGrappling = true;
		}
		else if(collision.callbackLayers == callBackUnGrapple)
		{
			hookEventEnd = true;
			isUnGrappling = true;
		}
		else if(collision.tag == "Enemy")
		{
			attackEnemy = collision.GetComponent<EnemyController>();
			hookEventEnd = true;
			isEnemyAttack = true;

		}
	}


	private void OnEnable()
	{
		isGrappling = false;
		isUnGrappling = false;

		isEnemyAttack = false;
		hookEventEnd = false;
	}
}
