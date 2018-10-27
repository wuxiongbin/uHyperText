﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    public partial class SymbolText : Text, Owner
    {
        List<Draw> m_UsedDraws = new List<Draw>();

        protected void FreeDraws()
        {
            m_UsedDraws.ForEach((Draw d) => 
            {
                if (d != null)
                {
                    DrawFactory.Free(d);
                }
            });

            m_UsedDraws.Clear();
        }

        // 通过纹理获取渲染对象
        public Draw GetDraw(DrawType type, long key, Action<Draw, object> oncreate, object p = null)
        {
            for (int i = 0; i < m_UsedDraws.Count; ++i)
            {
                Draw draw = m_UsedDraws[i];
                if (draw.type == type && draw.key == key)
                    return m_UsedDraws[i];
            }

            Draw dro = DrawFactory.Create(gameObject, type);
            dro.key = key;
            m_UsedDraws.Add(dro);

            oncreate(dro, p);

            return dro;
        }
    }
}
