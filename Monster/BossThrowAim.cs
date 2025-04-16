using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossThrowAim : AimDisplay
{
	[SerializeField] private Transform startPos;
	[SerializeField] private LineRenderer[] throwLineRenderes;      // 보스가 던지는 돌멩이의 궤도를 보여줄 LineRendere


	public override void ThrowAimDispaly(int LineNumber, Vector2 ThrowAimVector2)
	{
		throwLineRenderes[LineNumber].gameObject.SetActive(true);
		StartCoroutine(LineDraw(throwLineRenderes[LineNumber], ThrowAimVector2));
	}

	public override void ThrowAimUnDispaly()
	{
		for (int i = 0; i < throwLineRenderes.Length; i++)
		{
			throwLineRenderes[i].SetPosition(0, Vector3.zero);
			throwLineRenderes[i].SetPosition(1, Vector3.zero);

			throwLineRenderes[i].gameObject.SetActive(false);
		}
	}

	private IEnumerator LineDraw(LineRenderer line, Vector2 aim)
	{

		Vector2 startVector2 = startPos.position;
		line.SetPosition(0, startPos.position);

		Vector2 dir = startVector2 + new Vector2(aim.x, aim.y).normalized * 100f;
		line.SetPosition(1, dir);
		yield return null;
	}
}
