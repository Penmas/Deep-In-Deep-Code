using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] private PlayerStateBase playerStateBase;


	[Space(10)]
	[Header("HP¹Ù")]
	[SerializeField] private Image hpBar;



	private void Update()
	{
		HPUIView();
	}



	private void HPUIView()
	{
		float currentHP = playerStateBase.PlayerCurrentHp;
		float maxHP = playerStateBase.PlayerMaxHp;
		hpBar.fillAmount = currentHP / maxHP;
	}

}
