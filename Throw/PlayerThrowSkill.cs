using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowSkill : MonoBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private PlayerStateBase playerStatebase;
	[SerializeField] private PlayerController playerController;

	[SerializeField] [Tooltip("���� �� �ִ� �Ÿ�")] private float throwRange;                //������ ���� �Ÿ�
	[SerializeField] [Tooltip("������ ����")] private float throwPower;
	[SerializeField] [Tooltip("������ ���� ��")] private GameObject throwViewGameobject;
	[SerializeField] [Tooltip("������ ���� ȭ��ǥ")] private GameObject throwArrow;

	private bool isClick;                                   // ���� Ŭ�� ������ �Ǵ�
	private GameObject throwObject;                         // ������ ������Ʈ
	private LineRenderer line;

	private Vector2 throwObjectDefaultPos;

	private void Awake()
	{
		line = GetComponent<LineRenderer>();

		playerStatebase = GetComponent<PlayerStateBase>();
		playerController = GetComponent<PlayerController>();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1) & !playerStatebase.IsGrapping)
		{
			playerStatebase.IsGrapping = true;
			isClick = true;
			CatchObject();
		}
		else if (Input.GetMouseButtonUp(1))
		{
			playerStatebase.IsGrapping = false;
			isClick = false;
		}


		ThrowRangeView();
	}


	private void CatchObject()
	{
		// ���콺 ��ġ���� ȭ�� ���� 3D �������� ���̸� ��
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

		// ���̰� � ������Ʈ�� �浹�ߴ��� Ȯ��
		if (hit.collider != null)
		{
			// �浹�� ������Ʈ�� �������ų� �ش� ������Ʈ�� ��ũ��Ʈ�� ����
			GameObject clickedObject = hit.collider.gameObject;


			// �Ÿ� �̻��� �� return;
			if (Vector2.Distance(player.transform.position, clickedObject.transform.position) >= throwRange)
			{
				Debug.Log("�ָ� ����");
				return;
			}



			// ���̿� ������Ʈ ���� �� return;




			// �����Ⱑ ������ ������Ʈ�� ���
			if (clickedObject.tag == "Object")
			{
				throwObject = clickedObject;
				throwObjectDefaultPos = throwObject.transform.position;

				// ���⿡ Ŭ���� ������Ʈ�� ���� �߰� �۾� ����
				Debug.Log("Clicked object: " + clickedObject.name);
				StartCoroutine("HookLineDraw");
				StartCoroutine("ThrowArrowCoroutine");
			}

		}

	}


	private IEnumerator HookLineDraw()
	{
		line.enabled = true;
		line.positionCount = 2;

		float t = 0;
		float time = 10;

		line.SetPosition(0, transform.position);
		line.SetPosition(1, transform.position);

		Vector2 newPos;

		for (; t < time; t += 100f * Time.deltaTime)
		{
			newPos = Vector2.Lerp(transform.position, throwObjectDefaultPos, t / time);
			line.SetPosition(0, transform.position);
			line.SetPosition(1, newPos);

			if (playerStatebase.IsHitting) //�׷��߿� ������ 
			{
				playerController.Stun(); // ���� ����
			}
			yield return null;
		}


		while(true)
		{
			throwObject.transform.position = throwObjectDefaultPos;

			line.SetPosition(0, transform.position);
			line.SetPosition(1, throwObject.transform.position);

			if (!isClick)
			{
				break;
			}
			yield return null;
		}

		
	}


	private IEnumerator HookLineBack()
	{


		float t = Vector2.Distance(transform.position, throwObject.transform.position);
		float time = 10;

		line.SetPosition(0, transform.position);
		line.SetPosition(1, transform.position);

		Vector2 newPos;

		for (; t < time; t += 100f * Time.deltaTime)
		{
			newPos = Vector2.Lerp(throwObjectDefaultPos, transform.position, t / time);
			line.SetPosition(0, transform.position);
			line.SetPosition(1, newPos);

			if (playerStatebase.IsHitting) //�׷��߿� ������ 
			{
				playerController.Stun(); // ���� ����
			}
			yield return null;
		}

		line.SetPosition(0, transform.position);
		line.SetPosition(1, transform.position);

		line.enabled = false;
		line.positionCount = 2;
	}



	private IEnumerator ThrowArrowCoroutine()
	{
		

		while (true)
		{
			Vector3 objectPos = throwObject.transform.position;

			// ���콺�� ���� ��ġ
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0; // Z ���� ��ġ�����ݴϴ�.

			// A ������Ʈ���� ���콺�� ���ϴ� ���� ���� ���
			Vector3 directionVector = mousePosition - objectPos;

			// ����ȭ�� ���� ���͸� ��� �ʹٸ� ������ ���
			Vector3 normalizedDirection = directionVector.normalized;



			// ȭ��ǥ �׸���
			ThrowArrowView(normalizedDirection);


			// ��Ŭ���� ���� ���� �� �߻�
			if (!isClick)
			{
				StartCoroutine("HookLineBack");
				ObjectThrowSkll(normalizedDirection);
				break;
			}

			yield return null;
		}

	}

	private void ObjectThrowSkll(Vector2 pos)
	{
		throwObject.GetComponent<Rigidbody2D>().AddForce(pos * throwPower);

		throwArrow.SetActive(false);
	}




	private void ThrowArrowView(Vector3 target)
	{
		throwArrow.transform.position = throwObject.transform.position;

		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
		Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
		Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 10000);
		throwArrow.transform.rotation = rotation;

		throwArrow.SetActive(true);

	}


	private void ThrowRangeView()
	{
		throwViewGameobject.transform.localScale = new Vector3(throwRange * 2, throwRange * 2, 1);
	}
}
