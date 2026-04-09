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

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CarterGames.Shared.AudioManager.Editor
{
    /// <summary>
    /// The new v2 setup for checking asset versions. Queries an API instead of a JSON file directly.
    /// </summary>
    public static class VersioningAPI
    {
        /// <summary>
        /// Call to query for the assets latest version on the server.
        /// </summary>
        /// <param name="onSuccess">Logic to run on success.</param>
        /// <param name="onFailed">Logic to run on failure.</param>
        public static void Query(Action<VersionPacketSuccess> onSuccess, Action<VersionPacketError> onFailed)
        {
            var url = $"api.carter.games/v0/versioning/query?id={VersionInfo.Key}";
            
            var request = UnityWebRequest.Get(url);
            request.timeout = 5;

            var op = request.SendWebRequest();
            op.completed -= OnRequestCompleted;
            op.completed += OnRequestCompleted;

            return;

            void OnRequestCompleted(AsyncOperation operation)
            {
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    onFailed?.Invoke(VersionPacketError.Custom("Cannot connect to versioning API. Please try again later."));
                }
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        onSuccess?.Invoke(JsonUtility.FromJson<VersionPacketSuccess>(request.downloadHandler.text));
                    }
#pragma warning disable 0168
                    catch (Exception e)
#pragma warning restore 0168
                    {
                        onFailed?.Invoke(VersionPacketError.Custom("Couldn't parse data received."));
                    }
                }
                else
                {
                    onFailed?.Invoke(JsonUtility.FromJson<VersionPacketError>(request.downloadHandler.text));
                }
            }
        }
    }
}