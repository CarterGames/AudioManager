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
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
    public static class InternalAudioUtil
    {
        enum Method
        {
            PlayClip,
            StopClip,
            PauseClip,
            ResumeClip,
            LoopClip,
            IsClipPlaying,
            StopAllClips,
            GetClipPosition,
            GetClipSamplePosition,
            SetClipSamplePosition,
            GetSampleCount,
            GetChannelCount,
            GetBitRate,
            GetBitsPerSample,
            GetFrequency,
            GetSoundSize,
            GetSoundCompressionFormat,
            GetTargetPlatformSoundCompressionFormat,
            GetAmbisonicDecoderPluginNames,
            HasPreview,
            GetImporterFromClip,
            GetMinMaxData,
            GetDuration,
            GetFMODMemoryAllocated,
            GetFMODCPUUsage,
            IsTrackerFile,
            GetMusicChannelCount,
            GetLowpassCurve,
            GetListenerPos,
            UpdateAudio,
            SetListenerTransform,
            HasAudioCallback,
            GetCustomFilterChannelCount,
            GetCustomFilterProcessTime,
            GetCustomFilterMaxIn,
            GetCustomFilterMaxOut,
        }


        //AudioUtil型
        static readonly Type tAudioUtil = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil");
        //コンパイル済みメソッドのキャッシュ
        static readonly ConcurrentDictionary<Method, Func<object[], object>> 
            compiled = new ConcurrentDictionary<Method, Func<object[], object>>();

        //キャッシュからメソッドを取得する。コンパイル済みでなければコンパイルしてキャッシュし、それを返す。
        static Func<object[], object> GetOrCompile(Method method)
        {
            return compiled.GetOrAdd(method, _m =>
            {
                //キャッシュが存在しなければここに来る

                //MethodInfo取得
                var m = tAudioUtil.GetMethod(_m.ToString(), BindingFlags.Static | BindingFlags.Public);

                //voidメソッドのためのreturn先ラベルを定義
                var voidTarget = Expression.Label(typeof(object));

                //引数はobject[]
                var args = Expression.Parameter(typeof(object[]), "args");
                //MethodInfoのパラメータの型に引数をキャストするExpressionの束
                var parameters = m.GetParameters()
                    .Select((x, index) =>
                        Expression.Convert(
                            Expression.ArrayIndex(args, Expression.Constant(index)),
                            x.ParameterType))
                    .ToArray();
                //式木構築
                var lambda = Expression.Lambda<Func<object[], object>>(
                    m.ReturnType == typeof(void)
                        //voidメソッドの場合、ブロックにしてreturn default(object)する必要がある
                        ? (Expression)Expression.Block(
                            Expression.Call(null, m, parameters),
                            Expression.Return(voidTarget, Expression.Default(typeof(object))),
                            Expression.Label(voidTarget, Expression.Constant(null))
                        )
                        //返り値がある場合はCallして結果をobjectにキャストするだけ
                        : Expression.Convert(
                            Expression.Call(null, m, parameters),
                            typeof(object)),
                    args);

                //コンパイルしてキャッシュしつつ返す
                return lambda.Compile();
            });
        }
        
        
        
        static TRet Call<TRet>(Method method) 
            => (TRet)GetOrCompile(method).Invoke(null);
        static TRet Call<T0, TRet>(Method method, T0 arg0) 
            => (TRet)GetOrCompile(method).Invoke(new object[] { arg0 });
        static TRet Call<T0, T1, TRet>(Method method, T0 arg0, T1 arg1)
            => (TRet)GetOrCompile(method).Invoke(new object[] { arg0, arg1 });
        static TRet Call<T0, T1, T2, TRet>(Method method, T0 arg0, T1 arg1, T2 arg2)
            => (TRet)GetOrCompile(method).Invoke(new object[] { arg0, arg1, arg2 });
        static TRet Call<T0, T1, T2, T3, TRet>(Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
            => (TRet)GetOrCompile(method).Invoke(new object[] { arg0, arg1, arg2, arg3 });

        static void Call(Method method)
            => GetOrCompile(method).Invoke(null);
        static void Call<T0>(Method method, T0 arg0) 
            => GetOrCompile(method).Invoke(new object[] { arg0 });  
        static void Call<T0, T1>(Method method, T0 arg0, T1 arg1)
            => GetOrCompile(method).Invoke(new object[] { arg0, arg1 });
        static void Call<T0, T1, T2>(Method method, T0 arg0, T1 arg1, T2 arg2)
            => GetOrCompile(method).Invoke(new object[] { arg0, arg1, arg2 });
        static void Call<T0, T1, T2, T3>(Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
            => GetOrCompile(method).Invoke(new object[] { arg0, arg1, arg2, arg3 });
        
        
        public static AudioImporter GetImporterFromClip(AudioClip clip) => Call<AudioClip, AudioImporter>(Method.GetImporterFromClip, clip);
        public static float[] GetMinMaxData(AudioImporter importer) => Call<AudioImporter, float[]>(Method.GetMinMaxData, importer);
    }
}