using CarterGames.Assets.Shared.Common;

namespace CarterGames.Assets.AudioManager
{
	public static class LibraryHelper
	{
		private static AudioLibrary LibraryRef => AssetAccessor.GetAsset<AudioLibrary>();
		
		
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