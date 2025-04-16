using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{

	// ½Ì±ÛÅæ ¼±¾ð
	private static StageManager instance;
	public static StageManager Instance
	{
		get
		{
			if(null == instance)
			{
				return null;
			}
			return instance;
		}
	}
	// ½Ì±ÛÅæ ¼±¾ð

	public UnityEvent ComeBossZone;

	public UnityEvent StartBossStage;

	public UnityEvent GameClear;




	[SerializeField] private GameObject cutScene1;
	[SerializeField] private GameObject cutScene2;


	[Space(10)]
	[SerializeField] private PlayerCamera playerCamera;

	[Space(10)]
	[SerializeField] private GameOverManager _gameOverManager;
	[SerializeField] private GameClearManager _gameClearManager; 

	private bool isLock;
	public bool IsLock
	{
		set => isLock = value;
		get => isLock;
	}

	private bool isGameoverTextOut = false;

	private float time;

	private void Awake()
	{
		if (null == instance)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}


	private void Update()
	{

		if (GameManager.Instance.IsGameOver
			&& !isGameoverTextOut)
		{
			time += Time.deltaTime;

			if(time >= 2)
			{
				isGameoverTextOut = true;
				_gameOverManager.BlakcImageFadeOut();
			}

			return;
		}




	}



	public void CutSceneStart()
	{
		playerCamera.YPos = 2f;
		cutScene1.SetActive(true);
		cutScene2.SetActive(true);
	}

	public void StartBoss()
	{
		GameManager.Instance.CurrentStage = StageName.BossSnakehead;
	}		
}
