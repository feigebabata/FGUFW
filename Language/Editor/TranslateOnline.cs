#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace FGUFW.Language
{
/*
,简体中文,繁体中文,英文,日语,德语,西班牙语,葡萄牙语-巴西,韩语,法语,俄语
*,中文,繁体中文,English,にほん,Deutsch,Español,Português,한국어,Français,Русский
$,zh-cn,zh-hk,en-us,ja-jp,de-de,es-ar,pt-br,ko-kr,fr-fr,ru-ru
*/
    public static class TranslateOnline
    {
        static bool translating = false;

        [UnityEditor.MenuItem("多语言/在线翻译")]
        private static async void translateOnline()
        {
            if(translating)
            {
                Debug.LogError("任务进行中,请等待或查看Background Tasks");
                return;
            }
            translating = true;
            int progressId = Progress.Start("在线翻译多语言配置表");
            var filePath = Application.dataPath.Replace("Assets",Languages.CONFIG_PATH);

            string[][] data = CsvHelper.Parse(File.ReadAllText(filePath));
            var lineCount = data.Length;
            var itemCount = data[0].Length;

            int totalCount = (itemCount-2)*(lineCount-3);
            int currentIndex = 0;

            var sl = data[2][1];
            for (int i = 2; i < itemCount; i++)
            {
                var tl = data[2][i];
                for (int j = 3; j < lineCount; j++)
                {
                    currentIndex++;
                    Progress.Report(progressId, currentIndex / (float)totalCount,$"在线翻译多语言:{currentIndex}/{totalCount}");

                    if(!string.IsNullOrEmpty(data[j][i]))continue;
                    var q = data[j][1];
                    q = Uri.EscapeUriString(q);
                    var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&dt=t&sl={sl}&tl={tl}&q={q}";
                    UnityWebRequest uwr = new UnityWebRequest(url);

                    uwr.downloadHandler = new DownloadHandlerBuffer();
                    await uwr.RequestAsync();
                    // Debug.Log(uwr.downloadHandler.text);
                    
                    var newText = string.Empty;
                    try
                    {
                        var jsonData = JsonMapper.ToObject(uwr.downloadHandler.text);
                        foreach (JsonData item in jsonData[0])
                        {
                            newText = newText+item[0].ToString();
                        }

                    }
                    catch
                    {
                        Debug.LogError($"{q}\n{uwr.downloadHandler.text}\n{uwr.error}");
                    }

                    data[j][i] = newText;
                }
            }

            File.WriteAllText(filePath,CsvHelper.ToCsvText(data),new UTF8Encoding(true));
            translating = false;
            Progress.Remove(progressId);
        }

    }
}
#endif