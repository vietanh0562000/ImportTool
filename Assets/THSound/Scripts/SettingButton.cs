namespace THSound {
	using UnityEngine;
	using UnityEngine.UI;

	public class SettingButton : MonoBehaviour {
		[SerializeField] private SettingType _typeSetting;

		[SerializeField] private Button _btn;
		[SerializeField] private Image  _onImg;
		[SerializeField] private Image  _offImg;

		private void OnEnable() {
			_btn.onClick.AddListener(ChangeSetting);
			bool current = SoundManager.Instance._dataSetting.Get_SettingData(_typeSetting);
			ToggleVisual(current);
		}
		private void ChangeSetting() {
			bool current = SoundManager.Instance._dataSetting.Get_SettingData(_typeSetting);
			SoundManager.Instance.soundAction_SaveSetting.Invoke(_typeSetting, !current);
			ToggleVisual(!current);
		}
		public void ToggleVisual(bool on) {
			_onImg.gameObject.SetActive(on);
			_offImg.gameObject.SetActive(!on);
		}
	}
}