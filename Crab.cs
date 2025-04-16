using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Crab : MonoBehaviour
{
	[Header("�⺻��")]
	[SerializeField] private int crabVersion;
	
	
	[Space(10)]
	[Header("�Ѿ�")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed = 5f;

	[SerializeField] private float minAttackDelay;
	[SerializeField] private float maxAttackDelay;

	//[SerializeField] private float fireRate = 2f; // �Ѿ��� �߻��ϴ� �ֱ�


	[Space(10)]
	[Header("�����")]
	[SerializeField] private AudioSource attackAudioSource;


	private Animator _animator;
		
	private float currentTime = 0;

	private float bulletShootTime;
	void Start()
	{
		_animator = GetComponent<Animator>();
		_animator.SetInteger("Version", crabVersion);

		// �߻� �ð� ����
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
			// �Ѿ��� �߻�
			ShootBullet();
			_animator.SetTrigger("isAttack");

			// �߻� �ð� �ʱ�ȭ
			currentTime = 0;

			// �߻� �ð� ����
			bulletShootTime = Random.Range(minAttackDelay, maxAttackDelay);
		}

	}

	void ShootBullet()
	{
		Bullet bullet = null;

		// �Ѿ��� �����ϰ� �߻� ��ġ�� �̵���Ŵ
		if (crabVersion == 1)
		{
			bullet = ObjectPool.GetObject("CrabBulletVer1");
		}
		else if(crabVersion == 2)
		{
			bullet = ObjectPool.GetObject("CrabBulletVer2");
		}

		// �Ѿ˿� �ӵ��� �ο��Ͽ� �߻�
		Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

		// �߻� ��ġ
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


		// �Ҹ�
		attackAudioSource.Play();

	}
}
