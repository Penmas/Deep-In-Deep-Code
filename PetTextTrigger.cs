using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetTextTrigger : MonoBehaviour
{
	[SerializeField] private int petDialogueIndexNumber;
	[SerializeField] private PetDialog _petDialog;

	BoxCollider2D _boxCollider;

	private void Start()
	{
		_boxCollider = GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			_petDialog.DisplayiedPetDialog(petDialogueIndexNumber);
			gameObject.SetActive(false);
		}

	}

}
