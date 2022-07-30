namespace THSound {
	using System;
	using System.Drawing;
	using Object = UnityEngine.Object;

	public class DebugHelper {
		public const string DebugColor  = "#60C7FF";
		public const string BugLogColor = "#FF6257";
		public static string GetColorfulLog(string logMessage, string color = DebugColor) {
			return $"<color={color}> {logMessage}</color>";
		}
	}
}