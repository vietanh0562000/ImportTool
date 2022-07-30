namespace THSound.THEditor {
	using UnityEditor;
	using UnityEngine;

	public class AudioProcessor : AssetPostprocessor {
		private const string MUSICBG_PATTERN = @"/AudioBg/";
		private const string SFX_PATTERN  = @"/AudioSfx/";
		
		void OnPreprocessAudio() {
			if (assetPath.Contains(MUSICBG_PATTERN)) {
				UnityEditor.AudioImporter   audioImporterEditor = assetImporter as UnityEditor.AudioImporter;
				AudioImporterSampleSettings audioSetting        = new AudioImporterSampleSettings();
				audioSetting.loadType               = AudioClipLoadType.CompressedInMemory;
				audioSetting.compressionFormat      = AudioCompressionFormat.Vorbis;
				audioSetting.quality                = 0.7f;
				audioImporterEditor.defaultSampleSettings = audioSetting;
			} else if (assetPath.Contains(SFX_PATTERN)) {
				UnityEditor.AudioImporter   audioImporterEditor = assetImporter as UnityEditor.AudioImporter;
				AudioImporterSampleSettings audioSetting        = new AudioImporterSampleSettings();
				audioSetting.loadType               = AudioClipLoadType.DecompressOnLoad;
				audioSetting.compressionFormat      = AudioCompressionFormat.PCM;
				audioImporterEditor.defaultSampleSettings = audioSetting;
			}
		}
		
		//https://blog.theknightsofunity.com/wrong-import-settings-killing-unity-game-part-2/
	}
}

