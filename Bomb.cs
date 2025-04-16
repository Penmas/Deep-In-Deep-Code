using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
	[SerializeField] private Animator animator;

	[SerializeField] private int damage;
	[SerializeField] private AudioSource bombAudioSource;
	private void Awake()
	{
		animator = GetComponent<Animator>();
		animator.enabled = false;
	}

	public void disableObject()
	{
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.tag == "Player")
		{
			animator.enabled = true;
			other.GetComponent<PlayerStateBase>().SetMinusPlayerHP(damage);
		}
	}

	public void AudioStart()
	{
		bombAudioSource.Play();
	}
}
