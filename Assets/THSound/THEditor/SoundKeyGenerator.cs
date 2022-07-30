namespace THSound.THEditor {
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	public static class SoundKeyGenerator {
		private static string EnumVar    = @"#enumVar#";
		private static string EnumBgmVar = @"#enumBgmVar#";

		private static string SoundEnumClassContent =
			@"
namespace THSound {
	public enum SoundKey {
	"
		   +
			EnumVar
		   +
			@"
	}"
		   +
			@"

	public enum SoundBgmKey {
	"
		   +
			EnumBgmVar
		   +
			@"
	}
}
		";

	
		public static void Generate(string pathMusicBg, string pathSFX) {
			string supportedExtensions = "*.mp3,*.wav";
			string final               = SoundEnumClassContent;
			/*------------------------------------------------------------------------------------------------Background-*/
			string        bgClipsPath     = pathMusicBg;
			string        removeAssetPath = bgClipsPath.Substring(6, bgClipsPath.Length - 6);
			DirectoryInfo dir             = new DirectoryInfo(Application.dataPath + removeAssetPath);

			StringBuilder         sbBg = new StringBuilder();
			IEnumerable<FileInfo> files;

			files = dir.GetFiles("*.*", SearchOption.AllDirectories)
			           .Where(_ => supportedExtensions.Contains(Path.GetExtension(_.FullName).ToLower()));
			
			foreach (var file in files) {
				var path     = file.Name;
				var nameOnly = path.Substring(0, path.LastIndexOf("."));

				var refineName = "Bgm" + RefineName(nameOnly);
				sbBg.Append(refineName);
				sbBg.Append(",");
			}

			final = final.Replace(EnumBgmVar, sbBg.ToString());
			/*-------------------------------------------------------------------------------------------------------SFX-*/
			string        sfxClipPath        = pathSFX;
			string        removeAssetPathSfx = sfxClipPath.Substring(6, sfxClipPath.Length - 6);
			DirectoryInfo dirSfx             = new DirectoryInfo(Application.dataPath + removeAssetPathSfx);

			StringBuilder         sb = new StringBuilder();
			IEnumerable<FileInfo> filesSfx;
			
			filesSfx = dirSfx.GetFiles("*.*", SearchOption.AllDirectories)
			                 .Where(_ => supportedExtensions.Contains(Path.GetExtension(_.FullName).ToLower()));

			foreach (var file in filesSfx) {
				var path     = file.Name;
				var nameOnly = path.Substring(0, path.LastIndexOf("."));

				var refineName = "Sfx" + RefineName(nameOnly);
				sb.Append(refineName);
				sb.Append(",");
			}

			final = final.Replace(EnumVar, sb.ToString());
			
			/*-----------------------------------------------------------------------------------------------------Final-*/
			final = final.Replace(",", ", ");
			/*------------------------------------------------------------------------------------------------------Save-*/
			Debug.Log(DebugHelper.GetColorfulLog(final));
			string dedicatedScriptPath = "/THSound/Scripts/SoundKey.cs";
			string filePath            = Application.dataPath + dedicatedScriptPath;
			using (var sw = new StreamWriter(filePath, false)) {
				sw.Write(final);
				sw.Dispose();
				sw.Close();

				AssetDatabase.ImportAsset(@"Assets\" + dedicatedScriptPath);
			}
			
			
		}
		private static string RefineName(string name) {
			var result = ObjectNames.NicifyVariableName(name);
			result = name.Replace(" ", "");
			result = result.Replace("-", "");
			result = result.Replace("_", "");
			return result;
		}
	}
}