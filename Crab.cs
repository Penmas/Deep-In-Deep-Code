using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Crab : MonoBehaviour
{
	[Header("기본값")]
	[SerializeField] private int crabVersion;
	
	
	[Space(10)]
	[Header("총알")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed = 5f;

	[SerializeField] private float minAttackDelay;
	[SerializeField] private float maxAttackDelay;

	//[SerializeField] private float fireRate = 2f; // 총알을 발사하는 주기


	[Space(10)]
	[Header("오디오")]
	[SerializeField] private AudioSource attackAudioSource;


	private Animator _animator;
		
	private float currentTime = 0;

	private float bulletShootTime;
	void Start()
	{
		_animator = GetComponent<Animator>();
		_animator.SetInteger("Version", crabVersion);

		// 발사 시간 결정
		bulletShootTime = Random.Range(minAttackDelay, maxAttackDelay);

	}


	void Update()
	{
        if (GameManager.Instance.IsGameClear || GameManager.Instance.IsGameOver)
        {
			return;
        }

        currentTime += Time.deltaTime;

		if (currentTime > bulletShootTime)
		{
			// 총알을 발사
			ShootBullet();
			_animator.SetTrigger("isAttack");

			// 발사 시간 초기화
			currentTime = 0;

			// 발사 시간 결정
			bulletShootTime = Random.Range(minAttackDelay, maxAttackDelay);
		}

	}

	void ShootBullet()
	{
		Bullet bullet = null;

		// 총알을 생성하고 발사 위치로 이동시킴
		if (crabVersion == 1)
		{
			bullet = ObjectPool.GetObject("CrabBulletVer1");
		}
		else if(crabVersion == 2)
		{
			bullet = ObjectPool.GetObject("CrabBulletVer2");
		}

		// 총알에 속도를 부여하여 발사
		Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

		// 발사 위치
		Vector2 attackPos = transform.position + new Vector3(0, 1f, 0);
		if (transform.position.x < 0)
		{
			//bullet.transform.eulerAngles = new Vector3(0, 0, -90);
			bullet.gameObject.transform.position = attackPos;
			bullet.gameObject.SetActive(true);

			bulletRb.velocity = Vector2.right * bulletSpeed;
		}
		else
		{
			//bullet.transform.eulerAngles = new Vector3(0, 0, 90);

			bullet.gameObject.transform.position = attackPos;
			bullet.gameObject.SetActive(true);

			bulletRb.velocity = Vector2.left * bulletSpeed;

		}


		// 소리
		attackAudioSource.Play();

	}
}
