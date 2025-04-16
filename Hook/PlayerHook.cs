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
            Debug.Log("PlayerStateBaser �ν��Ͻ� ��ã��. ã�ƿͶ�");
        }

    }

    private void Update()
    {

        // ���� �����̸� ����
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

        // �߻�
        if (Input.GetMouseButtonDown(0))
        {
            // ���� �׷� ����� ������ �����ΰ�?
            if (isPosibleGrap)
            {
                // �÷��̾� Ŭ���ν����� ���� �׷� �Ÿ� ���� ���� �Ǵ�
                StartGrapple();

            }

        }
        // �̵�
        else if(Input.GetMouseButtonUp(0))
		{

            // ���� �׷� ��� ���� ���°� �ƴ� �� = �׷��� ��� ���� ������ ��
            if(!isPosibleGrap)
            {

               // Debug.Log("�� ��");
                // ��ũ�� ������ �������� ��
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
        // �߰� ȸ��
        else if(Input.GetMouseButtonDown(1))
		{
            isReturGrap = true;

        }

    }


    // �׷� �Ǵ�
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



        // ���⼭ üũ
        SetPlayerColliderTrigger(false);

    }


    // �߻�
    private IEnumerator Grapple()
    {
       // Debug.Log("��ũ ������ ���� = ���� �׸��� ����");

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

            if (playerStatebase.IsHitting) //�׷��߿� ������ 
            {
                playerController.Stun(); // ���� ����
            }


            // �߰��� ���´ٸ� ȸ�� �ڷ�ƾ ����
            // �̵��� �ϸ� �� �ǰ� ���� �ڷ�ƾ ����
            if(isMissedObject)
			{
                //Debug.Log("��ũ ��ħ");
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



        // ���
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

        // Debug.Log("�÷��̾� �̵�");
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

        // Debug.Log("��ũ ȸ�� ����");

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

            if (playerStatebase.IsHitting) //�׷��߿� ������ 
            {
                playerController.Stun(); // ���� ����
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
