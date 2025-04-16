using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowSkill : MonoBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private PlayerStateBase playerStatebase;
	[SerializeField] private PlayerController playerController;

	[SerializeField] [Tooltip("잡을 수 있는 거리")] private float throwRange;                //던지기 가능 거리
	[SerializeField] [Tooltip("던지는 세기")] private float throwPower;
	[SerializeField] [Tooltip("던지는 범위 뷰")] private GameObject throwViewGameobject;
	[SerializeField] [Tooltip("던지는 방향 화살표")] private GameObject throwArrow;

	private bool isClick;                                   // 현재 클릭 중인지 판단
	private GameObject throwObject;                         // 던지는 오브젝트
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
		// 마우스 위치에서 화면 상의 3D 공간으로 레이를 쏨
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

		// 레이가 어떤 오브젝트와 충돌했는지 확인
		if (hit.collider != null)
		{
			// 충돌한 오브젝트를 가져오거나 해당 오브젝트의 스크립트를 실행
			GameObject clickedObject = hit.collider.gameObject;


			// 거리 이상일 때 return;
			if (Vector2.Distance(player.transform.position, clickedObject.transform.position) >= throwRange)
			{
				Debug.Log("멀리 있음");
				return;
			}



			// 사이에 오브젝트 있을 때 return;




			// 던지기가 가능한 오브젝트인 경우
			if (clickedObject.tag == "Object")
			{
				throwObject = clickedObject;
				throwObjectDefaultPos = throwObject.transform.position;

				// 여기에 클릭된 오브젝트에 대한 추가 작업 수행
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

			if (playerStatebase.IsHitting) //그랩중에 맞으면 
			{
				playerController.Stun(); // 스턴 실행
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

			if (playerStatebase.IsHitting) //그랩중에 맞으면 
			{
				playerController.Stun(); // 스턴 실행
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

			// 마우스의 현재 위치
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0; // Z 축을 일치시켜줍니다.

			// A 오브젝트에서 마우스로 향하는 방향 벡터 계산
			Vector3 directionVector = mousePosition - objectPos;

			// 정규화된 방향 벡터를 얻고 싶다면 다음을 사용
			Vector3 normalizedDirection = directionVector.normalized;



			// 화살표 그리기
			ThrowArrowView(normalizedDirection);


			// 우클릭을 떼면 종료 및 발사
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
