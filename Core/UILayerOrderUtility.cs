using System.Collections.Generic;
using UnityEngine;

namespace FGUFW
{
    /// <summary>
    /// 无需设定Order 只需设定Layer
    /// 同级根据弹出时间 后出现的更优先 Order自动设定
    /// 强覆盖的设为不同Layer
    /// </summary>
    public static class UILayerOrderUtility
    {
        private static Dictionary<string,int> layerOrders = new();

        public static void OrderIssue(this Canvas canvas)
        {
            var layer = canvas.sortingLayerName;
            int order;

            if(!layerOrders.TryGetValue(layer,out order))
            {
                layerOrders.Add(layer,order);
            }

            order++;
            layerOrders[layer] = order;

            canvas.sortingOrder = order;
        }

        public static void OrderRecycle(this Canvas canvas)
        {
            var layer = canvas.sortingLayerName;
            int order;

            if(!layerOrders.TryGetValue(layer,out order))
            {
                layerOrders.Add(layer,order);
            }

            order--;
            layerOrders[layer] = order;

            canvas.sortingOrder = order;
        }
    }
}