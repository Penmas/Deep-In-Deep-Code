using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField] private GameObject player;         // �÷��̾� 
	[SerializeField] private float cameraSpeed;


	[SerializeField] private float yPos;

    // ���� ȿ���� ����
    private float shakeAmount = 0.1f;

    // ���� ȿ���� �ӵ�
    private float shakeSpeed = 5.0f;

    // ���� ȿ���� ���� �ð�
    private float shakeDuration = 2f;


    public float YPos
	{
        set => yPos = value;
        get => yPos;
	}


    void Start()
    {
        // ���� ���� �� �ʱ� ī�޶� ��ġ ����
        //originalPosition = transform.position;
    }

    private void Update()
	{
		Vector3 target = new Vector3(0, player.transform.position.y - (yPos * -1f), -10);
		transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * cameraSpeed);

        // ���� ȿ�� ����
        /*if (shakeDuration > 0)
        {
            // ������ ���� ���� ����
            Vector3 shakeVector = Random.insideUnitSphere * shakeAmount;

            // ī�޶� ��ġ�� ���� ���� ����
            transform.position = transform.position + shakeVector;

            // ���� ȿ���� ������ �ð��� ���� ���ҽ�Ŵ
            //shakeAmount -= shakeAmount * Time.deltaTime * shakeSpeed;

            // ���� ȿ���� ���� �ð� ����
            //shakeDuration -= Time.deltaTime;
        }*/

        // �׽�Ʈ������ �����̽� Ű�� ������ ���� ȿ�� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartShake(2);
        }
    }


    // ���� ȿ�� ���� �Լ�
    public void StartShake(float time)
    {
        shakeAmount = 0.1f; // �ʱ� ���� ȿ�� ����
        shakeSpeed = 5.0f; // ���� ȿ���� �ӵ�
        shakeDuration = time; // ���� ȿ�� ���� �ð�
    }

}
