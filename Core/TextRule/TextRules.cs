using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace FGUFW
{
    public class TextRules
    {
        public char Rule;
        public bool Variate;
        public float Value;
        public List<TextRules> Children;


        public TextRules(object variateGet, ReadOnlySpan<char> text)
        {
            //去掉空字符
            text = text.Trim();

            //去掉首尾括号
            if (text[0] == TextRulesHelper.Code_DomainStart && text[text.Length - 1] == TextRulesHelper.Code_DomainEnd)
            {
                text = text.Slice(1, text.Length - 2);
            }

            if (TextRulesHelper.IsValueText(text))//纯数值
            {
                if (text[0] == TextRulesHelper.Code_Variate)//变量
                {
                    Variate = true;
                    Value = VariateGetUtility.GetVariateKey(variateGet, text.Slice(1).ToString());
                }
                else//常量
                {
                    Variate = false;
                    try
                    {
                        Value = text.ToFloat();
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError($"无法解析常量:{text.ToString()}");
                    }
                }
            }
            else if (TextRulesHelper.FindCodeOR(text, 0) != -1)// 或
            {
                Children = new List<TextRules>(2);
                int idx = TextRulesHelper.FindCodeOR(text, 0);
                Rule = text[idx];
                Children.Add(new TextRules(variateGet, text.Slice(0, idx)));
                Children.Add(new TextRules(variateGet, text.Slice(idx + 1)));
            }
            else if (TextRulesHelper.FindCodeAND(text, 0) != -1)// 且
            {
                Children = new List<TextRules>(2);
                int idx = TextRulesHelper.FindCodeAND(text, 0);
                Rule = text[idx];
                Children.Add(new TextRules(variateGet, text.Slice(0, idx)));
                Children.Add(new TextRules(variateGet, text.Slice(idx + 1)));
            }
            else if (TextRulesHelper.FindEqualCode(text, 0) != -1)//比较
            {
                Children = new List<TextRules>(2);
                int idx = TextRulesHelper.FindEqualCode(text, 0);
                Rule = text[idx];
                Children.Add(new TextRules(variateGet, text.Slice(0, idx)));
                Children.Add(new TextRules(variateGet, text.Slice(idx + 1)));
            }
            else if (TextRulesHelper.FindComputeL2Code(text, 0) != -1)//加减
            {
                Children = new List<TextRules>(2);
                int idx = TextRulesHelper.FindComputeL2Code(text, 0);
                Rule = text[idx];
                Children.Add(new TextRules(variateGet, text.Slice(0, idx)));
                Children.Add(new TextRules(variateGet, text.Slice(idx + 1)));
            }
            else if (TextRulesHelper.FindComputeL1Code(text, 0) != -1)//乘除取余
            {
                Children = new List<TextRules>(2);
                int idx = TextRulesHelper.FindComputeL1Code(text, 0);
                Rule = text[idx];
                Children.Add(new TextRules(variateGet, text.Slice(0, idx)));
                Children.Add(new TextRules(variateGet, text.Slice(idx + 1)));
            }
            else if (TextRulesHelper.FindInOutCode(text) != -1) //集合
            {
                Children = new List<TextRules>();
                int idx = TextRulesHelper.FindInOutCode(text);
                Rule = text[idx];
                Children.Add(new TextRules(variateGet, text.Slice(0, idx)));

                var rights = text.Slice(idx + 1).ToString().Split(TextRulesHelper.Code_Split);
                foreach (var item in rights)
                {
                    Children.Add(new TextRules(variateGet, item));
                }

            }
            else
            {
                throw new Exception($"unknown textRule:{text.ToString()}");
            }
            
        }


        public float GetValue(object variateGet)
        {
            float result = 0;
            switch (Rule)
            {
                case TextRulesHelper.Code_Value:
                    {
                        result = VariateGetUtility.GetValue(variateGet, Variate, Value);
                    }
                    break;
                //----------------------------------------------------
                case TextRulesHelper.Code_Or:
                    {
                        foreach (var child in Children)
                        {
                            if (child.GetValue(variateGet) == 1)
                            {
                                result = 1;
                                break;
                            }
                        }
                    }
                    break;
                case TextRulesHelper.Code_And:
                    {
                        result = 1;
                        foreach (var child in Children)
                        {
                            if (child.GetValue(variateGet) == 0)
                            {
                                result = 0;
                                break;
                            }
                        }
                    }
                    break;
                //----------------------------------------------------
                case TextRulesHelper.Code_In:
                    {
                        result = 0;
                        var left = Children[0].GetValue(variateGet);

                        int length = Children.Count;
                        for (int i = 1; i < length; i++)
                        {
                            var right = Children[i].GetValue(variateGet);
                            if (left == right)
                            {
                                result = 1;
                                break;
                            }
                        }
                    }
                    break;
                case TextRulesHelper.Code_Out:
                    {
                        result = 1;
                        var left = Children[0].GetValue(variateGet);

                        int length = Children.Count;
                        for (int i = 1; i < length; i++)
                        {
                            var right = Children[i].GetValue(variateGet);
                            if (left == right)
                            {
                                result = 0;
                                break;
                            }
                        }
                    }
                    break;
                //----------------------------------------------------
                case TextRulesHelper.Code_Equal:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left == right ? 1 : 0;
                    }
                    break;
                case TextRulesHelper.Code_NotEqual:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left != right ? 1 : 0;
                    }
                    break;
                case TextRulesHelper.Code_Greater:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left > right ? 1 : 0;
                    }
                    break;
                case TextRulesHelper.Code_Less:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left < right ? 1 : 0;
                    }
                    break;
                case TextRulesHelper.Code_GreaterAndEqual:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left >= right ? 1 : 0;
                    }
                    break;
                case TextRulesHelper.Code_LessAndEqual:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left <= right ? 1 : 0;
                    }
                    break;
                //----------------------------------------------------
                case TextRulesHelper.Code_Add:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left + right;
                    }
                    break;
                case TextRulesHelper.Code_Subtract:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left - right;
                    }
                    break;
                case TextRulesHelper.Code_Multiply:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left * right;
                    }
                    break;
                case TextRulesHelper.Code_Divide:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left / right;
                    }
                    break;
                case TextRulesHelper.Code_Remainder:
                    {
                        var left = Children[0].GetValue(variateGet);
                        var right = Children[1].GetValue(variateGet);
                        result = left % right;
                    }
                    break;

            }


            return result;
        }

    }
}