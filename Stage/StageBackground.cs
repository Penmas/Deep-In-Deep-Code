using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageBackground : MonoBehaviour
{
	//오디오 관련 
	public AudioClip stage1MusicClip;
	public AudioClip stage2MusicClip;
    public AudioClip stage3MusicClip;
    public AudioClip stage4MusicClip;

	private AudioSource currentAudioSource;
	//

    [Header("오브젝트")]
	[SerializeField] private Image[] backImage;


	[Space(10)]
	[Header("챕터 별 배경 이미지")]
	[SerializeField] private Sprite[] chapter1;
	[SerializeField] private Sprite[] chapter2;
	[SerializeField] private Sprite[] chapter3;
	[SerializeField] private Sprite[] chapter4;


	[Space(10)]
	[Header("챕터 별 y좌표")]
	[SerializeField] private float[] yPos;



	private PlayerStateBase playerStateBase;


	private bool isCutSceneOut;

	private int currnetChapter = 0;

	private bool isBossStage;

	private void Awake()
	{
		playerStateBase = FindObjectOfType<PlayerStateBase>();
	}


	private void Update()
	{

		// 1 챕터
		if(playerStateBase.transform.position.y < yPos[0] 
			&& playerStateBase.transform.position.y > yPos[1])
		{
			if(currnetChapter != 1)
			{
				currnetChapter = 1;
				ChangeImages(1);
                PlayeStageMusic(1);

            }
			
		}
		else if (playerStateBase.transform.position.y < yPos[1]
			&& playerStateBase.transform.position.y > yPos[2])
		{
			if (currnetChapter != 2)
			{
				currnetChapter = 2;
				ChangeImages(2);
                PlayeStageMusic(2);
            }
		}
		else if (playerStateBase.transform.position.y < yPos[2]
			&& playerStateBase.transform.position.y > yPos[3])
		{
			if (currnetChapter != 3)
			{
				currnetChapter = 3;
				ChangeImages(3);
                PlayeStageMusic(3);
            }
		}
		else if (playerStateBase.transform.position.y < yPos[3]
			&& playerStateBase.transform.position.y > yPos[4])
		{
			if (currnetChapter != 4)
			{
				currnetChapter = 4;
				ChangeImages(4);
				PlayeStageMusic(4);

            }
		}




		if(isCutSceneOut)
		{
			return;
		}

		if(playerStateBase.gameObject.transform.position.y < yPos[4])
		{
			isCutSceneOut = true;
			StageManager.Instance.ComeBossZone.Invoke();
		}
	}

	public void ChangeImages(int chapter)
	{
		
		switch(chapter)
		{
			case 1:
				for (int i = 0; i < backImage.Length; ++i)
				{
					backImage[i].sprite = chapter1[i];
				}
				break;
			case 2:
				for (int i = 0; i < backImage.Length; ++i)
				{
					backImage[i].sprite = chapter2[i];
				}
				break;
			case 3:
				for (int i = 0; i < backImage.Length; ++i)
				{
					backImage[i].sprite = chapter3[i];
				}
				break;
			case 4:
				for (int i = 0; i < backImage.Length; ++i)
				{
					backImage[i].sprite = chapter4[i];
				}

				break;
		}

		
	}

	void PlayeStageMusic(int _stage)
	{

		if (isBossStage)
		{
			return;
		}

		//GameObject audioSourceObject = new GameObject("StageSoundManager");
		//audioSourceObject.transform.position = transform.position;
		//currentAudioSource = audioSourceObject.AddComponent<AudioSource>();

		switch(_stage)
		{
			case 1:
				GameManager.Instance.CurrentStage = StageName.Stage01;
				//currentAudioSource.clip = stage1MusicClip;
				break;
			case 2:
				GameManager.Instance.CurrentStage = StageName.Stage02;
				break;
            case 3:
				GameManager.Instance.CurrentStage = StageName.Stage03;
				break;
            case 4:
				GameManager.Instance.CurrentStage = StageName.Stage04;
				break;
			default:
				break;
        }
    }


	public void isBossStageAudioChange()
	{
		isBossStage = true;
	}
}
