using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaObjectOut : MonoBehaviour
{
	[SerializeField] private GameObject[] seaObject;



	public void Ative()
	{
		for(int i = 0; i < seaObject.Length; ++i)
		{
			seaObject[i].SetActive(true);
		}
	}
}
