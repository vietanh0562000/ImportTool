using System;
using Sirenix.OdinInspector;
using THSound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class THSoundExamplePanel : MonoBehaviour {
	[Header("Visual:")]
	[SerializeField] private Image _imgFillBgSound;
	[SerializeField] private Text _songName;

	private void Start() {
		SoundManager.Instance.soundAction_PlayBackground += OnAction_ChangeSongName;
	}

	
	private void OnAction_ChangeSongName(BgmBus soundBus) { _songName.text = soundBus.soundKey.ToString(); }
	
	private void FixedUpdate() {
		var audioSource = SoundManager.Instance.Get_BgmAudioSource();
		if (audioSource.isPlaying == false) { return; }

		var percent = audioSource.time * 1f / audioSource.clip.length;
		_imgFillBgSound.fillAmount = percent;
	}
}