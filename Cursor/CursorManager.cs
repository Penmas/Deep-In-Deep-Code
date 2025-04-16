using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum CursorType
{
	PossibleCursor,
	ImpossibleCursor,
	BackCursor,
	ChargingCursor,
	ChargingEndCursor,
}


public class CursorManager : MonoBehaviour
{
	private static CursorManager instance;
	public static CursorManager Instance
	{
		get { return instance; }
	}



	[Header("Ŀ�� ����")]
	[SerializeField] private GameObject cursorObject;
	[SerializeField] private Canvas overlayCanvas;


	[Header("�ؽ�Ʈ Ŀ��")]
	[SerializeField] private Texture2D nomalCursor;
	[SerializeField] private Texture2D impossibleCursor;
	[SerializeField] private Texture2D backCursor;
	[SerializeField] private Texture2D[] chargingCursorAnimation;

	private bool isCursorOn;				// ���� ���Ӹ�� Ŀ���ΰ�?
	[SerializeField] private bool isGamePlay;
	private Animator _animator;


	private CursorType currentCursor;

	public CursorType CurrentCursor
	{
		set => currentCursor = value;
		get => currentCursor;
	}



	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}


		_animator = cursorObject.GetComponent<Animator>();
	}


	private void Start()
	{

		isCursorOn = false;

		// Ŀ�� �� ���̰� �ϱ�
		Cursor.visible = false;

		// Ŀ�� �⺻�� ����
		currentCursor = CursorType.PossibleCursor;
		//ChangeCuresor(CursorType.PossibleCursor);

	}



	private void Update()
	{

		if(isGamePlay && !GameManager.Instance.IsGameOver && !GameManager.Instance.IsGameClear)
		{
			// ������ �÷��� ���� ���� Ŀ��
			// isCursorOn = true;
			Cursor.visible = false;
			cursorObject.SetActive(true);
		}
		else
		{
			// �ؽ�Ʈ�� ���� ���� �϶��� Ŀ��
			Cursor.visible = true;
			SetCursor();
			cursorObject.SetActive(false);

			return;
		}

		// ���콺 ��ǥ�� ��ũ�� ��ǥ�� ������
		Vector2 mousePosition = Input.mousePosition;

		Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

		cursorObject.transform.position = worldMousePosition;
	}

	public void ChangeCuresor(CursorType texture2dName)
	{
		switch (texture2dName)
		{
			case CursorType.PossibleCursor:
				_animator.SetBool("isPossible", true);
				_animator.SetBool("isImpossible", false);
				_animator.SetBool("isBack", false);
				_animator.SetBool("isPossibleEffect", false);

				break;
			case CursorType.ImpossibleCursor:
				_animator.SetBool("isPossible", false);
				_animator.SetBool("isImpossible", true);
				_animator.SetBool("isBack", false);
				_animator.SetBool("isPossibleEffect", false);

				break;
			case CursorType.BackCursor:
				_animator.SetBool("isPossible", false);
				_animator.SetBool("isImpossible", false);
				_animator.SetBool("isBack", true);
				_animator.SetBool("isPossibleEffect", false);

				break;
			case CursorType.ChargingCursor:

				break;
			case CursorType.ChargingEndCursor:
				_animator.SetBool("isPossible", false);
				_animator.SetBool("isImpossible", false);
				_animator.SetBool("isBack", false);
				_animator.SetBool("isPossibleEffect", true);


				break;

		}
	}
	
	public void ChangeCursorTexture2D(CursorType texture2dName)
	{

	}

	public void SetCursor()
	{
		Vector2 hotSpot = Vector2.zero;
		hotSpot.x = nomalCursor.width / 2;
		hotSpot.y = nomalCursor.height / 2;

		Cursor.SetCursor(nomalCursor, hotSpot, CursorMode.Auto);
	}
}
