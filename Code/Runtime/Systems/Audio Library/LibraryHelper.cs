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

namespace CarterGames.Assets.AudioManager
{
	public static class LibraryHelper
	{
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Properties
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		private static AudioLibrary LibraryRef => AmAssetAccessor.GetAsset<AudioLibrary>();
		
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Methods
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		public static bool HasClip(string request)
		{
			return LibraryRef.LibraryLookup.ContainsKey(request) || LibraryRef.ReverseLibraryLookup.ContainsKey(request);
		}
		
		
		public static bool HasGroup(string groupId)
		{
			return LibraryRef.GroupsLookup.ContainsKey(groupId) || LibraryRef.ReverseGroupsLookup.ContainsKey(groupId);
		}
		
		
		public static bool HasMixer(string mixerId)
		{
			return LibraryRef.MixerLookup.ContainsKey(mixerId) || LibraryRef.ReverseMixerLookup.ContainsKey(mixerId);
		}
	}
}