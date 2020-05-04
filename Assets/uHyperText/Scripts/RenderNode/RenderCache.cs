﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

namespace WXB
{
    // 缓存渲染元素
    public partial class RenderCache
    {
        Owner mOwner;

        public RenderCache(Owner st)
        {
            mOwner = st;
            materials = new List<Texture>();
        }

        public abstract class BaseData
        {
            Rect m_Rect;

            public Rect rect
            {
                set { m_Rect = value; }
                get { return m_Rect; }
            }

            public NodeBase node = null;

            public Line line { get; protected set; } // 当前所处的行

            public bool isContain(Vector2 pos)
            {
                var rect = new Rect(m_Rect);
                rect.y = rect.y + line.y - rect.height;

                return rect.Contains(pos);
            }

            public virtual bool isAlignByGeometry
            {
                get { return false; }
            }

            public abstract void Render(VertexHelper vh, Rect area, Vector2 offset, float pixelsPerUnit);

            public virtual void OnMouseEnter() { }
            public virtual void OnMouseLevel() { }
            public virtual void OnMouseUp(PointerEventData eventData) { }

            public int subMaterial { get; set; }

            public override string ToString()
            {
                return string.Format("rect:{0}", m_Rect);
            }

            public virtual void Release()
            {
                node = null;
                line = null;
                OnRelease();
            }

            protected abstract void OnRelease();

            public virtual void OnAlignByGeometry(ref Vector2 offset, float pixelsPerUnit)
            {

            }

            public virtual void OnLineYCheck(float pixelsPerUnit)
            {

            }

            protected LineAlignment lineAlignment
            {
                get
                {
                    return (node.lineAlignment == LineAlignment.Default ? node.owner.lineAlignment : node.lineAlignment);
                }
            }

            public virtual Vector2 GetStartLeftBottom(float unitsPerPixel)
            {
                if (line == null)
                {
                    return new Vector2(rect.x, rect.y + rect.height);
                }

                Vector2 leftBottomPos = new Vector2(rect.x, rect.y + rect.height);
                var la = lineAlignment;
                switch (la)
                {
                case LineAlignment.Top:
                    break;
                case LineAlignment.Center:
                    {
                        if (line.y == rect.height)
                        {

                        }
                        else
                        {
                            float offset = ((line.y - rect.height) * 0.5f);
                            leftBottomPos.y += offset;
                        }
                    }
                    break;

                case LineAlignment.Bottom:
                    leftBottomPos.y = rect.y + line.y;
                    break;
                }

                return leftBottomPos;
            }
        }

        List<BaseData> DataList = new List<BaseData>();

        public List<Texture> materials { get; protected set; }

        public void Release()
        {
            materials.Clear();

            BaseData bd = null;
            for (int i = 0; i < DataList.Count; ++i)
            {
                bd = DataList[i];
                bd.Release();
                if (bd is TextData)
                {
                    PoolData<TextData>.Free((TextData)bd);
                }
                else if (bd is SpriteData)
                {
                    PoolData<SpriteData>.Free((SpriteData)bd);
                }
                else if (bd is ISpriteData)
                {
                    PoolData<ISpriteData>.Free((ISpriteData)bd);
                }
            }

            DataList.Clear();
        }

        public void cacheText(Line l, TextNode n, string text, Rect rect)
        {
            TextData td = PoolData<TextData>.Get();
            td.Reset(n, text, rect, l);
            DataList.Add(td);

            td.subMaterial = materials.IndexOf(n.d_font.material.mainTexture);
            if (td.subMaterial == -1)
            {
                td.subMaterial = materials.Count;
                materials.Add(n.d_font.material.mainTexture);
            }
        }

        public void cacheISprite(Line l, NodeBase n, ISprite sprite, Rect rect)
        {
            var s = sprite.Get();
            if (s != null)
                cacheSprite(l, n, sprite, rect);
            else
            {
                ISpriteData cd = PoolData<ISpriteData>.Get();
                cd.Reset(n, sprite, rect, l);
                DataList.Add(cd);
            }
        }

        public void cacheSprite(Line l, NodeBase n, ISprite sprite, Rect rect)
        {
            if (sprite != null)
            {
                var s = sprite.Get();
                SpriteData sd = PoolData<SpriteData>.Get();
                sd.Reset(n, sprite, rect, l);
                DataList.Add(sd);

                sd.subMaterial = materials.IndexOf(s.texture);
                if (sd.subMaterial == -1)
                {
                    sd.subMaterial = materials.Count;
                    materials.Add(s.texture);
                }
            }
        }

        public void cacheCartoon(Line l, NodeBase n, Cartoon cartoon, Rect rect)
        {
            if (cartoon != null)
            {
                CartoonData cd = PoolData<CartoonData>.Get();
                cd.Reset(n, cartoon, rect, l);
                DataList.Add(cd);
            }
        }

        public bool isEmpty
        {
            get { return DataList.Count == 0; }
        }

        struct Key
        {
            public int subMaterial;
            public bool isBlink;
            public bool isOffset;
            public Rect offsetRect;

            public bool IsEquals(BaseData bd)
            {
                return subMaterial == bd.subMaterial &&
                       isBlink == bd.node.d_bBlink &&
                       (
                       (isOffset == false && bd.node.d_bOffset == false) ||
                       (isOffset == bd.node.d_bOffset && offsetRect == bd.node.d_rectOffset)
                       );
            }

            public List<BaseData> nodes;

            public DrawType drawType
            {
                get
                {
                    if (isBlink)
                    {
                        if (isOffset)
                            return DrawType.OffsetAndAlpha;

                        return DrawType.Alpha;
                    }

                    if (isOffset)
                    {
                        return DrawType.Offset;
                    }

                    return DrawType.Default;
                }
            }

            public Draw Get(Owner owner, Texture texture)
            {
                long key = nodes[0].node.keyPrefix;
                key += texture.GetInstanceID();
                Draw draw = owner.GetDraw(drawType, key,
                    (Draw d, object p) =>
                    {
                        d.texture = texture;
                        if (d is OffsetDraw)
                        {
                            OffsetDraw od = d as OffsetDraw;
                            od.Set((Rect)p);
                        }
                    }, offsetRect);

                return draw;
            }
        }

        static List<Key> s_keys = new List<Key>();

        Vector2 DrawOffset;
        public void OnAlignByGeometry(ref Vector2 offset, float pixelsPerUnit, float firstHeight)
        {
            for (int m = 0; m < DataList.Count; ++m)
            {
                if (DataList[m].rect.y > firstHeight)
                    continue;

                if (!DataList[m].isAlignByGeometry)
                {
                    offset = Vector2.zero;
                    return;
                }

                DataList[m].OnAlignByGeometry(ref offset, pixelsPerUnit);
            }
        }

        // 行修正
        public void OnCheckLineY(float pixelsPerUnit)
        {
            for (int m = 0; m < DataList.Count; ++m)
            {
                DataList[m].OnLineYCheck(pixelsPerUnit);
            }
        }

        public void Render(VertexHelper vh, Rect rect, Vector2 offset, float pixelsPerUnit, Mesh workerMesh, Material defaultMaterial)
        {
            DrawOffset = offset /** pixelsPerUnit*/;
            s_keys.Clear();
            for (int m = 0; m < DataList.Count; ++m)
            {
                BaseData bd = DataList[m];
                int index = s_keys.FindIndex((Key k) =>
                {
                    return k.IsEquals(bd);
                });

                if (index == -1)
                {
                    Key k = new Key();
                    k.subMaterial = bd.subMaterial;
                    k.isOffset = bd.node.d_bOffset;
                    k.isBlink = bd.node.d_bBlink;
                    k.offsetRect = bd.node.d_rectOffset;
                    k.nodes = ListPool<BaseData>.Get();
                    s_keys.Add(k);
                    k.nodes.Add(bd);
                }
                else
                {
                    s_keys[index].nodes.Add(bd);
                }
            }

            vh.Clear();
            for (int i = 0; i < s_keys.Count; ++i)
            {
                Key key = s_keys[i];
                for (int m = 0; m < key.nodes.Count; ++m)
                {
                    key.nodes[m].Render(vh, rect, offset, pixelsPerUnit);
                }

                if (vh.currentVertCount != 0)
                {
                    Draw draw = key.Get(mOwner, materials[key.subMaterial]);
                    vh.FillMesh(workerMesh);
                    draw.FillMesh(workerMesh);
                    vh.Clear();
                }

                ListPool<BaseData>.Release(key.nodes);
            }

            s_keys.Clear();
        }

        public BaseData Get(Vector2 pos)
        {
            BaseData bd = null;
            pos -= DrawOffset;
            for (int i = 0; i < DataList.Count; ++i)
            {
                bd = DataList[i];
                if (bd.isContain(pos))
                    return bd;
            }

            return null;
        }

        public void Get(List<BaseData> bds, NodeBase nb)
        {
            BaseData bd = null;
            for (int i = 0; i < DataList.Count; ++i)
            {
                bd = DataList[i];
                if (bd.node == nb)
                    bds.Add(bd);
            }
        }
    }
}