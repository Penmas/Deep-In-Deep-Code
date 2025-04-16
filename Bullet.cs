using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private string bulletType;

	private void OnDisable()
	{
		ObjectPool.ReturnObject(this, bulletType);
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{

		if (collision.CompareTag("Player"))
		{
			var _playerStateBase = collision.GetComponent<PlayerStateBase>();
			_playerStateBase.SetMinusPlayerHP(1);

			gameObject.SetActive(false);
		}
	}


	void OnBecameInvisible()
	{
		// ī�޶� ������ ������ �� ȣ��Ǵ� �Լ�
		gameObject.SetActive(false);
	}
}
