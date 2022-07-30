namespace THSound {
	using UnityEngine;

	public class SoundBusInfor {
		public SettingType type;
	}

	public class VibrateBus : SoundBusInfor {
		public VibrationType vibrateType = VibrationType.Medium;
	}

	public class SfxBus : SoundBusInfor {
		public SoundKey soundKey;
		public float    volume          = 1;
		public int      loop            = 0;
		public bool     cancelSameSound = false;
		public bool     randomPitch     = false;
		public bool     sound3D         = false;
		public Vector3  posSound        = Vector3.zero;
		public float    maxDistance     = 500f;
	}

	public class BgmBus : SoundBusInfor {
		public SoundBgmKey soundKey;
		public float       volume = 1;

		public bool           dynamicVolume = false;
		public AnimationCurve curveVolume;
		public float          effectDuration;
		public bool           intertwineAtStart = false;
		public float          intertwinePoint;

		public BgmBus(SoundBgmKey soundKey, float volume) {
			this.soundKey = soundKey;
			this.volume   = volume;
		}
		public BgmBus(SoundBgmKey soundKey, bool dynamicVolume, AnimationCurve curveVolume, float effectDuration,
		              bool intertwineAtStart, float intertwinePoint) {
			this.soundKey          = soundKey;
			this.dynamicVolume     = dynamicVolume;
			this.curveVolume       = curveVolume; 
			this.effectDuration    = effectDuration;
			this.intertwineAtStart = intertwineAtStart;
			this.intertwinePoint   = intertwinePoint;
		}
	}
}