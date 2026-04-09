/*
 * Copyright (c) 2025 Carter Games
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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