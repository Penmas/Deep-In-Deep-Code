using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Camera mainCamera;


    [SerializeField] private GameObject player;
    [SerializeField] public float speed;
   
    public GameObject stonesSave;

    [Header("������ ��ų")]
    [SerializeField] private float bitingSpeed;
  //  [SerializeField] private float bitingHeight;
    [SerializeField] private float idleTime;
    [SerializeField] private Vector2 target;
    [SerializeField] private Vector2 startPos;

    [Header("�÷��̾� ���� & ���� ��ȯ")]
    public float gravityIncreaseRate = 2f;
    [SerializeField] private GameObject stone;
    [SerializeField] private Transform[] spots;
    [SerializeField] private Transform mousePos;
    [SerializeField] private float throwForce = 40f;

    [Header("�Ʒ��� ����")]
    [SerializeField] private float downwardForce = 1f;
    [SerializeField] private float endTime = 2f; //���� ������ ���� �ð�
    //-downwardForce �������� - �ٿ��� ���, �� Ŭ���� ������ ���� �߷��� Ŀ��

    public PlayerStateBase playerStateBase;

    private Rigidbody2D _rigidbody2D; //�θ��� ����
  //  private Rigidbody2D rb; //�÷��̾� ����
    private Rigidbody2D stoneTrow;

 

    private Vector3 throwDirection;
    // private bool check = false;
    private Biting biting;
  

    private float defaultSpeed;
    private void Awake()
    {
        playerStateBase = FindObjectOfType<PlayerStateBase>();
        defaultSpeed = speed;
        _rigidbody2D = GetComponent<Rigidbody2D>();
      //  rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        //if (check)
        //{
        //    InvokeRepeating("RandomFunction", 0f, 5f);
        //}

        //     InvokeRepeating("RandomFunction", 0f, 5f);

        int seed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
        mainCamera = Camera.main;
        biting = this.gameObject.GetComponent<Biting>();
    }


    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    // StartAcceleration �Լ� ȣ��
        //    ThrowInDirection();
        //}
      
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //����(����)
           PlayerDownForce();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //�� ������
            ThrowInDirection();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // biting.MoveStart();
            //����
            Biting();
        }

        Vector3 cameraCenter = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, mainCamera.nearClipPlane));

        target = new Vector2(cameraCenter.x, cameraCenter.y);

        Vector3 cameraBottomCenter = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0f, mainCamera.nearClipPlane));

        startPos = new Vector2(cameraBottomCenter.x, cameraBottomCenter.y);
    }

    private void RandomFunction()
    {
        int randomIndex = UnityEngine.Random.Range(0, 2);

        if (randomIndex == 0)
        {
            Biting();
        }
        else
        {
            ThrowInDirection();
        }
    }

    public void ThrowInDirection()
    {

        for (int i = 0; i < spots.Length; i++)
        {

            stonesSave = Instantiate(stone, spots[i].position, Quaternion.identity);


            //stoneTrow = stonesSave.GetComponent<Rigidbody2D>();
            //throwDirection = (spots[i].position - mousePos.position).normalized;
            //stoneTrow.velocity = throwDirection * throwForce;

        }


    }


    public void BossMovement()
    {
        _rigidbody2D.velocity = Vector2.up * speed;
    }

    public void Biting()
    {
        
       // target = transform.position + new Vector3(0, bitingHeight, 0);
        StartCoroutine(BitingCoroutine());

    }



    private IEnumerator BitingCoroutine()
    {
        
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, target, bitingSpeed * Time.deltaTime);

            if (Mathf.Abs(target.y - transform.position.y) <= 0.1f)
            {
                break;

            }

            yield return null;

        }

        StartCoroutine(BitingDownCoroutine());
        //float time = 0;
        //while (true)
        //{
        //    time += Time.deltaTime;
        //    speed = 10;
        //    if (time >= idleTime)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}


        //speed = defaultSpeed;

    }

    private IEnumerator BitingDownCoroutine()
    {

        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, bitingSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - startPos.y) <= 0.1f)
            {
                break;

            }

            yield return null;

        }

    }


    public void PlayerDownForce()
    {
        //player.GetComponent<Rigidbody2D>().AddForce(Vector2.down * downwardForce, ForceMode2D.Impulse);
        /*
          public void SetPlayerGravity(float _gravity)
    {
        PlayerGravity = new Vector2(0f, _gravity);
        Physics2D.gravity = PlayerGravity;
    }
        */
        Vector2 act = playerStateBase.PlayerGravity;
        Debug.Log(act.y); //���� �߷� & ������ ����

        //������ ����
        playerStateBase.SetPlayerGravity(-downwardForce);
       
        Debug.Log(playerStateBase.PlayerGravity.y); //����� �߷�
        StartCoroutine(ReturnPlayerGravity(act));
    }

    private IEnumerator ReturnPlayerGravity(Vector2 act)
    {
      
        yield return new WaitForSeconds(endTime);
        playerStateBase.SetPlayerGravity(act.y);
        Debug.Log(act.y); //���������� ����

    }
}
