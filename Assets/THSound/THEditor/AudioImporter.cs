namespace THSound.THEditor {
	using System.IO;
	using Newtonsoft.Json;
	using Sirenix.OdinInspector;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities;
	using Sirenix.Utilities.Editor;
	using UnityEditor;
	using UnityEngine;

	public class THAudioImporterEditor : OdinEditorWindow {
		public const string JSON_PATH = "/THSound/THEditor/FolderPath.json";

		[PropertySpace(spaceBefore: 10)]
		[FolderPath] [SerializeField] private string _pathAudioBackground;
		[PropertySpace(spaceBefore: 10)]
		[FolderPath] [SerializeField] private string _pathAudioSFX;


		[MenuItem("Tools/3DX Importer/Audio Importer")]
		private static void OpenWindow() {
			var window = GetWindow<THAudioImporterEditor>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 150);
		}

		[PropertySpace(spaceBefore: 40)] [Button]
		private void GenerateSoundKey() { SoundKeyGenerator.Generate(_pathAudioBackground, _pathAudioSFX); }

		private void Save_PathAsset() {
			AudioPath pathData = new AudioPath(_pathAudioBackground, _pathAudioSFX);
			File.WriteAllText(Application.dataPath + JSON_PATH, JsonConvert.SerializeObject(pathData));
		}

		private void Get_PathAsset() {
			AudioPath pathData =
				JsonConvert.DeserializeObject<AudioPath>(File.ReadAllText(Application.dataPath + JSON_PATH));
			_pathAudioBackground = pathData.musicPath;
			_pathAudioSFX        = pathData.sfxPath;
		}
		protected override void OnEnable() {
			base.OnEnable();
			Get_PathAsset();
		}

		protected override void OnGUI() {
			base.OnGUI();
			if (EditorUtility.IsDirty(this)) {
				this.Save_PathAsset();
			}
		}
	}

	public class AudioPath {
		public string musicPath;
		public string sfxPath;
		public AudioPath(string musicPath, string sfxPath) {
			this.musicPath = musicPath;
			this.sfxPath   = sfxPath;
		}
	}
}