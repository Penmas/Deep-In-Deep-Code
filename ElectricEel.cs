using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public enum ElectricEelState
{
	None,
	Move,
	Attack,
}

public class ElectricEel : MonoBehaviour
{
	[Header("기본값")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float moveDistance;

	[Space(10)]
	[Header("공격")]
	[SerializeField] private float attackRange;
	[SerializeField] private float minAttackDelay;
	[SerializeField] private float maxAttackDelay;


	private SpriteRenderer _spriteRenderer;
	private Animator _animator;
	private Rigidbody2D _rigidbody2D;

	private Vector2 defaultPosition;

	private int direction;          // -1 : 왼쪽방향으로 직진. /1 : 오른쪽방향으로 직진

	private float attackTime;				// 공격 시간
	private float currentAttackTime;		// 이전 공격에서 지난 시간

	private ElectricEelState currentElectricEelState;

	private void Awake()
	{
		direction = -1;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		defaultPosition = transform.position;

		currentElectricEelState = ElectricEelState.Move;
		AttackSelect();
	}

	private void Update()
	{
		// 왼쪽 끝 부분 도착
		if (transform.position.x < defaultPosition.x - moveDistance)
		{
			_spriteRenderer.flipX = true;
			direction = 1;
		}
		else if (transform.position.x > defaultPosition.x + moveDistance)
		{
			_spriteRenderer.flipX = false;
			direction = -1;
		}

		ElectricEelControl();
	}



	private void ElectricEelControl()
	{
		switch(currentElectricEelState)
		{
			case ElectricEelState.Move:
				MonsterMove();



				currentAttackTime += Time.deltaTime;
				if (currentAttackTime > attackTime)
				{
					AttackSelect();
					currentAttackTime = 0;
					currentElectricEelState = ElectricEelState.Attack;
					_animator.SetTrigger("isAttack");
				}


				break;
			case ElectricEelState.Attack:
				Attack();
				break;
		}
	}



	private void MonsterMove()
	{
		_rigidbody2D.velocity = Vector2.right * moveSpeed * direction;
	}


	private void AttackSelect()
	{
		attackTime = Random.Range(minAttackDelay, maxAttackDelay);
	}

	private void Attack()
	{
		_rigidbody2D.velocity = Vector2.zero;

		// 중점으로 가상의 원을 생성해, 그 안에 있는 콜라이더 검사
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
		
		
		if (colliders.Length > 0) 
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				// v플레이어인지 판단
				if (colliders[i].tag == "Player")
				{
					colliders[i].GetComponent<PlayerStateBase>().SetMinusPlayerHP(1); 
					break;
				}
			}
		}
	}


	// 공격이 끝나면 Idle 상태로 돌아옴
	public void AttackEnd()
	{
		currentElectricEelState = ElectricEelState.Move;
	}

}
