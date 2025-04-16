using System.Collections.Generic;
using UnityEngine;



public enum HookType
{ 
	// ���, ��¡��, ������ ������ ����, ���� ����, �÷��̾� ������, ��ũ ȸ��
	None = 0,
	Charging,
	Shooting,
	Grappling,
	PlayerMove,
	Retrieve,
}




public class HookController : MonoBehaviour
{
	[Header("�÷��̾�")]
	[SerializeField] private PlayerStateBase _playerState;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private Rigidbody2D playerRigidbody2D;					


	[Header("����")]
	[SerializeField][Tooltip("������ ������Ʈ")] private Hook hookobject;					// ����
	[SerializeField][Tooltip("������ ���κ�")] private GameObject hookAnchor;              // ���� ��
	[SerializeField][Tooltip("�׷��� �� �ִ� ���̾� ����ũ")] private LayerMask grappleAbleLayMask;
	[SerializeField][Tooltip("�÷��̾ ����� �� ���� ���̾� ����ũ")] private LayerMask wallLayersMask;
	[SerializeField][Tooltip("������ ����ũ")] private LayerMask ignoreMask;
	[SerializeField] private LineRenderer ropeLineRenderer;                                 // ������ ������ �׸� ���η�����
	[SerializeField] private DistanceJoint2D distanceJoint2D;                               // ��ġ ���� ����Ʈ
	[SerializeField] private GameObject chargingEffect;
	[SerializeField] private GameObject chargingEndEffect;
	[SerializeField] private Vector2 limiteScreenVector2;									// ��ũ�� ȭ�� Ȯ���� ����

	[Space(10)]
	[Header("��ġ ǥ��")]
	[SerializeField] private GameObject playerLaserObject;									
	[SerializeField] private LineRenderer targetLineRendere;                                // ���콺�� ������ ǥ���� ���η�����
	[SerializeField] private GameObject targetAnchor;										// �������� ���� �� ������ ��ġ

	[Space(10)]
	[Header("��ġ")]
	[SerializeField] private float hookSpeed;
	[SerializeField] private float hookPlayerSpeed;
	[SerializeField][Tooltip("��¡ �ð�")] private float hookChargingTime;					// ���� ���� ��¡���� �ɸ��� �ð�
	[SerializeField][Tooltip("�Ÿ� ����")] private float limitDistance;						// ���� �Ÿ� ����




	// �ν����Ϳ� ���� ����
	//private List<GameObject> hookBody;								// ���� �� : ���࿡ �罽���� �ɷ� �� �Ÿ�


	// ����
	private HookType currentRopeState;                              // ���� ���� ����
	private HookType RetrieveRopeState;								// ���� ���� ����
	private int ropeAnchor = 0;										// ������ ��ֹ��� �꿩 �־������� ��, ������� ����
	private Queue<Vector3> ropeAnchorVector = new Queue<Vector3>();	// ������ ���� ��ġ
	private float currentChargingTime;                              // �÷��̾ ������ �߻��� �� ��¡�ϴ� �ð�
	private Vector3 myMousePosition;                                // ���콺�� ���� ���� ��ġ(���� ĳ��Ʈ ����)
	private Vector2 direction;
	private Vector2 hookTargetPos;


	private bool isAbleAttack;                                      // ���� ���� �Ǵ�;
	private Vector2 targetPos;                                      // �÷��̾ ������ ���� �����̴� ��ġ


	private Vector2 playerMoveVector2;                              // ���� �÷��̾ �̵��ϴ� ����

	private bool isChargingSoundOut;								// ���� ��¡ ���尡 ��������� �Ǵ�

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

		// ���� ������Ʈ ã��
		hookobject = FindObjectOfType<Hook>();
		// ���� ���� LineRenderer �޾ƿ���
		ropeLineRenderer = hookobject.transform.GetChild(0).GetComponent<LineRenderer>();
		// ���� ��Ŀ �޾ƿ���
		hookAnchor = hookobject.transform.GetChild(1).gameObject;
		// Distance Joint2D �޾ƿ���
		distanceJoint2D = hookobject.GetComponent<DistanceJoint2D>();
		// Distance Joint2D�� Player�� Rigid Body ����
		distanceJoint2D.connectedBody = gameObject.GetComponent<Rigidbody2D>();


		// ������ ������Ʈ ã��
		playerLaserObject = GameObject.FindGameObjectWithTag("PlayerLaser");
		// ������ LineRenderer �޾ƿ���
		targetLineRendere = playerLaserObject.transform.GetChild(0).GetComponent<LineRenderer>();
		// ������ ���κ� ������Ʈ �޾ƿ���
		targetAnchor = playerLaserObject.transform.GetChild(1).gameObject;



		// ���� ����
		// ���η����� �ʱⰪ ����
		ropeLineRenderer.positionCount = 2;
		ropeLineRenderer.useWorldSpace = true;
		distanceJoint2D.enabled = false;
		hookobject.gameObject.SetActive(false);

		// ������ ����
		playerLaserObject.SetActive(false);


		// ��ġ ����
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
			// �⺻ ����
			case HookType.None:
				_playerState.IsGrapping = false;
				isChargingSoundOut = false;
				_playerState.CurrentAnimationType = PlayerAnimationType.None;
				HookIdle();

				CursorManager.Instance.ChangeCuresor(CursorType.PossibleCursor);
				break;
			// �÷��̾ ��ũ�� ��¡�ϰ� �ִ� ����
			case HookType.Charging :
				if(!isChargingSoundOut)
				{
					isChargingSoundOut = true;
					_playerState.ChargingAudioSource.Play();
				}
				HookCharging();
				break;
			// ������ ���󰡴� ����
			case HookType.Shooting :
				isChargingSoundOut = false;
				_playerState.ChargingAudioSource.Stop();
				RopeLineDrwa();
				HookShooting();
				CursorManager.Instance.ChangeCuresor(CursorType.BackCursor);
				_playerState.IsGrapping = true;

				break;
			// �Ŵ޷� �ִ� ����
			case HookType.Grappling:
				RopeLineDrwa();
				HookGrappling();
				_playerState.IsGrapping = true;
				break;
			// ������ �ִ� ��ġ�� �̵��ϰ� �ִ� ����
			case HookType.PlayerMove :
				_playerState.CurrentAnimationType = PlayerAnimationType.Move;

				RopeLineDrwa();
				PlayerMove();
				CursorManager.Instance.ChangeCuresor(CursorType.BackCursor);
				

				break;
			// ������ ȸ���ϴ� ����
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

			// ���콺 ������ �� �޾ƿ���
			// ���콺 ��ġ
			myMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// ���콺�� ���� ��ġ�� �÷��̾��� ��ġ�� ���� ���� ���� ����
			direction = myMousePosition - transform.position;


			// ������ �߻��� ������ ǥ���� LineRenderer

			// ������ �߻�� ����
			// ������ ��ȣ�ۿ��� �� �ִ� ����ũ�� �ν�

			// ���� �Ÿ� ����
			//RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, limitDistance, grappleAbleLayMask);

			// ���� �Ÿ� ���� X
			RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleAbleLayMask);


			// ���� ���콺�� �ƹ��͵� Ŭ���ϰ� ���� �ʴٸ�
			// ���ͳ� �׷��� ������ ��ġ�� Ŭ���Ͽ�����
			if (hookHitObject.collider == null)
			{

				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
				return;
			}


			// ���� ĳ��Ʈ�� ���� ���� ���ϱ�
			hookTargetPos = hookHitObject.point;

			// ���� �Ŵ� ���� ������ ��� �Ǵ�
			// ����
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


			// ���� ��¡�� �ð��� ���� ��¡�ð� ���� ũ�ٸ�
			// ���� ���� ���¸� ��Ÿ���� isAbleAttack bool ���� true,
			// �۴ٸ� false�� ����
			if (currentChargingTime >= hookChargingTime)
			{
				isAbleAttack = true;
			}
			else
			{
				isAbleAttack = false;
			}


			// ���콺 ������ �� �޾ƿ���
			// ���콺 ��ġ
			myMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// ���콺�� ���� ��ġ�� �÷��̾��� ��ġ�� ���� ���� ���� ����
			direction = myMousePosition - transform.position;


			// �ð� �ʱ�ȭ
			//Debug.Log(currentChargingTime);
			TimeReset();


			// ������ ���� ��ġ
			// ������ ��ȣ�ۿ��� �� �ִ� ����ũ�� �ν�
			
			// ���� �Ÿ� ����
			//RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, limitDistance, grappleAbleLayMask);

			// ���� �Ÿ� ���� X
			RaycastHit2D hookHitObject = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleAbleLayMask);



			// ���� ���콺�� �ƹ��͵� Ŭ���ϰ� ���� �ʴٸ�
			// ���ͳ� �׷��� ������ ��ġ�� Ŭ���Ͽ�����
			if (hookHitObject.collider == null)
			{
				currentRopeState = HookType.None;
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
				return;
			}


			// ���� ĳ��Ʈ�� ���� ���� ���ϱ�
			hookTargetPos = hookHitObject.point;

			// ���� �Ŵ� ���� ������ ��� �Ǵ�
			// ����
			if (HookPossibleCheck(hookTargetPos))
			{
				TargetLineDrwa(hookTargetPos);

				// ���� ������Ʈ �ʱ� ��ġ�� �÷��̾� ��ġ�� ����
				hookobject.transform.position = transform.position;

				// ���� ȸ��
				HookArrowRotation(direction.normalized);

				//���η������� ���� Ȱ��ȭ
				ropeLineRenderer.SetPosition(0, transform.position);
				ropeLineRenderer.SetPosition(1, hookobject.transform.position);
				hookobject.gameObject.SetActive(true);
				ropeLineRenderer.enabled = true;


				// ���� ��Ȱ��ȭ
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}

				// ���� ���� ���� ����
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
			else // �Ұ���
			{
				currentRopeState = HookType.None;
				if (playerLaserObject.activeSelf)
				{
					playerLaserObject.SetActive(false);
				}
				return;
			}



			/*// ���� ������Ʈ �ʱ� ��ġ�� �÷��̾� ��ġ�� ����
			hookobject.transform.position = transform.position;
			
			// ���� ȸ��
			HookArrowRotation(direction.normalized);

			//���η������� ���� Ȱ��ȭ
			ropeLineRenderer.SetPosition(0, transform.position);
			ropeLineRenderer.SetPosition(1, hookobject.transform.position);
			hookobject.gameObject.SetActive(true);
			ropeLineRenderer.enabled = true;


			// ���� ��Ȱ��ȭ
			if (playerLaserObject.activeSelf)
			{
				playerLaserObject.SetActive(false);
			}

			// ���� ���� ���� ����
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


		// ������ �����̴� ��
		//Debug.Log(hookTargetPos);
		hookobject.gameObject.transform.position = Vector2.MoveTowards(hookobject.gameObject.transform.position, hookTargetPos, Time.deltaTime * hookSpeed);



		// ������ ���� ������Ʈ�� ���� �����ΰ� or ���͸� ������ �����ΰ�
		if (hookobject.IsGrappling)
		{

			// ��ǥ ��ġ ����
			ropeAnchorVector.Enqueue(hookobject.transform.position);
			targetPos = hookobject.transform.position;

			// ������Ʈ�� ���� ���¶��
			// Grappling ���·� ����
			// currentRopeState = HookType.Grappling;


			// DistnaceJoint2D Ȱ��ȭ
			distanceJoint2D.enabled = true;

			// PlayerMove ���·� ����
			currentRopeState = HookType.PlayerMove;


			return;

		}
		else if(hookobject.IsUnGrappling)
		{
			// �׷� �Ұ���
			currentRopeState = HookType.Retrieve;
		}
		else if(hookobject.IsEnemyAttack)
		{
			// ���� ������ ���� Ȯ��
			if(isAbleAttack)
			{
				// ���� ����
				PlayerAttack();
				currentRopeState = HookType.Retrieve;
				return;
			}
			else
			{
				// ���� �Ұ���
				currentRopeState = HookType.Retrieve;
				return;
			}
		}

		

		// ������ �Ѱ� ��ġ�� �����ߴ°�?
		if(Vector2.Distance(transform.position, hookobject.transform.position) >= limitDistance)
		{
			currentRopeState = HookType.Retrieve;
			return;
		}

	}


	private void HookGrappling()
	{
		// ���콺 ��Ŭ���� ������ �ִ� �÷��̾ �����̴� ���·� ����
		/*if(Input.GetMouseButtonUp(1))
		{
			// ������ ���� �÷��̾ �����̱� ���� distanceJointȰ��ȭ
			distanceJoint2D.enabled = true;


			targetPos = ropeAnchorVector.Dequeue();
			currentRopeState = HookType.PlayerMove;
			
			return;
			//playerRigidbody2D.AddForce(targetPos.normalized * hookPlayerSpeed);

		}


		// ������ �÷��̾�� �ؿ� ������ �߷��� ������ �켱�� �ϱ� ����
		// distanceJoint�� ��Ȱ��ȭ�Ѵ�.
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

		// �÷��̾ ���ϴ� ���⿡ ���� �ִٸ� �����̴� ���� �����
		Vector2 targetVector = (targetPos - (Vector2)transform.position).normalized;
		objectRay = Physics2D.Raycast(transform.position, targetVector, 0.4f, wallLayersMask);

		playerMoveVector2 = targetVector;

		// ���� �÷��̾ ���� ������ ���� ���� �ö󰡰� �ִ� ���¶�� Ray�� Vector2.up �������� ��
		/*if (targetPos.y - transform.position.y > 0)
		{
			objectRay = Physics2D.Raycast(transform.position, Vector2.up, 0.2f, wallLayersMask);

		}
		else
		{
			objectRay = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, wallLayersMask);
		}*/



		// �÷��̾�� ���� ���̰� �������� �ʴٸ�
		// ������ ���� �̵��� ��
		if (objectRay.collider == null)
		{
			//Debug.Log("�ƹ��͵� ����");
			distanceJoint2D.distance -= Time.deltaTime * hookPlayerSpeed;
		}



		/*Vector2 targetVector = (targetPos - (Vector2)transform.position).normalized;
		playerController.PlayerMoveVelocity = targetVector * hookPlayerSpeed;*/

		//transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * hookPlayerSpeed);





		// �÷��̾ ��ġ�� �����ߴٸ�
		if (Vector2.Distance(transform.position, targetPos) < 1f)
		{


			// DistnaceJoint2D ��Ȱ��ȭ
			distanceJoint2D.enabled = false;

			playerController.PlayerMoveVelocity = Vector2.zero;
			currentRopeState = HookType.None;
			hookobject.gameObject.SetActive(false);


			// ���� ��ǥ ��ġ�� �ִ��� �Ǵ��ϰ�
			/*if(ropeAnchorVector.Count == 0)
			{

				// DistnaceJoint2D ��Ȱ��ȭ
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


		// �߰� ȸ��
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

	// ���콺�� ���ϴ� ��ġ �����ֱ�
	private void TargetLineDrwa(Vector2 hookTargetPos)
	{
		targetLineRendere.SetPosition(0, transform.position);
		targetLineRendere.SetPosition(1, hookTargetPos);
		targetAnchor.transform.position = hookTargetPos;
	}


	// ������ ���� �׸���
	private void RopeLineDrwa()
	{
		
		ropeLineRenderer.SetPosition(0, transform.position);
		ropeLineRenderer.SetPosition(1, hookAnchor.transform.position);
	}

	// ��¡ Ÿ�� ����
	private void TimeReset()
	{
		currentChargingTime = 0;
	}

	// �÷��̾��� ����
	public void PlayerAttack()
	{
		hookobject.AttackEnemy.HitEnemy(1);
	}

	// ������ ����
	private void TargetLaserReset()
	{
		targetLineRendere.SetPosition(0, transform.position);
		targetLineRendere.SetPosition(1, transform.position);
		targetAnchor.transform.position = transform.position;

	}

	// ���� ���� ����
	private void HookArrowRotation(Vector3 target)
	{

		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
		Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
		Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 10000);
		hookobject.transform.rotation = rotation;

	}



	// ��¡�߿� ���콺 �����Ͱ� ����Ű�� ���⿡ ������ �Ŵ� ���� �������� �Ǵ�
	private bool HookPossibleCheck(Vector2 point)
	{


		// ���� ������ ���� ��ġ�� ��ũ�� ��ǥ�� ����
		Vector2 hookPointScreenVector2 = Camera.main.WorldToScreenPoint(point);


		// ���� ��ǥ�� ��ũ��(�߰��� ���� Ȯ���� ��)���� ������� �Ǵ�
		if (hookPointScreenVector2.x < 0 - limiteScreenVector2.x
			|| hookPointScreenVector2.x > Screen.width + limiteScreenVector2.x
			|| hookPointScreenVector2.y < 0 - limiteScreenVector2.y
			|| hookPointScreenVector2.y > Screen.height + limiteScreenVector2.y)
		{
			// ��ũ�� ȭ�� �ٱ����� ����
			
			// ���� Ŀ���� ImposibleCursor�� �ƴ϶�� Ŀ�� ����
			if(CursorManager.Instance.CurrentCursor != CursorType.ImpossibleCursor)
			{
				CursorManager.Instance.ChangeCuresor(CursorType.ImpossibleCursor);
			}
			return false;
		}
		else
		{
			// ��ũ�� ȭ�� �� �ʿ� ������

			// ���� Ŀ���� PosibleCursor�� �ƴ϶�� Ŀ�� ����
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
