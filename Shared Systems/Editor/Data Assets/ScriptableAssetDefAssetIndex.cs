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
using UnityEditor;

namespace CarterGames.Shared.AudioManager.Editor
{
	/// <summary>
	/// Handles the creation and referencing of the asset index file.
	/// </summary>
	public sealed class ScriptableAssetDefAssetIndex : IScriptableAssetDef<AmDataAssetIndex>
	{
		// IScriptableAssetDef Implementation
		/* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		private static AmDataAssetIndex cache;
		private static SerializedObject objCache;

		public Type AssetType => typeof(AmDataAssetIndex);
		public string DataAssetFileName => $"[{AssetVersionData.AssetName}] Asset Index.asset";
		public string DataAssetFilter => $"t:{typeof(AmDataAssetIndex).FullName} name={DataAssetFileName}";
		public string DataAssetPath => $"{ScriptableRef.FullPathResources}{DataAssetFileName}";

		public AmDataAssetIndex AssetRef => ScriptableRef.GetOrCreateAsset(this, ref cache);
		public SerializedObject ObjectRef => ScriptableRef.GetOrCreateAssetObject(this, ref objCache);
		
		public void TryCreate()
		{
			ScriptableRef.GetOrCreateAsset(this, ref cache);
		}

		public void OnCreated() { }

		// ILegacyAssetPort Implementation
		/* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		public bool CanPort => AssetDatabaseHelper.TypeExistsElsewhere<AmDataAssetIndex>(DataAssetPath);
		

		public void PortAsset()
		{
			TryCreate();

			var assets = AssetDatabaseHelper.GetAssetPathNotAtPath<AmDataAssetIndex>(DataAssetPath);

			if (assets != null)
			{
				foreach (var entry in assets)
				{
					AssetDatabase.DeleteAsset(entry);
				}
			}
			
			AssetDatabase.SaveAssets();
			AssetIndexHandler.UpdateIndex();
		}
	}
}