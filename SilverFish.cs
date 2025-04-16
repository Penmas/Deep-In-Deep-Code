using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverFish : MonoBehaviour
{

	[SerializeField] private float force = 3000;

	[Space(10)]
	[SerializeField] private AudioSource silverFishAppearAudioSource;

	private PlayerStateBase _playerStateBase;
	private Rigidbody2D _rigidbody2D;


	private bool isMoved;

	private void Awake()
	{
		_playerStateBase = FindObjectOfType<PlayerStateBase>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}


	private bool isRight;                   //현재 오른쪽에서 나왔는가


	private void Update()
	{
		if(isMoved)
		{
			return;
		}

		if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGameClear)
		{
			return;
		}

		if (Mathf.Abs(_playerStateBase.transform.position.y - transform.position.y) < 5)
		{
			isMoved = true;
			SilverFishMove();
		}
	}


	private void OnEnable()
	{

		if(transform.position.x > 0)
		{
			isRight = true;
		}
		else if(transform.position.x < 0)
		{
			isRight = false;
		}

		silverFishAppearAudioSource.Play();
	}


	private void SilverFishMove()
	{
		if(isRight)
		{
			_rigidbody2D.AddForce(transform.right * -1 * force);
		}
		else
		{
			_rigidbody2D.AddForce(transform.right * force);
		}
	}


	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			_playerStateBase.SetMinusPlayerHP(1);
		}
	}

}
