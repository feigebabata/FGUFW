using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace FGUFW.Console
{
    public static class ConsoleUtility
    {
        private const int MSG_LINE_MAX_LENGTH = 100;
        private const int MSG_LINE_OUTSIZE = 10;

        private static Dictionary<string,CommandData> commandDatas = new Dictionary<string, CommandData>();
        private static List<string> commandDataKeys;

        private static StringBuilder allMsg = new StringBuilder(MSG_LINE_MAX_LENGTH);
        private static List<string> msgLines = new List<string>();

        private static List<VariableData> variableDatas = new List<VariableData>();

        private static int lineIndex;

        public static Func<string,string> OnAddDefCommandMsg,OnAddSetCommandMsg,OnAddInvokeResultMsg,OnAddInvokeFailMsg;

        public static bool Initialized{get;private set;}


        [RuntimeInitializeOnLoadMethod]
        private static void onRuntimeInitialize()
        {
            Initialized = false;
            Task.Run(getAllCommand);
        }

        //避免在初始化完成前调用
        static void getAllCommand()
        {
            commandDatas.Clear();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!(type.IsClass || type.IsValueType)) continue;
                    
                    foreach (var methodInfo in type.GetMethods())
                    {
                        foreach (CommandAttribute att in methodInfo.GetCustomAttributes<CommandAttribute>())
                        {
                            var key = att.Alias;
                            if(string.IsNullOrEmpty(key))
                            {
                                key = $"{type.Name}.{methodInfo.Name}";
                            }
                            var commandData = new CommandData(key,att.Description,methodInfo,att.Target,type,methodInfo.IsStatic,att.Inactive);
                            if(!commandDatas.TryAdd(key,commandData))
                            {
                                throw new Exception($"FGUFW.Console 重复静态Command:{key}");
                            }
                        }
                    }
                    
                }
            }

            commandDataKeys = commandDatas.Keys.ToList();

            Initialized = true;
        }


        public static (InvokeCommandResult,object) InvokeCommand(string key,params object[] args)
        {
            CommandData commandData = default;
            if(!commandDatas.TryGetValue(key,out commandData))
            {
                return (InvokeCommandResult.NoneCommand,default);
            }
            
            if(commandData.IsStatic)
            {
                object result = default;
                InvokeCommandResult commandResult = default;
                try
                {
                    result = commandData.Method.Invoke(default,args);
                    commandResult = InvokeCommandResult.Completion;
                }
                catch (System.Exception ex)
                {
                    errorMsg(ex.Message);
                    result = default;
                    commandResult = InvokeCommandResult.InvokeError;
                }
                return (commandResult,result);
            }
            else
            {
                if(commandData.Target == MonoTargetType.First)
                {
                    var target = GameObject.FindFirstObjectByType(commandData.TargetType,commandData.Inactive);
                    if(target!=default)
                    {
                        object result = default;
                        InvokeCommandResult commandResult = default;
                        try
                        {
                            result = commandData.Method.Invoke(default,args);
                            commandResult = InvokeCommandResult.Completion;
                        }
                        catch (System.Exception ex)
                        {
                            errorMsg(ex.Message);
                            result = default;
                            commandResult = InvokeCommandResult.InvokeError;
                        }
                        return (commandResult,result);
                    }
                    else
                    {
                        return (InvokeCommandResult.NoneTarget,default);
                    }
                }
                else if(commandData.Target == MonoTargetType.All)
                {
                    var targets = GameObject.FindObjectsByType(commandData.TargetType,commandData.Inactive,FindObjectsSortMode.InstanceID);
                    if(targets?.Length>0)
                    {
                        List<object> results = new List<object>();
                        InvokeCommandResult commandResult = default;
                        try
                        {
                            foreach (var target in targets)
                            {
                                results.Add(commandData.Method.Invoke(target,args));
                            }
                            commandResult = InvokeCommandResult.Completion;
                        }
                        catch (System.Exception ex)
                        {
                            errorMsg(ex.Message);
                            commandResult = InvokeCommandResult.InvokeError;
                        }
                        return (commandResult,results);
                    }

                }

                return (InvokeCommandResult.NoneTarget,default);
            }

        }


        public static object Input2Arg<T>(object arg)
        {
            try
            {
                if(arg is string)
                {
                    var type = typeof(T);
                    var s = arg.ToString();
                    if(type == typeof(int))
                    {
                        return int.Parse(s);
                    }
                    else if(type == typeof(float))
                    {
                        return float.Parse(s);
                    }
                    else if(type == typeof(bool))
                    {
                        return bool.Parse(s);
                    }
                    else if(type.IsEnum)
                    {
                        return Enum.Parse(type,s);
                    }
                }

            }
            catch (System.Exception ex)
            {
                errorMsg(ex.Message);
            }

            return arg;

        }

        private static void errorMsg(string msg)
        {
            onInvokeFail(InvokeCommandResult.InvokeError,lineIndex++,msg);
        }

        [Command("all-command")]
        public static string AllCommand()
        {
            StringBuilder msg = new StringBuilder();
            foreach (var (k,v) in commandDatas)
            {
                msg.AppendLine($"   {v.Key} : {v.IsStatic} , {v.Description}");
            }
            return msg.ToString();
        }

        /// <summary>
        /// 命令补全
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (int,string) CommandSupplement(string cmd,int index=0)
        {
            index++;

            var length = commandDataKeys.Count;
            for (int i = 0; i < length; i++)
            {
                var idx = (index+i)%length;
                var key = commandDataKeys[idx];

                if(key.StartsWith(cmd))
                {
                    index = idx;
                    cmd = key;
                    break;
                }

            }
            return (index,cmd);
        }

        public static void ParseCommand(string cmdLine)
        {
            MsgLineData msgData = new MsgLineData();

            var setVar = cmdLine.Split('=');
            if(setVar.Length==1)
            {
                var args = setVar[0].Split(' ');
                var cmd = args[0];
                var objs = new object[args.Length-1];

                for (int i = 1; i < args.Length; i++)
                {
                    var key = args[i];
                    var variableData = variableDatas.Find(vd=>vd.Key==key);
                    if(variableData==default)
                    {
                        objs[i-1] = key;
                    }
                    else
                    {
                        objs[i-1] = variableData.Obj;
                    }
                }

                addCmdLine(cmdLine,lineIndex++);

                var (resultState,varObj) = InvokeCommand(cmd,objs);
                switch (resultState)
                {
                    case InvokeCommandResult.Completion:
                    {
                        onAddResultMsgLine(varObj,lineIndex++);
                    }
                    break;
                    default:
                    {
                        onInvokeFail(resultState,lineIndex++);
                    }
                    break;
                }

            }
            else if(setVar.Length==2 && !setVar[0].Contains(' '))
            {
                var varKey = setVar[0];
                var args = setVar[1].Split(' ');
                var cmd = args[0];
                var objs = new object[args.Length-1];

                for (int i = 1; i < args.Length; i++)
                {
                    var key = args[i];
                    var variableData = variableDatas.Find(vd=>vd.Key==key);
                    if(variableData==default)
                    {
                        objs[i-1] = key;
                    }
                    else
                    {
                        objs[i-1] = variableData.Obj;
                    }
                }

                addSetVarCmdLine(cmdLine,lineIndex++);

                var (resultState,varObj) = InvokeCommand(cmd,objs);
                switch (resultState)
                {
                    case InvokeCommandResult.Completion:
                    {
                        var variableData = variableDatas.Find(vd=>vd.Key==varKey);
                        if(variableData==default)
                        {
                            variableDatas.Add(new VariableData{Index=lineIndex,Obj=varObj,Key=varKey});
                        }
                        else
                        {
                            variableData.Index = lineIndex;
                            variableData.Obj = varObj;
                        }
                        onAddResultMsgLine(varObj,lineIndex++);
                    }
                    break;
                    default:
                    {
                        onInvokeFail(resultState,lineIndex++);
                    }
                    break;
                }
            }
            else
            {
                onInvokeFail(InvokeCommandResult.NoneCommand,lineIndex++);
            }
        }

        public static string ConloseAllMsg
        {
            get
            {
                allMsg.Clear();
                msgLines.ForEach(m=>allMsg.AppendLine(m));
                return allMsg.ToString();
            }
        }

        private static void addCmdLine(string cmdLine, int index)
        {
            var msg = OnAddDefCommandMsg?.Invoke(cmdLine);

            msgLines.Add(msg);
            checkMsgLineSize();
        }

        private static void onAddResultMsgLine(object varObj, int index)
        {
            var msg = OnAddInvokeResultMsg?.Invoke(varObj?.ToString());

            msgLines.Add(msg);
            checkMsgLineSize();
        }

        private static void onInvokeFail(InvokeCommandResult resultState,int index,string msg = "")
        {

            string outMsg = default;
            switch (resultState)
            {
                case InvokeCommandResult.NoneCommand:
                    outMsg = "找不到指令;";
                break;
                case InvokeCommandResult.InvokeError:
                    outMsg = $"执行命令失败:{msg}";
                break;
                case InvokeCommandResult.NoneTarget:
                    outMsg = "找不到对象;";
                break;
            }
            var line = OnAddInvokeFailMsg?.Invoke(outMsg);

            msgLines.Add(line);
            checkMsgLineSize();
        }

        private static void addSetVarCmdLine(string cmdLine, int index)
        {
            var msg = OnAddSetCommandMsg?.Invoke(cmdLine);

            msgLines.Add(msg);
            checkMsgLineSize();
        }

        private static void checkMsgLineSize()
        {
            if(msgLines.Count < MSG_LINE_MAX_LENGTH)return;

            int outLineIdx = lineIndex - msgLines.Count+MSG_LINE_OUTSIZE;
            variableDatas.RemoveAll(vb=>vb.Index<outLineIdx);
            
            msgLines.RemoveRange(0,MSG_LINE_OUTSIZE);
        }

        public class MsgLineData
        {
            public int Index;
            public MsgLineType LineType;
            public string Msg;
        }

        public class VariableData
        {
            public int Index;
            public string Key;
            public object Obj;
        }

        public enum MsgLineType
        {
            Result,
            Command,
            SetVariable,
            ErrorText,
            
        }
        
    }
}