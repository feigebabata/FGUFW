using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

namespace FGUFW.ExcelUtils
{
    public static class GenerateExcelCsharpCode
    {


        public static void GenerateCsharpCode(Excel excel, string path)
        {

            var configClass = new StringBuilder();

            var className = Path.GetFileNameWithoutExtension(path);
            var directory = Path.GetDirectoryName(path);

            foreach (var sheet in excel)
            {
                setConfigText(sheet, configClass);
            }

            string scriptText =
            @"using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FGUFW;
using System;

namespace ExcelConfig
{
    [Serializable]
    public class |CLASS_NAME|
    {

|CONFIG_CLASS|

    }
}
";

            scriptText = scriptText.Replace("|CLASS_NAME|", className);
            scriptText = scriptText.Replace("|CONFIG_CLASS|", configClass.ToString());
            var scriptPath = Path.Combine(directory, $"{className}.cs");
            File.WriteAllText(scriptPath, scriptText);

            // AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate);

            // AssetDatabase.Refresh();

        }

        static void setConfigText(ISheet sheet, StringBuilder configTexts)
        {
            var row = sheet.GetRow(0);
            if (row == default) return;

            //第一行第一列 校验标识
            if (row.GetCell(0)?.ToString() != "GenerateExcelCsharpCode") return;

            var collection = row.GetCell(1)?.ToString();
            if (collection.IsNull()) return;

            switch (collection)
            {
                case "List":
                    setConfigTextList(sheet, configTexts);
                    break;
                case "Table":
                    setConfigTextTable(sheet, configTexts);
                    break;
                case "Single":
                    setConfigTextSingle(sheet, configTexts);
                    break;
            }
        }

        static void setConfigTextList(ISheet sheet, StringBuilder configTexts)
        {

            var className = sheet.SheetName;
            string scriptText =
@"
        public List<|CLASS_NAME|> |CLASS_NAME|s;
        [Serializable]
        public class |CLASS_NAME|
        {
|MENBERS|
        }
";

            //第二行字段注释
            //第三行字段类型
            var types = sheet.GetRow(2);
            //第四行字段名
            var names = sheet.GetRow(3);
            int maxCellIdx = sheet.GetRow(1).LastCellNum;

            StringBuilder menbers = new StringBuilder();
            for (int i = 0; i < maxCellIdx; i++)
            {
                //过滤有效列 type列为空则忽略
                var tVar = types.GetCell(i);
                if (tVar == default) continue;

                menbers.AppendLine(
@$"            public {tVar} {names.GetCell(i)};"
);
            }

            var key = types.GetCell(0).ToString();

            scriptText = scriptText.Replace("|CLASS_NAME|", className);
            scriptText = scriptText.Replace("|KEY|", key);
            scriptText = scriptText.Replace("|MENBERS|", menbers.ToString());

            configTexts.AppendLine(scriptText);
        }

        static void setConfigTextTable(ISheet sheet, StringBuilder configTexts)
        {

            var className = sheet.SheetName;
            string scriptText =
@"
        public Table<|KEY|,|CLASS_NAME|> |CLASS_NAME|s;
        [Serializable]
        public class |CLASS_NAME|
        {
|MENBERS|
        }
";

            //第二行字段注释
            //第三行字段类型
            var types = sheet.GetRow(2);
            //第四行字段名
            var names = sheet.GetRow(3);
            int maxCellIdx = sheet.GetRow(1).LastCellNum;

            StringBuilder menbers = new StringBuilder();
            for (int i = 0; i < maxCellIdx; i++)
            {
                //过滤有效列 type列为空则忽略
                var tVar = types.GetCell(i);
                if (tVar == default) continue;

                menbers.AppendLine(
@$"            public {tVar} {names.GetCell(i)};"
);
            }

            var key = types.GetCell(0).ToString();

            scriptText = scriptText.Replace("|CLASS_NAME|", className);
            scriptText = scriptText.Replace("|KEY|", key);
            scriptText = scriptText.Replace("|MENBERS|", menbers.ToString());

            configTexts.AppendLine(scriptText);
        }

        static void setConfigTextSingle(ISheet sheet, StringBuilder configTexts)
        {

            var className = sheet.SheetName;
            string scriptText =
@"
        public |CLASS_NAME| |CLASS_NAME|Single;
        [Serializable]
        public class |CLASS_NAME|
        {
|MENBERS|
        }
";

            
            int maxRowNum = sheet.LastRowNum+1;

            StringBuilder menbers = new StringBuilder();
            for (int i = 2; i < maxRowNum; i++)
            {
                var row = sheet.GetRow(i);
                menbers.AppendLine(
@$"            public {row.GetCell(0)} {row.GetCell(1)};"
);
            }

            scriptText = scriptText.Replace("|CLASS_NAME|", className);
            scriptText = scriptText.Replace("|MENBERS|", menbers.ToString());

            configTexts.AppendLine(scriptText);
        }

    }
}