using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBullet : MonoBehaviour
{
	[SerializeField] private int crabBulletVersion;


	private Animator _animator;


	private void Start()
	{
		_animator = GetComponent<Animator>();
		_animator.SetInteger("Version", crabBulletVersion);
	}
}
