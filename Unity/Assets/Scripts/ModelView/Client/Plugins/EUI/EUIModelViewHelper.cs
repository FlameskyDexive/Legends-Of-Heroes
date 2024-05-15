using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    public static class EUIModelViewHelper
    {

        public static T BindTrans<T>(this EntityRef<T> self, Transform transform) where T : Entity, IAwake, IUIScrollItem<T>
        {
            T value = self;
            return value.BindTrans(transform);
        }
        
        public static void AddUIScrollItems<K,T>(this K self, ref Dictionary<int, EntityRef<T>> dictionary, int count) where K : Entity,IUILogic  where T : Entity,IAwake,IUIScrollItem<T>
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, EntityRef<T>>();
            }
            
            if (count <= 0)
            {
                return;
            }
            
            foreach (var item in dictionary)
            {
                T t = item.Value;
                t.Dispose();
            }
            dictionary.Clear();
            for (int i = 0; i <= count; i++)
            {
                T itemServer = self.AddChild<T>(true);
                dictionary.Add(i , itemServer);
            }
        }
    }
}