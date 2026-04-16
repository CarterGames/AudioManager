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

using CarterGames.Shared.AudioManager;
using CarterGames.Shared.AudioManager.Editor;

namespace CarterGames.Assets.AudioManager.Editor
{
	/// <summary>
	/// Handles porting the asset data assets from 3.0.x -> 3.1.x+
	/// </summary>
	public sealed class AudioManagerAssetPort : IAssetEditorInitialize, IAssetEditorReload
	{
		// IAssetEditorInitialize Implementation
		/* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		public int InitializeOrder => 2000;
		
		
		public void OnEditorInitialized()
		{
			TryPortAssets();
		}
		
		// IAssetEditorReload Implementation
		/* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		public void OnEditorReloaded()
		{
			TryPortAssets();
		}
		
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Methods
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Tries to port the assets, if they are already ported nothing will happen.
		/// </summary>
		private void TryPortAssets()
		{
			foreach (var porting in AssemblyHelper.GetClassesOfType<ILegacyAssetPort>())
			{
				if (!porting.CanPort) continue;
				porting.PortAsset();
			}
		}
	}
}