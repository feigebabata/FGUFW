using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FGUFW
{
    public static class TMPExtensions
    {
        public static void SetText(this TMP_Text self, object obj)
        {
            self.text = obj.ToString();
        }
    }
}