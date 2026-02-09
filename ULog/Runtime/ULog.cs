using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace FGUFW
{
    public static class ULog
    {
        /// <summary>
        /// 开启日志写入
        /// </summary>
        public const string Conditional_Log = "ULog";
        public const string Conditional_IgnoreLog = "ULog_IgnoreLogWrite";
        
        public const int MAX_LOG_FILE_COUNT = 32;
        public const string LOG_BEGIN = "---LogBegin";
        public const string LOG_END = "---LogEnd";
        public const string LOG_SPLIT = "--- --- ---";

        public static string SavePath{get;private set;}
        public static string LogFileName{get;private set;}
        private static StreamWriter _logFile;
        private static StringBuilder _msgBuilder = new StringBuilder();


        // [System.Diagnostics.Conditional(Conditional_Log)]
        // public static void log(this MonoBehaviour mb,object obj)
        // {
        //     Debug.Log(obj);
        // }

        // [System.Diagnostics.Conditional(Conditional_Log)]
        // public static void logWarning(this MonoBehaviour mb,object obj)
        // {
        //     Debug.LogWarning(obj);
        // }

        // [System.Diagnostics.Conditional(Conditional_Log)]
        // public static void logError(this MonoBehaviour mb,object obj)
        // {
        //     Debug.LogError(obj);
            
        // }

        // /// <summary>
        // /// 需要宏 UNITY_ASSERTIONS 开启
        // /// </summary>
        // /// <param name="mb"></param>
        // /// <param name="b"></param>
        // /// <param name="msg"></param>
        // [System.Diagnostics.Conditional(Conditional_Log)]
        // public static void assert(this MonoBehaviour mb,bool b,string msg)
        // {
        //     Assert.IsTrue(b,msg);
        // }

        [System.Diagnostics.Conditional(Conditional_Log)]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void runtimeInit()
        {
            SavePath = $"{Application.persistentDataPath}/Logs";
            if(!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            clearLogFile();

            LogFileName = getLogFileName();

            var filePath = Path.Combine(SavePath,LogFileName);

            Application.logMessageReceivedThreaded -= onLogReceive;
            Application.logMessageReceivedThreaded += onLogReceive;

            Application.quitting -= onAppQuit;
            Application.quitting += onAppQuit;

            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            _logFile = File.CreateText(filePath);
            
            _logFile.WriteLine($"{LOG_BEGIN} {DateTime.Now.SecondTickName()}\n");
        }

        //崩溃 未捕获异常
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _msgBuilder.Clear();

            var exception = (Exception)e.ExceptionObject;

            _msgBuilder.AppendLine($"Crash {DateTime.Now.SecondTickName()}.{DateTime.Now.Millisecond}");
            _msgBuilder.AppendLine();
            _msgBuilder.AppendLine(exception.Message);
            _msgBuilder.AppendLine();
            _msgBuilder.AppendLine(exception.StackTrace);


            File.WriteAllText(Path.Combine(SavePath, $"Crash{LogFileName}"),_msgBuilder.ts());

        }

        private static void clearLogFile()
        {
            var filePaths = Directory.GetFiles(SavePath);

            if(filePaths==default || filePaths.Length<MAX_LOG_FILE_COUNT)return;
            
            Array.Sort<string>(filePaths,(l,r)=>
            {
                return (int)(long.Parse(Path.GetFileNameWithoutExtension(l)) - long.Parse(Path.GetFileNameWithoutExtension(r)));
            });

            var length = filePaths.Length-MAX_LOG_FILE_COUNT;
            for (int i = 0; i < length; i++)
            {
                File.Delete(filePaths[i]);
            }
        }


        private static void onAppQuit()
        {
            _logFile.WriteLine($"{LOG_END} {DateTime.Now.SecondTickName()}");

            _logFile.Flush();
            _logFile.Close();

            Application.logMessageReceivedThreaded -= onLogReceive;
            Application.quitting -= onAppQuit;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        }

        private static string getLogFileName()
        {
            return $"{DateTime.Now.SecondTickName()}.txt";
        }

        private static void onLogReceive(string condition, string stackTrace, LogType type)
        {
            
#if ULog_IgnoreLogWrite
            if( type== LogType.Log)return; //忽略普通日志
#endif

            _msgBuilder.Clear();

            _msgBuilder.AppendLine(condition);
            _msgBuilder.AppendLine();
            _msgBuilder.AppendLine($"{type} {DateTime.Now.SecondTickName()}.{DateTime.Now.Millisecond}");
            _msgBuilder.AppendLine();
            _msgBuilder.AppendLine(stackTrace);
            _msgBuilder.AppendLine(LOG_SPLIT);

            _logFile.WriteLine(_msgBuilder);
        }
    }
    
}

// public static partial class FGUsing
// {
//     [System.Diagnostics.Conditional(FGUFW.ULog.Conditional_Log)]
//     public static void log(object obj)
//     {
//         Debug.Log(obj);
//     }

//     [System.Diagnostics.Conditional(FGUFW.ULog.Conditional_Log)]
//     public static void logWarning(object obj)
//     {
//         Debug.LogWarning(obj);
//     }

//     [System.Diagnostics.Conditional(FGUFW.ULog.Conditional_Log)]
//     public static void logError(object obj)
//     {
//         Debug.LogError(obj);
        
//     }

//     /// <summary>
//     /// 需要宏 UNITY_ASSERTIONS 开启
//     /// </summary>
//     /// <param name="mb"></param>
//     /// <param name="b"></param>
//     /// <param name="msg"></param>
//     [System.Diagnostics.Conditional(FGUFW.ULog.Conditional_Log)]
//     public static void assert(this MonoBehaviour mb,bool b,string msg)
//     {
//         Assert.IsTrue(b,msg);
//     }
// }