using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FGUFW;

namespace FGUFW.Language
{
    
    public static class Languages
    {
        public const string CONFIG_PATH = "Assets/FGUFW/Language/LanguageConfig.csv";
        static private Dictionary<int, string[]> languageConfig;
        static private string[] languageNames,languageCodes;
        public static int LanguageIndex{get; private set;}
        public static Action OnLanguageChanged;

        static void tryInitConfig()
        {
            if(languageNames!=null)return;
            string csvText = AssetHelper.Load<TextAsset>(CONFIG_PATH).text;
            
            var table = CsvHelper.Parse2(csvText);

            var languageCount = table.GetLength(1)-1;
            languageNames = new string[languageCount];
            languageCodes = new string[languageCount];
            for (int i = 0; i < languageCount; i++)
            {
                languageNames[i] = table[1,i+1];
                languageCodes[i] = table[2,i+1];
            }

            var lineCount = table.GetLength(0);
            languageConfig = new Dictionary<int, string[]>(lineCount);

            for (int l_idx = 3; l_idx < lineCount; l_idx++)
            {
                var line = new string[languageCount];
                for (int i = 0; i < languageCount; i++)
                {
                    line[i] = table[l_idx,i+1];
                }
                languageConfig.Add(table[l_idx,0].ToInt32(),line);
            }
        }

        public static void Reload()
        {
            languageNames = null;
            tryInitConfig();
        }

        public static string[] GetLanguageNames()
        {
            tryInitConfig();
            return languageNames;
        }

        public static string[] GetLanguageCodes()
        {
            tryInitConfig();
            return languageCodes;
        }

        public static string GetLanguageText(int id)
        {
            tryInitConfig();
            string text = string.Empty;
            if(languageConfig.ContainsKey(id) && languageConfig[id].Length>LanguageIndex)
            {
                text = languageConfig[id][LanguageIndex];
            }
            else
            {
                text = id.ToString();
            }
            return text;
        }

        public static void SetLanguage(int index)
        {
            tryInitConfig();
            LanguageIndex = index;
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// 转当前语言
        /// </summary>
        /// <returns></returns>
        public static string ToLT(this int id)
        {
            return GetLanguageText(id);
        }
    }
}