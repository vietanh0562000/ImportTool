using UnityEngine;

namespace THSound {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using MoreMountains.NiceVibrations;
	using Sirenix.OdinInspector;
	using Random = UnityEngine.Random;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	// [ExecuteAlways]
	[DefaultExecutionOrder(-10)]
	public class SoundManager : MonoBehaviour {
		[SerializeField] private string PLAYERPREFS_STORE_KEY = "TH_STORE_KEY";
		[Space]
		[Header("AudioSource prefab to spawn:")]
		[AssetSelector(Paths = "Assets/THSound/Prefabs")]
		[SerializeField] private AudioSource _prefabsAudioSource;

		private AudioSource       _bgmAudioSource;
		private List<AudioSource> _sfxAudioSources;
		private Coroutine         _bgmCoroutine;
		private BgmBus            _tempBus;
		/*----------------------------------------------------------------------------------Data store in player device-*/
		[PropertySpace(spaceBefore: 20)]
		[ReadOnly] [ShowInInspector] public SoundSettingData _dataSetting;
		[ReadOnly] [ShowInInspector] public List<AudioClip> _bgClips;
		[ReadOnly] [ShowInInspector] public List<AudioClip> _sfxClips;

		#region AUTOVALIDATE_DIRECTORY

#if UNITY_EDITOR
		[PropertySpace(spaceBefore: 20)]
		[FolderPath] [SerializeField] private string bgClipsPath;
		[FolderPath] [SerializeField] private string sfxClipsPath;

		[PropertySpace(spaceBefore: 20)]
		[Button("Re-validate")]
		public void ReValidate() {
			ValidateBgSound();
			ValidateSfxSound();
		}

		private void ValidateBgSound() {
			var removeAssetPath = bgClipsPath.Substring(6, bgClipsPath.Length - 6);
			var dir             = new DirectoryInfo(Application.dataPath + removeAssetPath);

			string                supportedExtensions = "*.mp3,*.wav";
			IEnumerable<FileInfo> files;
			try {
				files = dir.GetFiles("*.*", SearchOption.AllDirectories)
				           .Where(_ => supportedExtensions.Contains(Path.GetExtension(_.FullName).ToLower()));

				_bgClips.Clear();

				//Checks all files and stores all WAV files into the Files list.
				string[] separator = new[] {"Assets"};
				foreach (var file in files) {
					var path      = file.FullName;
					var split     = path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					var finalPath = @"Assets\" + split[split.Length - 1];
					var clip      = AssetDatabase.LoadAssetAtPath<AudioClip>(finalPath);
					_bgClips.Add(clip);
				}

				Debug.Log(DebugHelper.GetColorfulLog($"{_bgClips.Count} bgSound imported."));
			}
			catch (Exception exception) {
				Debug.Log(DebugHelper
					          .GetColorfulLog($"Bg's path is not found: {Application.dataPath + removeAssetPath}",
					                          DebugHelper.BugLogColor));
			}
		}

		private void ValidateSfxSound() {
			var removeAssetPath = sfxClipsPath.Substring(6, sfxClipsPath.Length - 6);
			var dir             = new DirectoryInfo(Application.dataPath + removeAssetPath);

			string supportedExtensions = "*.mp3,*.wav";

			IEnumerable<FileInfo> files;
			try {
				files = dir.GetFiles("*.*", SearchOption.AllDirectories)
				           .Where(_ => supportedExtensions.Contains(Path.GetExtension(_.FullName).ToLower()));

				_sfxClips.Clear();

				//Checks all files and stores all WAV files into the Files list.
				string[] separator = new[] {"Assets"};
				foreach (var file in files) {
					var path      = file.FullName;
					var split     = path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					var finalPath = @"Assets\" + split[split.Length - 1];
					var clip      = AssetDatabase.LoadAssetAtPath<AudioClip>(finalPath);
					_sfxClips.Add(clip);
				}

				Debug.Log(DebugHelper.GetColorfulLog($"{_sfxClips.Count} sfxSound imported."));
			}
			catch (Exception exception) {
				Debug.Log(DebugHelper
					          .GetColorfulLog($"Sfx's path is not found: {Application.dataPath + removeAssetPath}",
					                          DebugHelper.BugLogColor));
			}
		}
#endif

		#endregion


		/*-------------------------------------------------------------------------------------------------------Action-*/
		public Action<VibrateBus>        soundAction_PlayVibrate;
		public Action<BgmBus>            soundAction_PlayBackground;
		public Action<SfxBus>            soundAction_PlaySfx;
		public Action<SettingType, bool> soundAction_SaveSetting;
		private void OnEnable() {
			_sfxAudioSources = new List<AudioSource>();

			_dataSetting                  = new GameObject("TH_SOUND_DATA").AddComponent<SoundSettingData>();
			_dataSetting.transform.parent = transform;
			_dataSetting.Initialize(PLAYERPREFS_STORE_KEY);

			_bgmAudioSource                  = new GameObject("Bgm_AudioSource").AddComponent<AudioSource>();
			_bgmAudioSource.loop             = true;
			_bgmAudioSource.playOnAwake      = false;
			_bgmAudioSource.transform.parent = transform;


			soundAction_PlayVibrate      += this.ActionPlayVibrate;
			soundAction_PlayBackground   += this.ActionPlayBackground;
			soundAction_PlaySfx          += this.ActionPlaySfx;
			this.soundAction_SaveSetting += _dataSetting.Set_SettingData;
			this.soundAction_SaveSetting += this.ChangeCurrentSoundState;
		}

		#region CALL-OUT

		public void StopAllSound() {
			StopBackgroundSound();
			StopSFXSound();
		}

		public void StopBackgroundSound() {
			if (_bgmCoroutine != null) {
				StopCoroutine(_bgmCoroutine);
			}

			_bgmAudioSource.Stop();
		}

		public void StopSFXSound() {
			foreach (var audiSource in _sfxAudioSources) { audiSource.Stop(); }
		}

		#endregion

		#region Vibrate

		private void ActionPlayVibrate(VibrateBus busInfor) {
			if (!_dataSetting.Get_SettingData(SettingType.Vibrate)) { return; }

			switch (busInfor.vibrateType) {
				case VibrationType.Heavy:
					MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
					break;
				case VibrationType.Medium:
					MMVibrationManager.Haptic(HapticTypes.MediumImpact);
					break;
				case VibrationType.Light:
					MMVibrationManager.Haptic(HapticTypes.LightImpact);
					break;
			}
		}

		#endregion

		#region BGM_Audio

		private void ActionPlayBackground(BgmBus busInfor) {
			_tempBus = busInfor;

			if (!_dataSetting.Get_SettingData(SettingType.BGMusic)) { return; }

			if (_bgmCoroutine != null) { StopCoroutine(_bgmCoroutine); }

			if (_tempBus.dynamicVolume) {
				_bgmCoroutine = StartCoroutine(ChangeVolumeOvertime());
			} else {
				BgmCasualPlay();
			}
		}
		private void BgmCasualPlay() {
			_bgmAudioSource.clip   = _bgClips[(int) _tempBus.soundKey];
			_bgmAudioSource.volume = _tempBus.volume;
			_bgmAudioSource.Play();
		}
		
		private IEnumerator ChangeVolumeOvertime() {
			var fixedTime       = 0.02f;
			var tickTime        = new WaitForSeconds(fixedTime);
			int totalTicks      = (int) (_tempBus.effectDuration / fixedTime);
			int changeTickPoint = 0;
			if (_tempBus.intertwineAtStart) {
				changeTickPoint = (int) (_tempBus.intertwinePoint * totalTicks);
			}

			_bgmAudioSource.volume = _tempBus.curveVolume.Evaluate(0);
			
			for (int i = 0; i < totalTicks; i++) {
				if (changeTickPoint == i) {
					_bgmAudioSource.clip = _bgClips[(int) _tempBus.soundKey];
					if(_bgmAudioSource.isPlaying == false) { _bgmAudioSource.Play(); } 
				}
				
				yield return tickTime;
				var timePosInCurve     = i * 1f / totalTicks;
				var valueAtCurve       = _tempBus.curveVolume.Evaluate(timePosInCurve);
				_bgmAudioSource.volume = valueAtCurve;
			}

			_bgmAudioSource.volume = _tempBus.curveVolume.Evaluate(1);
		}

		#endregion

		#region SFX_Audio

		private void ActionPlaySfx(SfxBus busInfor) {
			if (!_dataSetting.Get_SettingData(SettingType.SFXSound)) { return; }

			if (busInfor.cancelSameSound) {
				foreach (var sfxSource in _sfxAudioSources) {
					if(sfxSource.clip == _sfxClips[(int) busInfor.soundKey]) sfxSource.Stop();
				}
			}
			
			var audioSource = Get_AudioSourceToPlay();

			audioSource.clip   = _sfxClips[(int) busInfor.soundKey];
			audioSource.volume = busInfor.volume;

			
			if (busInfor.randomPitch) {
				audioSource.pitch = 1 + Random.Range(-0.5f, 0.5f);
			} else {
				audioSource.pitch = 1;
			}

			audioSource.maxDistance        = busInfor.maxDistance;
			audioSource.transform.position = busInfor.posSound;

			if (busInfor.loop < 1) {
				audioSource.Play();
			} else {
				StartCoroutine(LoopSFXAudio(audioSource, busInfor.loop));
			}
		}

		private IEnumerator LoopSFXAudio(AudioSource source, int loopTime) {
			float length   = source.clip.length;
			var   waitTime = new WaitForSeconds(length);

			for (int i = 0; i < loopTime; i++) {
				source.Play();
				yield return waitTime;
			}
		}

		#endregion

		#region OnChangeSetting

		private void ChangeCurrentSoundState(SettingType type, bool isOn) {
			if (type == SettingType.BGMusic) {
				if (isOn) {
					ActionPlayBackground(_tempBus);
				} else {
					StopBackgroundSound();
				}

				return;
			}

			if (type == SettingType.SFXSound && isOn == false) {
				StopSFXSound();
			}
		}

		#endregion

		/*-----------------------------------------------------------------------------------------Persistent singleton-*/

		#region PERSISTENT_SINGLETON

		public static SoundManager Instance;
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				DontDestroyOnLoad(gameObject);
			} else {
				Debug.Log($"Destroy because THSoundManager already exist!");
				Destroy(gameObject);
			}
		}

		#endregion

		/*---------------------------------------------------------------------------------------------Utility Function-*/
		public AudioSource Get_BgmAudioSource() { return _bgmAudioSource; }
		private AudioSource Get_AudioSourceToPlay() {
			//create first
			if (_sfxAudioSources.Count == 0) {
				var firstAudioSource = Instantiate(_prefabsAudioSource).GetComponent<AudioSource>();
				if (firstAudioSource == null) { return null; }

				firstAudioSource.transform.parent = gameObject.transform;
				_sfxAudioSources.Add(firstAudioSource);
				return firstAudioSource;
			}

			// use if already exist but inactive
			for (int i = 0; i < _sfxAudioSources.Count; i++) {
				if (_sfxAudioSources[i].isPlaying == false ||
				    _sfxAudioSources[i].gameObject.activeInHierarchy == false) {
					return _sfxAudioSources[i];
				}
			}

			//create new if all busy
			var newAudioSource = Instantiate(_prefabsAudioSource).GetComponent<AudioSource>();
			if (newAudioSource == null) { return null; }

			newAudioSource.transform.parent = gameObject.transform;
			_sfxAudioSources.Add(newAudioSource);
			return newAudioSource;
		}
	}
}