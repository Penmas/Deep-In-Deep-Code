using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] private PlayerStateBase _playerState;
	[SerializeField] private Animator _animator;
	[SerializeField] private GameObject playerGameobject;
	[SerializeField] private SpriteRenderer playerSpriteRenderer;
	[SerializeField] private HookController _hookController;

	private PlayerAnimationType previousAnimationType;
	private PlayerAnimationType currentAnimationType;
	
	private void Update()
	{
		
		currentAnimationType = _playerState.CurrentAnimationType;
		ChangeAnimation();
	}


	private void ChangeAnimation()
	{
		switch (currentAnimationType)
		{
			case PlayerAnimationType.None:
				_animator.SetBool("isMove", false);
				playerSpriteRenderer.flipY = false;
				if (_playerState.IsUpSide)
				{
					playerGameobject.transform.eulerAngles = Vector3.zero;
				}
				else
				{
					playerGameobject.transform.eulerAngles = new Vector3(0, 0, 180);
				}
				break;
			case PlayerAnimationType.Move:
				_animator.SetBool("isMove", true);
				RotationCalculate(_hookController.PlayerMoveVector2);
				break;
			case PlayerAnimationType.Death:
				_animator.SetTrigger("isDeath");
				break;
		}

		
	}


	private void RotationCalculate(Vector2 vector)
	{

		float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
		Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
		Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 10000);
		playerGameobject.transform.rotation = rotation;

		//Debug.Log(vector);

		if (vector.x > 0)
		{
			playerGameobject.transform.eulerAngles += new Vector3(0, 0, -50);
			playerSpriteRenderer.flipY = true;
		}
		else
		{
			playerGameobject.transform.eulerAngles += new Vector3(0, 0, -130);
			playerSpriteRenderer.flipY = false;
		}

	}
}
