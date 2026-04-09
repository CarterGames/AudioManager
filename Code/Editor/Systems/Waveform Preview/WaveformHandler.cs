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

using UnityEditor;
using UnityEngine;

/*
 *   Credit: りじちょー (Rijicho_nl)
 *   Links: https://qiita.com/Rijicho_nl/items/c7e6a5cb9cf56e52588a & https://qiita.com/Rijicho_nl/items/3c51befd7bbe63c00878
 */

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A class to help draw a audio clip waveform.
    /// </summary>
    public static class WaveformHandler
    {
        // <summary>
        /// Invoked in RenderPreview() to define colors of AudioClip preview curves
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="t">Time (x value) between 0f(left-end), 1f(right-end)</param>
        /// <param name="min">Minimum y value of the curve at the time</param>
        /// <param name="max">Maximum y value of the curve at the time</param>
        /// <param name="minOfAll">Minimum y value of the curve</param>
        /// <param name="maxOfAll">Maximum y value of the curve</param>
        /// <returns>Color of the curve at the time</returns>
        public delegate Color AudioCurveColorSetter(int channel, float t, float min, float max, float minOfAll,
            float maxOfAll);

        
        /// <summary>
        /// Render waveform preview of the clip in given rect. If clip is null, do nothing.
        /// </summary>
        /// <param name="rect">Rect in which the wave will be rendered</param>
        /// <param name="clip">AudioClip source</param>
        /// <param name="colorSetter">Delegate for coloring the wave. Default: Color(1,0.54902,0)</param>
        /// <param name="amplitudeScale">Y-scale amplification of the wave</param>
        public static void RenderPreview(Rect rect, AudioClip clip, AudioCurveColorSetter colorSetter = null,
            float amplitudeScale = 1)
        {
            if (!clip) return;

            //公式実装に倣って補正
            amplitudeScale *= 0.95f;

            //データ取得
            var audioImporter = InternalAudioUtil.GetImporterFromClip(clip);
            float[] minMaxData = (audioImporter == null) ? null : InternalAudioUtil.GetMinMaxData(audioImporter);

            //全体の最大値・最小値を計算
            float minOfAll = 0;
            float maxOfAll = 0;
            
            for (int i = 0; i < minMaxData.Length; i++)
            {
                minOfAll = Mathf.Min(minMaxData[i], minOfAll);
                maxOfAll = Mathf.Max(minMaxData[i], maxOfAll);
            }

            minOfAll *= amplitudeScale;
            maxOfAll *= amplitudeScale;

            //チャンネル数・サンプル数
            int numChannels = clip.channels;
            int numSamples = (minMaxData == null) ? 0 : (minMaxData.Length / (2 * numChannels));
            //１チャンネルごとの専有height
            float h = rect.height / numChannels;

            //各チャンネルについて波形描画
            for (int channel = 0; channel < numChannels; channel++)
            {
                //描画範囲計算
                Rect channelRect = new Rect(rect.x, rect.y + h * channel, rect.width, h);

                //描画内容定義
                AudioCurveRendering.AudioMinMaxCurveAndColorEvaluator dlg =
                    delegate(float x, out Color col, out float minValue, out float maxValue)
                    {
                        if (numSamples <= 0)
                        {
                            minValue = 0.0f;
                            maxValue = 0.0f;
                        }
                        else
                        {
                            //minMaxDataの現在のx座標に対応する値を取得
                            float p = Mathf.Clamp(x * (numSamples - 2), 0.0f, numSamples - 2);
                            int i = (int)Mathf.Floor(p);
                            int offset1 = (i * numChannels + channel) * 2;
                            int offset2 = offset1 + numChannels * 2;

                            minValue = Mathf.Min(minMaxData[offset1 + 1], minMaxData[offset2 + 1]) * amplitudeScale;
                            maxValue = Mathf.Max(minMaxData[offset1 + 0], minMaxData[offset2 + 0]) * amplitudeScale;
                            
                            if (minValue > maxValue)
                            {
                                (minValue, maxValue) = (maxValue, minValue);
                            }
                        }

                        //色を指定
                        col = colorSetter?.Invoke(channel, x, minValue, maxValue, minOfAll, maxOfAll) ??
                              new Color(1, 0.54902f, 0, 1);
                    };

                //描画
                AudioCurveRendering.DrawMinMaxFilledCurve(channelRect, dlg);
            }
        }

//単色指定ver
        public static void RenderPreview(Rect rect, AudioClip clip, Color color, float amplitudeScale = 1)
        {
            RenderPreview(rect, clip, (_, __, ___, ____, _____, ______) => color, amplitudeScale);
        }

        
//時間経過でグラデーションver
        public static void RenderTimeAwarePreview(Rect rect, AudioClip clip, Color start, Color finish,
            float amplitudeScale = 1)
        {
            RenderPreview(rect, clip, (_, t, ___, ____, _____, ______) => Color.Lerp(start, finish, t), amplitudeScale);
        }

        
//振幅の大小でグラデーションver
        public static void RenderAmplitudeAwarePreview(Rect rect, AudioClip clip, Color lowAmp, Color highAmp,
            float amplitudeScale = 1)
        {
            RenderPreview(rect, clip, (channel, _, min, max, minOfAll, maxOfAll) =>
                Color.Lerp(lowAmp, highAmp, Mathf.Clamp01((max - min) / (maxOfAll - minOfAll))), amplitudeScale);
        }
    }
}