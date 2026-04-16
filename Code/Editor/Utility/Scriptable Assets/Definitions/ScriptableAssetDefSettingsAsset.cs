/*
 * Audio Manager (3.x)
 * Copyright (c) Carter Games
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the
 * GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version. 
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
 *
 * You should have received a copy of the GNU General Public License along with this program.
 * If not, see <https://www.gnu.org/licenses/>. 
 */

using System;
using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
	public class ScriptableAssetDefSettingsAsset : IScriptableAssetDef<AmAssetSettings>, ILegacyAssetPort
	{
		// IScriptableAssetDef Implementation
		/* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		private static AmAssetSettings cache;
		private static SerializedObject objCache;

		public Type AssetType => typeof(AmAssetSettings);
		public string DataAssetFileName => "[Audio Manager] Runtime Settings Asset.asset";
		public string DataAssetFilter => $"t:{typeof(AmAssetSettings).FullName} name={DataAssetFileName}";
		public string DataAssetPath => $"{ScriptableRef.FullPathData}{DataAssetFileName}";

		public AmAssetSettings AssetRef => ScriptableRef.GetOrCreateAsset(this, ref cache);
		public SerializedObject ObjectRef => ScriptableRef.GetOrCreateAssetObject(this, ref objCache);
		
		
		public void TryCreate()
		{
			ScriptableRef.GetOrCreateAsset(this, ref cache);
		}
		
		public void OnCreated() { }
		
		// ILegacyAssetPort Implementation
		/* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		public bool CanPort => AssetDatabaseHelper.TypeExistsElsewhere<AmAssetSettings>(DataAssetPath);
		
		
		public void PortAsset()
		{
			TryCreate();
			
			var assets = AssetDatabaseHelper.GetAssetPathNotAtPath<AmAssetSettings>(DataAssetPath).ToArray();

			if (assets.Length <= 0) return;

			UtilEditor.TransferProperties(
				new SerializedObject(AssetDatabase.LoadAssetAtPath(assets[0], typeof(AmAssetSettings))), ObjectRef);

			ObjectRef.ApplyModifiedProperties();
			ObjectRef.Update();

			foreach (var entry in assets)
			{
				AssetDatabase.DeleteAsset(entry);
			}
			
			AssetDatabase.SaveAssets();
		}
	}
}