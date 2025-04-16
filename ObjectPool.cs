using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	private static ObjectPool instance;
	public static ObjectPool Instance
	{  get { return instance; } }


	[SerializeField] private GameObject parentObject;


	[Space(10)]
	[SerializeField] private GameObject CrabBulletVer1;
	[SerializeField] private GameObject CrabBulletVer2;
	[SerializeField] private GameObject Spear;



	private Queue<Bullet> carbBulletsVer1 = new Queue<Bullet>();
	private Queue<Bullet> carbBulletsVer2 = new Queue<Bullet>();
	private Queue<Bullet> spearBullets = new Queue<Bullet>();

	
	public GameObject ParentObject
	{
		get => parentObject;
	}



	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	private Bullet CreateCrabBulletVer1()
	{
		var newObj = Instantiate(CrabBulletVer1).GetComponent<Bullet>();
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}

	private Bullet CreateCrabBulletVer2()
	{
		var newObj = Instantiate(CrabBulletVer2).GetComponent<Bullet>();
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}
	private Bullet CreateSpear()
	{
		var newObj = Instantiate(Spear).GetComponent<Bullet>();
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}

	public static Bullet GetObject(string type)
	{
		switch (type)
		{
			case "CrabBulletVer1":

				if (Instance.carbBulletsVer1.Count > 0)
				{
					var obj = Instance.carbBulletsVer1.Dequeue();
					obj.transform.SetParent(null);
					obj.gameObject.SetActive(true);
					return obj;
				}
				else
				{
					var newObj = Instance.CreateCrabBulletVer1();
					newObj.gameObject.SetActive(true);
					newObj.transform.SetParent(null);
					return newObj;
				}

			case "CrabBulletVer2":
				if (Instance.carbBulletsVer2.Count > 0)
				{
					var obj = Instance.carbBulletsVer2.Dequeue();
					obj.transform.SetParent(null);
					obj.gameObject.SetActive(true);
					return obj;
				}
				else
				{
					var newObj = Instance.CreateCrabBulletVer2();
					newObj.gameObject.SetActive(true);
					newObj.transform.SetParent(null);
					return newObj;
				}

			case "Spear":
				if (Instance.spearBullets.Count > 0)
				{
					var obj = Instance.spearBullets.Dequeue();
					obj.transform.SetParent(null);
					obj.gameObject.SetActive(true);
					return obj;
				}
				else
				{
					var newObj = Instance.CreateSpear();
					newObj.gameObject.SetActive(true);
					newObj.transform.SetParent(null);
					return newObj;
				}

		}


		return null;
	}

	public static void ReturnObject(Bullet obj, string type)
	{
		obj.gameObject.SetActive(false);
		//obj.transform.SetParent(Instance.ParentObject.transform);

		switch (type)
		{
			case "CrabBulletVer1":
				Instance.carbBulletsVer1.Enqueue(obj);
				break;
			case "CrabBulletVer2":
				Instance.carbBulletsVer2.Enqueue(obj);
				break;
			case "Spear":
				Instance.spearBullets.Enqueue(obj);
				break;

		}
	}
}
