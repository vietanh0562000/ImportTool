namespace THSound {
	using System;
	using UnityEngine;

	[Serializable]
	public class SoundSettingData : MonoBehaviour {
		public string _prefString;
		public bool   _isBGMusicAllowed;
		public bool   _isSFXAllowed;
		public bool   _isVibrateAllowed;

		public void Initialize(string keyPlayerPrefs) {
			_prefString = keyPlayerPrefs;
			if (PlayerPrefs.GetString(_prefString).Equals("")) {
				ResetData();
			}

			try {
				JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(_prefString), this);
			}
			catch (Exception e) {
				ResetData();
				Debug.LogError("Error On Load PlayerPrefs...");
				Debug.LogError("Error : " + e);
			}
		}

		/*--------------------------------------------------------------------------------------------Call-out function-*/
		public bool Get_SettingData(SettingType type) {
			switch (type) {
				case SettingType.BGMusic:
					return _isBGMusicAllowed;
				case SettingType.SFXSound:
					return _isSFXAllowed;
				case SettingType.Vibrate:
					return _isVibrateAllowed;
			}

			return true;
		}

		public void Set_SettingData(SettingType type, bool isAllowed) {
			switch (type) {
				case SettingType.BGMusic:
					_isBGMusicAllowed = isAllowed;
					break;
				case SettingType.SFXSound:
					_isSFXAllowed = isAllowed;
					break;
				case SettingType.Vibrate:
					_isVibrateAllowed = isAllowed;
					break;
			}

			Save();
		}

		/*-----------------------------------------------------------------------------------------------Inner function-*/
		private void ResetData() {
			_isVibrateAllowed = _isBGMusicAllowed = _isSFXAllowed = true;
			Save();
		}

		private void Save() {
			string json = JsonUtility.ToJson(this);
			PlayerPrefs.SetString(_prefString, json);
			// Debug.Log(DebugHelper.GetColorfulLog(json));
		}
	}
}