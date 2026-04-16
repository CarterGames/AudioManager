using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public class UtilityEditorWindow : EditorWindow
	{
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		private static UtilityEditorWindow window;

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Properties
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Gets if the window is currently open.
		/// </summary>
		public static bool IsOpen => window != null;
		
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Methods
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Opens the window when called.
		/// </summary>
		/// <param name="windowName">The name to show on the window title.</param>
		/// <typeparam name="T">The type the window is.</typeparam>
		public static void Open<T>(string windowName) where T : UtilityEditorWindow
		{
			window = GetWindow<T>(true);
			window.titleContent = new GUIContent(windowName);
			window.Show();
		}


		/// <summary>
		/// Closes the window when called.
		/// </summary>
		public new void Close()
		{
			if (window == null) return;
			((EditorWindow) window).Close();
			window = null;
		}
	}
}