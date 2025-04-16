using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{

	private static AudioManager instance;
	public static AudioManager Instance
	{
		get { return instance; }
	}





	[SerializeField] private AudioMixer mainAudioMixer;
	[SerializeField] private AudioSource audioSource;



	[Space(10)]
	[Header("BGM ÆÄÀÏ")]
	[SerializeField] private AudioClip titleBGM;
	[SerializeField] private AudioClip introBGM;
	[SerializeField] private AudioClip stage1MusicClip;
	[SerializeField] private AudioClip stage2MusicClip;
	[SerializeField] private AudioClip stage3MusicClip;
	[SerializeField] private AudioClip stage4MusicClip;
	[SerializeField] private AudioClip octopusBossBGM;
	[SerializeField] private AudioClip snakeheadBossBGM;



	private float masterSoundValue = 20f;
	private float sfxSoundValue = 20f;
	private float bgmSoundValue = 20f;


	private string currentAudio;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}


		audioSource = GetComponent<AudioSource>();
		DontDestroyOnLoad(gameObject);
	}


	private void Update()
	{
		if(SceneManager.GetActiveScene().name == "TitleScene")
		{
			GameManager.Instance.CurrentStage = StageName.Title;
		}

		ChangeStageBGM();
	}



	public void ChangeStageBGM()
	{
		if(GameManager.Instance.PreviousStage == GameManager.Instance.CurrentStage)
		{
			return;
		}

		GameManager.Instance.PreviousStage = GameManager.Instance.CurrentStage;

		audioSource.Pause();


		switch (GameManager.Instance.CurrentStage)
		{
			case StageName.Title:
				//audioSource.clip = titleBGM;
				
				break;
			case StageName.Intro:
				audioSource.clip = introBGM;

				break;
			case StageName.Stage01:
				audioSource.clip = stage1MusicClip;


				audioSource.Play();

				break;
			case StageName.Stage02:
				audioSource.clip = stage2MusicClip;


				audioSource.Play();

				break;
			case StageName.Stage03:
				audioSource.clip = stage3MusicClip;


				audioSource.Play();

				break;
			case StageName.Stage04:
				audioSource.clip = stage4MusicClip;


				audioSource.Play();

				break;
			case StageName.BossOctopus:

				break;
			case StageName.BossSnakehead:
				audioSource.clip = snakeheadBossBGM;


				audioSource.Play();

				break;
		}
	}






















	public AudioMixer MainAudioMixer
	{
		set => mainAudioMixer = value;
		get => mainAudioMixer;
	}

	public float MasterSoundValue
	{
		set => masterSoundValue = value;
		get => masterSoundValue;
	}
	public float SFXSoundValue
	{
		set => sfxSoundValue = value;
		get => sfxSoundValue;
	}
	public float BGMSoundValue
	{
		set => bgmSoundValue = value;
		get => bgmSoundValue;
	}
}
