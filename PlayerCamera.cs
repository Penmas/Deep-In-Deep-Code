using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField] private GameObject player;         // 플레이어 
	[SerializeField] private float cameraSpeed;


	[SerializeField] private float yPos;

    // 떨림 효과의 강도
    private float shakeAmount = 0.1f;

    // 떨림 효과의 속도
    private float shakeSpeed = 5.0f;

    // 떨림 효과의 지속 시간
    private float shakeDuration = 2f;


    public float YPos
	{
        set => yPos = value;
        get => yPos;
	}


    void Start()
    {
        // 게임 시작 시 초기 카메라 위치 저장
        //originalPosition = transform.position;
    }

    private void Update()
	{
		Vector3 target = new Vector3(0, player.transform.position.y - (yPos * -1f), -10);
		transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * cameraSpeed);

        // 떨림 효과 적용
        /*if (shakeDuration > 0)
        {
            // 랜덤한 떨림 벡터 생성
            Vector3 shakeVector = Random.insideUnitSphere * shakeAmount;

            // 카메라 위치에 떨림 벡터 적용
            transform.position = transform.position + shakeVector;

            // 떨림 효과의 강도를 시간에 따라 감소시킴
            //shakeAmount -= shakeAmount * Time.deltaTime * shakeSpeed;

            // 떨림 효과의 지속 시간 감소
            //shakeDuration -= Time.deltaTime;
        }*/

        // 테스트용으로 스페이스 키를 누르면 떨림 효과 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartShake(2);
        }
    }


    // 떨림 효과 시작 함수
    public void StartShake(float time)
    {
        shakeAmount = 0.1f; // 초기 떨림 효과 강도
        shakeSpeed = 5.0f; // 떨림 효과의 속도
        shakeDuration = time; // 떨림 효과 지속 시간
    }

}
