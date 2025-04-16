using System.Collections.Generic;
using UnityEngine;



public enum HookType
{ 
	// 대기, 차징중, 갈고리가 나가는 상태, 잡은 상태, 플레이어 움직임, 후크 회수
	None = 0,
	Charging,
	Shooting,
	Grappling,
	PlayerMove,
	Retrieve,
}




public class HookController : MonoBehaviour
{
	[Header("플레이어")]
	[SerializeField] private PlayerStateBase _playerState;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private Rigidbody2D playerRigidbody2D;					


	[Header("갈고리")]
	[SerializeField][Tooltip("갈고리의 오브젝트")] private Hook hookobject;					// 갈고리
	[SerializeField][Tooltip("갈고리의 끝부분")] private GameObject hookAnchor;              // 갈고리 끝
	[SerializeField][Tooltip("그랩할 수 있는 레이어 마스크")] private LayerMask grappleAbleLayMask;
	[SerializeField][Tooltip("플레이어가 통과할 수 없는 레이어 마스크")] private LayerMask wallLayersMask;
	[SerializeField][Tooltip("무시할 마스크")] private LayerMask ignoreMask;
	[SerializeField] private LineRenderer ropeLineRenderer;                                 // 갈고리의 로프를 그릴 라인렌더러
	[SerializeField] private DistanceJoint2D distanceJoint2D;                               // 위치 지정 조인트
	[SerializeField] private GameObject chargingEffect;
	[SerializeField] private GameObject chargingEndEffect;
	[SerializeField] private Vector2 limiteScreenVector2;									// 스크린 화면 확장할 범위

	[Space(10)]
	[Header("위치 표시")]
	[SerializeField] private GameObject playerLaserObject;									
	[SerializeField] private LineRenderer targetLineRendere;                                // 마우스를 누를때 표시할 라인렌더러
	[SerializeField] private GameObject targetAnchor;										// 레이저가 나올 때 마지막 위치

	[Space(10)]
	[Header("수치")]
	[SerializeField] private float hookSpeed;
	[SerializeField] private float hookPlayerSpeed;
	[SerializeField][Tooltip("차징 시간")] private float hookChargingTime;					// 갈고리 공격 차징까지 걸리는 시간
	[SerializeField][Tooltip("거리 제한")] private float limitDistance;						// 갈고리 거리 제한




	// 인스펙터에 없는 값들
	//private List<GameObject> hookBody;								// 갈고리 몸 : 만약에 사슬같은 걸로 할 거면


	// 로프
	private HookType currentRopeState;                              // 현재 로프 상태
	private HookType RetrieveRopeState;								// 이전 로프 상태
	private int ropeAnchor = 0;										// 로프가 장애물에 닿여 휘어져야할 때, 꺾어야할 개수
	private Queue<Vector3> ropeAnchorVector = new Queue<Vector3>();	// 로프가 꺾인 위치
	private float currentChargingTime;                              // 플레이어가 갈고리를 발사할 때 차징하는 시간
	private Vector3 myMousePosition;                                // 마우스가 현재 향한 위치(레이 캐스트 때문)
	private Vector2 direction;
	private Vector2 hookTargetPos;


	private bool isAbleAttack;                                      // 공격 가능 판단;
	private Vector2 targetPos;                                      // 플레이어가 로프에 따라 움직이는 위치


	private Vector2 playerMoveVector2;                              // 현재 플레이어가 이동하는 방향

	private bool isChargingSoundOut;								// 현재 차징 사운드가 출력중인지 판단

	public Vector2 PlayerMoveVector2
	{
		get => playerMoveVector2;
	}
	public HookType CurrentRopeState
	{
		get => currentRopeState;
	}
	private void Awake()
	{

		// 갈고리 오브젝트 찾기
		hookobject = FindObjectOfType<Hook>();
		// 갈고리 로프 LineRenderer 받아오기
		ropeLineRenderer = hookobject.transform.GetChild(0).GetComponent<LineRenderer>();
		// 갈고리 앵커 받아오기
		hookAnchor = hookobject.transform.GetChild(1).gameObject;
		// Distance Joint2D 받아오기
		distanceJoint2D = hookobject.GetComponent<DistanceJoint2D>();
		// Distance Joint2D의 Player의 Rigid Body 연결
		distanceJoint2D.connectedBody = gameObject.GetComponent<Rigidbody2D>();


		// 레이저 오브젝트 찾기
		playerLaserObject = GameObject.FindGameObjectWithTag("PlayerLaser");
		// 레이저 LineRenderer 받아오기
		targetLineRendere = playerLaserObject.transform.GetChild(0).GetComponent<LineRenderer>();
		// 레이저 끝부분 오브젝트 받아오기
		targetAnchor = playerLaserObject.transform.GetChild(1).gameObject;



		// 갈고리 설정
		// 라인렌더러 초기값 설정
		ropeLineRenderer.positionCount = 2;
		ropeLineRenderer.useWorldSpace = true;
		distanceJoint2D.enabled = false;
		hookobject.gameObject.SetActive(false);

		// 레이저 설정
		playerLaserObject.SetActive(false);


		// 수치 설정
		currentRopeState = HookType.None;

	}


	private void Update()
	{
		if(GameManager.Instance.IsGameClear)
		{
			return;
		}

		if(GameManager.Instance.IsGameOver)
		{
			currentRopeState = HookType.Grappling;
		}


		HookControll();

	}


	private void HookControll()
	{
		switch (currentRopeState)
		{
			// 기본 상태
			case HookType.None:
				_playerState.IsGrapping = false;
				isChargingSoundOut = false;
				_playerState.CurrentAnimationType = PlayerAnimationType.None;
				HookIdle();

				CursorManager.Instance.ChangeCuresor(CursorType.PossibleCursor);
				break;
			// 플레이어가 후크를 차징하고 있는 상태
			case HookType.Charging :
				if(!isChargingSoundOut)
				{
					isChargingSoundOut = true;
					_playerState.ChargingAudioSource.Play();
				}
				HookCharging();
				break;
			// 갈고리가 날라가는 상태
			case HookType.Shooting :
				isChargingSoundOut = false;
				_playerState.ChargingAudioSource.Stop();
				RopeLineDrwa();
				HookShooting();
				CursorManager.Instance.ChangeCuresor(CursorType.BackCursor);
				_playerState.IsGrapping = true;

				break;
			// 매달려 있는 상태
			case HookType.Grappling:
				RopeLineDrwa();
				HookGrappling();
				_playerState.IsGrapping = true;
				break;
			// 갈고리가 있는 위치로 이동하고 있는 상태
			case HookType.PlayerMove :
				_playerState.CurrentAnimationType = PlayerAnimationType.Move;

				RopeLineDrwa();
				PlayerMove();
				CursorManager.Instance.ChangeCuresor(CursorType.BackCursor);
				

				break;
			// 갈고리를 회수하는 상태
			case HookType.Retrieve:
				_playerState.CurrentAnimationType = PlayerAnimationType.None;
				RopeLineDrwa();
				HookRetrieve();
				//_playerState.IsGrapping = false;
				break;


		}

	}

	private void HookIdle()
	{
		if (Input.GetMouseButtonDown(0))
		{
			currentRopeState = HookType.Charging;
		}

		RetrieveRopeState = currentRopeState;

	}


	private void HookCharging()
	{
		if(RetrieveRopeState != currentRopeState)
		{
			RetrieveRopeState = currentRopeState;
			_playerState.ChargingAudioSource.Play();
		}

		
		currentChargingTime += Time.deltaTime;



		if (Input.GetMouseButton(0))
		{

			// 마우스 포지션 값 받아오기
			// 마우스 위치
			myMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// 마우스를 누른 위치와 플레이어의 위치를 통해 방향 벡터 구함
			direction = myMousePosition - transform.position;


			// 갈고리를 발사할 지점을 표시할 LineRenderer

			// 갈고리가 발사될 궤적
			// 갈고리가 상호작용할 수 있는 마스크만 인식

			// 제한 거리 존재
			//RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, limitDistance, grappleAbleLayMask);

			// 제한 거리 존재 X
			RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleAbleLayMask);


			// 만약 마우스가 아무것도 클릭하고 있지 않다면
			// 몬스터나 그랩이 가능한 위치를 클릭하여야함
			if (hookHitObject.collider == null)
			{

				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
				return;
			}


			// 레이 캐스트가 맞은 지점 구하기
			hookTargetPos = hookHitObject.point;

			// 갈고리 거는 것이 가능한 경우 판단
			// 가능
			if (HookPossibleCheck(hookTargetPos))
			{
				if (!playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(true);
				}
				TargetLineDrwa(hookTargetPos);
			}
			else
			{
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
			}



			if(currentChargingTime >= hookChargingTime)
			{
				chargingEffect.SetActive(false);
				chargingEndEffect.SetActive(true);
			}
			else
			{
				chargingEffect.SetActive(true);
				chargingEndEffect.SetActive(false);
			}

		}
		else if(Input.GetMouseButtonUp(0))
		{
			_playerState.ChargingAudioSource.Stop();

			chargingEffect.SetActive(false);
			chargingEndEffect.SetActive(false);

			currentChargingTime += Time.deltaTime;


			// 현재 차징한 시간이 공격 차징시간 보다 크다면
			// 공격 가능 상태를 나타내는 isAbleAttack bool 변수 true,
			// 작다면 false로 변경
			if (currentChargingTime >= hookChargingTime)
			{
				isAbleAttack = true;
			}
			else
			{
				isAbleAttack = false;
			}


			// 마우스 포지션 값 받아오기
			// 마우스 위치
			myMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// 마우스를 누른 위치와 플레이어의 위치를 통해 방향 벡터 구함
			direction = myMousePosition - transform.position;


			// 시간 초기화
			//Debug.Log(currentChargingTime);
			TimeReset();


			// 갈고리가 박힐 위치
			// 갈고리가 상호작용할 수 있는 마스크만 인식
			
			// 제한 거리 존재
			//RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, limitDistance, grappleAbleLayMask);

			// 제한 거리 존재 X
			RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleAbleLayMask);



			// 만약 마우스가 아무것도 클릭하고 있지 않다면
			// 몬스터나 그랩이 가능한 위치를 클릭하여야함
			if (hookHitObject.collider == null)
			{
				currentRopeState = HookType.None;
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
				return;
			}


			// 레이 캐스트가 맞은 지점 구하기
			hookTargetPos = hookHitObject.point;

			// 갈고리 거는 것이 가능한 경우 판단
			// 가능
			if (HookPossibleCheck(hookTargetPos))
			{
				TargetLineDrwa(hookTargetPos);

				// 갈고리 오브젝트 초기 위치를 플레이어 위치로 변경
				hookobject.transform.position = transform.position;

				// 갈고리 회전
				HookArrowRotation(direction.normalized);

				//라인렌더러와 갈고리 활성화
				ropeLineRenderer.SetPosition(0, transform.position);
				ropeLineRenderer.SetPosition(1, hookobject.transform.position);
				hookobject.gameObject.SetActive(true);
				ropeLineRenderer.enabled = true;


				// 궤적 비활성화
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}

				// 현재 상태 갈고리 변경
				currentRopeState = HookType.Shooting;

				if (isAbleAttack)
				{
					playerController._PlayerStatebase.AttackAudioSource.Play();
				}
				else
				{
					playerController._PlayerStatebase.GrabAudioSource.Play();
				}
			}
			else // 불가능
			{
				currentRopeState = HookType.None;
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
				return;
			}



			/*// 갈고리 오브젝트 초기 위치를 플레이어 위치로 변경
			hookobject.transform.position = transform.position;
			
			// 갈고리 회전
			HookArrowRotation(direction.normalized);

			//라인렌더러와 갈고리 활성화
			ropeLineRenderer.SetPosition(0, transform.position);
			ropeLineRenderer.SetPosition(1, hookobject.transform.position);
			hookobject.gameObject.SetActive(true);
			ropeLineRenderer.enabled = true;


			// 궤적 비활성화
			if (playerLaserObject.activeSelf)
			{
				playerLaserObject.SetActive(false);
			}

			// 현재 상태 갈고리 변경
			currentRopeState = HookType.Shooting;

			if(isAbleAttack)
			{
				playerController._PlayerStatebase.AttackAudioSource.Play();
			}
			else
			{
				playerController._PlayerStatebase.GrabAudioSource.Play();
			}*/

		}
	}


	private void HookShooting()
	{
		if(RetrieveRopeState != currentRopeState)
		{
			RetrieveRopeState = currentRopeState;
			_playerState.GrabAudioSource.Play();
		}


		// 갈고리가 움직이는 중
		//Debug.Log(hookTargetPos);
		hookobject.gameObject.transform.position = Vector2.MoveTowards(hookobject.gameObject.transform.position, hookTargetPos, Time.deltaTime * hookSpeed);



		// 갈고리가 현재 오브젝트에 박힌 상태인가 or 몬스터를 공격한 상태인가
		if (hookobject.IsGrappling)
		{

			// 목표 위치 저장
			ropeAnchorVector.Enqueue(hookobject.transform.position);
			targetPos = hookobject.transform.position;

			// 오브젝트에 박힌 상태라면
			// Grappling 상태로 변경
			// currentRopeState = HookType.Grappling;


			// DistnaceJoint2D 활성화
			distanceJoint2D.enabled = true;

			// PlayerMove 상태로 변경
			currentRopeState = HookType.PlayerMove;


			return;

		}
		else if(hookobject.IsUnGrappling)
		{
			// 그랩 불가능
			currentRopeState = HookType.Retrieve;
		}
		else if(hookobject.IsEnemyAttack)
		{
			// 공격 가능한 상태 확인
			if(isAbleAttack)
			{
				// 공격 가능
				PlayerAttack();
				currentRopeState = HookType.Retrieve;
				return;
			}
			else
			{
				// 공격 불가능
				currentRopeState = HookType.Retrieve;
				return;
			}
		}

		

		// 갈고리가 한계 위치에 도달했는가?
		if(Vector2.Distance(transform.position, hookobject.transform.position) >= limitDistance)
		{
			currentRopeState = HookType.Retrieve;
			return;
		}

	}


	private void HookGrappling()
	{
		// 마우스 우클릭시 갈고리가 있는 플레이어가 움직이는 상태로 변경
		/*if(Input.GetMouseButtonUp(1))
		{
			// 갈고리를 통해 플레이어를 움직이기 위해 distanceJoint활성화
			distanceJoint2D.enabled = true;


			targetPos = ropeAnchorVector.Dequeue();
			currentRopeState = HookType.PlayerMove;
			
			return;
			//playerRigidbody2D.AddForce(targetPos.normalized * hookPlayerSpeed);

		}


		// 갈고리가 플레이어보다 밑에 있으면 중력의 영향을 우선을 하기 위해
		// distanceJoint를 비활성화한다.
		if(hookAnchor.transform.position.y < transform.position.y)
		{
			distanceJoint2D.enabled = false;
		}
		else
		{
			distanceJoint2D.enabled=true;
		}*/
	}

	private void PlayerMove()
	{
		RaycastHit2D objectRay;

		_playerState.CurrentAnimationType = PlayerAnimationType.Move;

		if(_playerState.IsBossSuction)
		{
			return;
		}

		// 플레이어가 향하는 방향에 벽이 있다면 움직이는 것을 멈춘다
		Vector2 targetVector = (targetPos - (Vector2)transform.position).normalized;
		objectRay = Physics2D.Raycast(transform.position, targetVector, 0.4f, wallLayersMask);

		playerMoveVector2 = targetVector;

		// 만약 플레이어가 현재 로프로 인해 위로 올라가고 있는 상태라면 Ray를 Vector2.up 방향으로 쏨
		/*if (targetPos.y - transform.position.y > 0)
		{
			objectRay = Physics2D.Raycast(transform.position, Vector2.up, 0.2f, wallLayersMask);

		}
		else
		{
			objectRay = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, wallLayersMask);
		}*/



		// 플레이어와 로프 사이가 막혀있지 않다면
		// 로프를 통한 이동을 함
		if (objectRay.collider == null)
		{
			//Debug.Log("아무것도 없음");
			distanceJoint2D.distance -= Time.deltaTime * hookPlayerSpeed;
		}



		/*Vector2 targetVector = (targetPos - (Vector2)transform.position).normalized;
		playerController.PlayerMoveVelocity = targetVector * hookPlayerSpeed;*/

		//transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * hookPlayerSpeed);





		// 플레이어가 위치에 도착했다면
		if (Vector2.Distance(transform.position, targetPos) < 1f)
		{


			// DistnaceJoint2D 비활성화
			distanceJoint2D.enabled = false;

			playerController.PlayerMoveVelocity = Vector2.zero;
			currentRopeState = HookType.None;
			hookobject.gameObject.SetActive(false);


			// 다음 목표 위치가 있는지 판단하고
			/*if(ropeAnchorVector.Count == 0)
			{

				// DistnaceJoint2D 비활성화
				distanceJoint2D.enabled = false;

				playerController.PlayerMoveVelocity = Vector2.zero;
				currentRopeState = HookType.None;
				hookobject.gameObject.SetActive(false);
			}
			else
			{
				targetPos = ropeAnchorVector.Dequeue();
			}*/
		}


		// 중간 회수
		if(Input.GetMouseButtonDown(1))
		{
			playerController.PlayerMoveVelocity = Vector2.zero;
			currentRopeState = HookType.Retrieve;
		}
	}


	private void HookRetrieve()
	{
		hookobject.transform.position = Vector2.MoveTowards(hookobject.transform.position, transform.position, hookSpeed * Time.deltaTime);


		if(Vector2.Distance(hookobject.transform.position, transform.position) <= 0.5f)
		{
			distanceJoint2D.enabled = true;
			hookobject.gameObject.SetActive(false);
			currentRopeState = HookType.None;
		}

	}

	// 마우스가 향하는 위치 보여주기
	private void TargetLineDrwa(Vector2 hookTargetPos)
	{
		targetLineRendere.SetPosition(0, transform.position);
		targetLineRendere.SetPosition(1, hookTargetPos);
		targetAnchor.transform.position = hookTargetPos;
	}


	// 갈고리의 로프 그리기
	private void RopeLineDrwa()
	{
		
		ropeLineRenderer.SetPosition(0, transform.position);
		ropeLineRenderer.SetPosition(1, hookAnchor.transform.position);
	}

	// 차징 타임 리셋
	private void TimeReset()
	{
		currentChargingTime = 0;
	}

	// 플레이어의 공격
	public void PlayerAttack()
	{
		hookobject.AttackEnemy.HitEnemy(1);
	}

	// 레이저 리셋
	private void TargetLaserReset()
	{
		targetLineRendere.SetPosition(0, transform.position);
		targetLineRendere.SetPosition(1, transform.position);
		targetAnchor.transform.position = transform.position;

	}

	// 갈고리 방향 조정
	private void HookArrowRotation(Vector3 target)
	{

		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
		Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
		Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 10000);
		hookobject.transform.rotation = rotation;

	}



	// 차징중에 마우스 포인터가 가르키는 방향에 갈고리를 거는 것이 가능한지 판단
	private bool HookPossibleCheck(Vector2 point)
	{


		// 현재 갈고리가 박힌 위치를 스크린 좌표로 변경
		Vector2 hookPointScreenVector2 = Camera.main.WorldToScreenPoint(point);


		// 현재 좌표가 스크린(추가로 범위 확장한 곳)에서 벗어났는지 판단
		if (hookPointScreenVector2.x < 0 - limiteScreenVector2.x
			|| hookPointScreenVector2.x > Screen.width + limiteScreenVector2.x
			|| hookPointScreenVector2.y < 0 - limiteScreenVector2.y
			|| hookPointScreenVector2.y > Screen.height + limiteScreenVector2.y)
		{
			// 스크린 화면 바깥으로 나감
			
			// 현재 커서가 ImposibleCursor이 아니라면 커서 변경
			if(CursorManager.Instance.CurrentCursor != CursorType.ImpossibleCursor)
			{
				CursorManager.Instance.ChangeCuresor(CursorType.ImpossibleCursor);
			}
			return false;
		}
		else
		{
			// 스크린 화면 안 쪽에 존재함

			// 현재 커서가 PosibleCursor이 아니라면 커서 변경
			if(currentChargingTime >= hookChargingTime)
			{
				if (CursorManager.Instance.CurrentCursor != CursorType.ChargingEndCursor)
				{
					CursorManager.Instance.ChangeCuresor(CursorType.ChargingEndCursor);
				}
			}
			else
			{
				if (CursorManager.Instance.CurrentCursor != CursorType.ChargingCursor)
				{
					CursorManager.Instance.ChangeCuresor(CursorType.ChargingCursor);
				}
			}
			return true;
		}
	}
}
