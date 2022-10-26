using System.Collections;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A class to hold the audio args setup.
    /// </summary>
    public static class ArgsContainer
    {
        /// <summary>
        /// Passes through the arguments for the audio source.
        /// Is Static & can be accessed anywhere for use.
        /// </summary>
        /// <param name="values">Parings to pass through.</param>
        /// <returns>Setup with the values you have entered.</returns>
        public static Hashtable AudioArgs(params object[] values)
        {
            var table = new Hashtable(values.Length / 2);

            if (!(values.Length % 2).Equals(0)) return null;

            var i = 0;

            while (i < values.Length - 1)
            {
                table.Add(values[i], values[i + 1]);
                i += 2;
            }

            return table;
        }
    }
}