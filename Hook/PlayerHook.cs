using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHook : MonoBehaviour
{
    public PlayerStateBase playerStatebase;
    public PlayerController playerController;

    LineRenderer line;

    [SerializeField] LayerMask grapplableMask;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;

    Collider2D playerCollder2D;
    [SerializeField] Collider2D targetCollider2D;

    private bool isPosibleGrap = true;
    private bool isGrapping = false;

    [SerializeField] LayerMask defultMaxk;


    Vector2 target;

    private bool isPlayerGrapping;              //

    private bool isReturGrap;                   //

    private bool isHookOn;

    private bool isMissedObject;


    private float delay = 0;

    private void Start()
    {

        line = GetComponent<LineRenderer>();
        playerStatebase = FindObjectOfType<PlayerStateBase>();
        playerController = FindObjectOfType<PlayerController>();

        playerCollder2D = playerController.GetComponent<Collider2D>();
        if (playerStatebase != null)
        {
            Debug.Log("PlayerStateBaser 인스턴스 못찾음. 찾아와라");
        }

    }

    private void Update()
    {

        // 게임 오버이면 종료
        if(GameManager.Instance.IsGameOver)
		{
            return;
		}

        if(GameManager.Instance.IsGameClear)
        {
            return;
        }

        if(StageManager.Instance.IsLock)
		{
            return;
		}

        if(delay < 1f)
        {
            delay += Time.deltaTime;

            return;
        }

        // 발사
        if (Input.GetMouseButtonDown(0))
        {
            // 현재 그랩 사용이 가능한 상태인가?
            if (isPosibleGrap)
            {
                // 플레이어 클릭인식으로 인한 그랩 거리 가능 여부 판단
                StartGrapple();

            }

        }
        // 이동
        else if(Input.GetMouseButtonUp(0))
		{

            // 현재 그랩 사용 가능 상태가 아닐 때 = 그랩을 사용 중인 상태일 때
            if(!isPosibleGrap)
            {

               // Debug.Log("손 뗌");
                // 후크가 바위에 도착했을 때
                if (isHookOn)
                {
                    isMissedObject = false;
                    isPlayerGrapping = true;
                }
                else
                {
                    isMissedObject = true;

                }

            }

        }
        // 중간 회수
        else if(Input.GetMouseButtonDown(1))
		{
            isReturGrap = true;

        }

    }


    // 그랩 판단
    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, direction, maxDistance, defultMaxk);



        if(hit.collider == null)
		{
            return;
		}


        if(hit2.collider != null)
		{
            Debug.Log(hit2.collider.name);
            if(hit2.collider.tag == "SeaObject")
			{
                return;
			}
		}



        if (hit.collider != null)
        {
            isPosibleGrap = false;
            targetCollider2D = hit.collider;
            target = hit.point;

            

            StartCoroutine("Grapple");
        }



        // 여기서 체크
        SetPlayerColliderTrigger(false);

    }


    // 발사
    private IEnumerator Grapple()
    {
       // Debug.Log("후크 던지기 가능 = 라인 그리기 시작");

        float t = 0;
        float time = 10;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        line.enabled = true;
        line.positionCount = 2;
        Vector2 newPos;

        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(transform.position, target, t / time);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            if (playerStatebase.IsHitting) //그랩중에 맞으면 
            {
                playerController.Stun(); // 스턴 실행
            }


            // 중간에 놓는다면 회수 코루틴 시행
            // 이동을 하면 안 되게 현재 코루틴 중지
            if(isMissedObject)
			{
                //Debug.Log("후크 놓침");
                line.SetPosition(0, transform.position);
                line.SetPosition(1, newPos);
                StartCoroutine(HookLineBack(newPos));

                yield break;
            }

            SetPlayerColliderTrigger(true);
            yield return null;
        }




        playerStatebase.IsGrapping = true;

        isHookOn = true;
        line.SetPosition(1, target);



        // 대기
        while (true)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, target);

            if (isPlayerGrapping)
            {
                break;
            }
            yield return null;
        }

        StartCoroutine(PlayerMoveCoroutine());
        
    }


    private IEnumerator PlayerMoveCoroutine()
	{

        // Debug.Log("플레이어 이동");
        //playerStatebase.PlayerAudioSource.clip = playerStatebase.GrabVFX;
        //playerStatebase.PlayerAudioSource.Play();
        while(true)
		{
            Vector2 newPos = Vector2.Lerp(transform.position, target, grappleSpeed * Time.deltaTime);
            transform.position = newPos;
            line.SetPosition(0, transform.position);

            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                isMissedObject = false;
                line.enabled = false;
                isPlayerGrapping = false;
                isPosibleGrap = true;
                isHookOn = false;
                break;
            }


            if(isReturGrap)
			{
                playerStatebase.IsGrapping = false;
                StartCoroutine(HookLineBack(newPos));
                break;
			}

            yield return null;
        }

        playerStatebase.IsGrapping = false;

    }


    private IEnumerator HookLineBack(Vector2 lineEndPos)
    {

        // Debug.Log("후크 회수 시작");

        float t = Vector2.Distance(transform.position, lineEndPos);
        float time = 10;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 newPos;

        for (; t < time; t += 100f * Time.deltaTime)
        {
            newPos = Vector2.Lerp(lineEndPos, transform.position, t / time);
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
        
        line.positionCount = 2;
        line.enabled = false;

        isMissedObject = false;
        isPlayerGrapping = false;
        isPosibleGrap = true;
        isReturGrap = false;
        isHookOn = false;
    }


    private void SetPlayerColliderTrigger(bool _isTrigger)
    {
       /* if (playerCollder2D != null)
        {
            playerCollder2D.isTrigger = _isTrigger;
        }*/
    }
}
