namespace THSound {
	using Sirenix.OdinInspector;
	using UnityEngine;

	[DefaultExecutionOrder(0)]
	public class SoundBus : MonoBehaviour {
#if UNITY_EDITOR
		private bool soundCondition             => _type == SettingType.SFXSound || _type == SettingType.BGMusic;
		private bool sfx3DSoundCondition        => _type == SettingType.SFXSound && _3dSound == true;
		private bool bgmChangeOvertimeCondition => _type == SettingType.BGMusic && _dynamicVolumeAtStart == true;
#endif

		[PropertySpace(5, 20)] public SettingType _type;
		/*----------------------------------------------------------------------------------------------Vibrate setting-*/
		[ShowIf("_type", SettingType.Vibrate)] [Indent(2)] public VibrationType _vibrateType = VibrationType.Medium;

		/*--------------------------------------------------------------------------------------------Sound key setting-*/
		[ShowIf("_type", SettingType.BGMusic)] [Indent(2)]  public SoundBgmKey _soundBgmKey;
		[ShowIf("_type", SettingType.SFXSound)] [Indent(2)] public SoundKey    _soundKey;


		/*------------------------------------------------------------------------------------------------Sound setting-*/
		[ShowIf(nameof(soundCondition))] [HideIf("_dynamicVolumeAtStart")] [Indent(2)] [Range(0, 1)] public float _volume = 1;

		/*--------------------------------------------------------------------------------------------Sound BGM setting-*/
		[ShowIf("_type", SettingType.BGMusic)] [Indent(2)] public bool _playOnStart;
		[ShowIf("_type", SettingType.BGMusic)] [Indent(2)] public bool _dynamicVolumeAtStart;
		
		[ShowIf(nameof(bgmChangeOvertimeCondition))] [Indent(2)] public AnimationCurve _curve;
		[ShowIf(nameof(bgmChangeOvertimeCondition))] [Indent(2)] public float          _effectDuration;
		[ShowIf(nameof(bgmChangeOvertimeCondition))] [Indent(2)] public bool           _intertwineAtStart;
		[ShowIf(nameof(bgmChangeOvertimeCondition))] [ShowIf("_intertwineAtStart")] 
		[Indent(2)] [Range(0, 1)]public float _changePoint;
		
		
		/*--------------------------------------------------------------------------------------------Sound SFX setting-*/
		[ShowIf("_type", SettingType.SFXSound)] [Indent(2)] public bool _cancelSameSound;
		[ShowIf("_type", SettingType.SFXSound)] [Indent(2)] public bool _randomPitch;
		[ShowIf("_type", SettingType.SFXSound)] [Indent(2)] public int  _loop;
		[ShowIf("_type", SettingType.SFXSound)] [Indent(2)] public bool _3dSound;

		[ShowIf(nameof(sfx3DSoundCondition))] [Indent(3)] public float _maxDistance = 500f;

		private void Start() {
			if (_type == SettingType.BGMusic && _playOnStart) {
				SendBGMSignal();
			}
		}

		public void SendActionSignal() {
			switch (_type) {
				case SettingType.Vibrate:
					SendVibrateSignal();
					break;
				case SettingType.BGMusic:
					SendBGMSignal();
					break;
				case SettingType.SFXSound:
					SendSFXSignal();
					break;
			}
		}

		private void SendVibrateSignal() {
			var bus = new VibrateBus() {vibrateType = _vibrateType};
			SoundManager.Instance.soundAction_PlayVibrate?.Invoke(bus);
		}

		private void SendBGMSignal() {
			BgmBus bus;
			if (_dynamicVolumeAtStart) {
				bus = new BgmBus(_soundBgmKey, 
				                 _dynamicVolumeAtStart, _curve, _effectDuration,
				                 _intertwineAtStart, _changePoint);
			} else {
				bus = new BgmBus(_soundBgmKey, _volume);
			}

			SoundManager.Instance.soundAction_PlayBackground?.Invoke(bus);
		}
		private void SendSFXSignal() {
			var bus = new SfxBus() {
				                       soundKey = _soundKey, volume = _volume, loop = _loop, 
				                       cancelSameSound = _cancelSameSound, randomPitch = _randomPitch,
				                       sound3D  = _3dSound, posSound = transform.position, maxDistance = _maxDistance
			                       };
			SoundManager.Instance.soundAction_PlaySfx?.Invoke(bus);
		}
		private void OnDrawGizmos() {
			if (!_3dSound) { return; }

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, _maxDistance);
		}
	}
}