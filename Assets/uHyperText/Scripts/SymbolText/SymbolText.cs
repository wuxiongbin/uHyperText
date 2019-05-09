﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/SymbolText")]
    public partial class SymbolText : Text, Owner
    {
        static SymbolText()
        {
            Font.textureRebuilt += RebuildForFont;
        }

        public static Mesh WorkerMesh { get { return workerMesh; } }

        static void RebuildForFont(Font f)
        {
            for (int i = 0; i < ActiveList.Count; ++i)
            {
                if (ActiveList[i].font == f)
                    continue;

                if (ActiveList[i].isUsedFont(f))
                    ActiveList[i].FontTextureChangedOther();
            }
        }

        static public List<SymbolText> ActiveList = new List<SymbolText>();

        static TextParser sTextParser = new TextParser();

        protected TextParser Parser { get { return sTextParser; } }

        // 解析出来的结点
        [NonSerialized]
        protected LinkedList<NodeBase> mNodeList = new LinkedList<NodeBase>();

        [SerializeField]
        string m_ElementSegment = "Default"; // 分割类型

        protected bool m_textDirty = false; // 文字内容变化了，需要重新解析下结点
        protected bool m_renderNodeDirty = false; // 渲染结点的内容变化了，需要重新计算下
        protected bool m_layoutDirty = false; // 大小发生变化

        // 是否有使用此字体
        public bool isUsedFont(Font f)
        {
            foreach (NodeBase nb in mNodeList)
            {
                if (nb is TextNode && ((TextNode)nb).d_font == f)
                    return true;
            }

            return false;
        }

        public void SetRenderDirty()
        {
            FreeDraws();
            SetVerticesDirty();
            SetMaterialDirty();
        }

        public override string text
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                        return;

                    m_Text = "";
                    SetVerticesDirty();
                    SetTextDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    SetVerticesDirty();
                    SetLayoutDirty();
                    SetTextDirty();
                }
            }
        }

        public Anchor anchor
        {
            get { return (Anchor)alignment; }
        }

        static void FreeNode(NodeBase node)
        {
            if (node == null)
            {
                Debug.LogErrorFormat("FreeNode error! node == null");
                return;
            }

            node.Release();
            node = null;
        }

        public void Clear()
        {
            foreach (NodeBase node in mNodeList)
                FreeNode(node);
            
            mNodeList.Clear();
            FreeDraws();
        }

        [SerializeField]
        int m_MinLineHeight = 10;

        // 最小行高
        public int minLineHeight
        {
            get { return m_MinLineHeight; }
            set
            {
                if (m_MinLineHeight != value)
                {
                    m_MinLineHeight = value;
                    SetAllDirty();
                }
            }
        }

        [SerializeField]
        LineAlignment m_LineAlignment = LineAlignment.Bottom;

        // 最小行高
        public LineAlignment lineAlignment
        {
            get { return m_LineAlignment; }
            set
            {
                if (m_LineAlignment != value)
                {
                    m_LineAlignment = value;
                    SetAllDirty();
                }
            }
        }

        protected TextParser.Config CreateConfig()
        {
            TextParser.Config config = new TextParser.Config();
            config.anchor = Anchor.Null;
            config.font = font;
            config.fontStyle = fontStyle;
            config.fontSize = fontSize;
            config.fontColor = color;

            config.isBlink = false;
            config.isStrickout = false;
            config.isUnderline = false;
            config.dyncSpeed = Tools.s_dyn_default_speed;

            config.lineAlignment = lineAlignment;

            BaseMeshEffect effect = GetComponent<BaseMeshEffect>();
            if (effect != null)
            {
                if (effect is Outline)
                {
                    Outline outline = effect as Outline;
                    config.effectType = EffectType.Outline;
                    config.effectColor = outline.effectColor;
                    config.effectDistance = outline.effectDistance;
                }
                else if (effect is Shadow)
                {
                    Shadow shadow = effect as Shadow;
                    config.effectType = EffectType.Shadow;
                    config.effectColor = shadow.effectColor;
                    config.effectDistance = shadow.effectDistance;
                }
                else
                {
                    config.effectType = EffectType.Null;
                }
            }
            else
            {
                config.effectType = EffectType.Null;
            }

            return config;
        }

        RenderCache mRenderCache; // 渲染缓存

        public RenderCache renderCache
        {
            get 
            { 
                if (mRenderCache == null) 
                    mRenderCache = new RenderCache(this);
                return mRenderCache;
            }
        }

        List<Line> mLines = new List<Line>(); // 每一行的大小

        protected override void Awake()
        {
            mRenderCache = new RenderCache(this);
            base.Awake();

            m_textDirty = true;
            m_renderNodeDirty = true;

            DestroyDrawChild();
        }

        public void DestroyDrawChild()
        {
            int childCount = rectTransform.childCount;

            var components = ListPool<Component>.Get();
            Draw d;
            for (int i = 0; i < childCount; ++i)
            {
                if ((d = (rectTransform.GetChild(i)).GetComponent<Draw>()) != null)
                {
                    components.Add(d as Component);
                }
            }

            for (int i = 0; i < components.Count; ++i)
            {
                Tools.Destroy(components[i].gameObject);
            }

            ListPool<Component>.Release(components);
        }

        [SerializeField]
        bool m_isCheckFontY = false;

        public bool isCheckFontY
        {
            get { return m_isCheckFontY; }
            set
            {
                if (m_isCheckFontY == value)
                    return;

                m_isCheckFontY = value;
                SetVerticesDirty();
            }
        }

        protected override void UpdateGeometry()
        {
            VertexHelper vh = Tools.vertexHelper;
            if (rectTransform != null && rectTransform.rect.width >= 0 && rectTransform.rect.height >= 0 && !renderCache.isEmpty)
            {
                Rect inputRect = rectTransform.rect;
                Vector2 offset = new Vector2(-rectTransform.pivot.x * inputRect.width, rectTransform.pivot.y * inputRect.height);
                //inputRect.position += offset;

                float nodeHeight = getNodeHeight();
                //float nodeWidth = getNodeWidth();
                Vector2 refPoint = Vector2.zero;
                switch (alignment)
                {
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperRight: // 顶对齐
                    break;

                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleRight:
                    refPoint.y = (inputRect.height - nodeHeight) / 2f;
                    break;

                default:
                    refPoint.y = (inputRect.height - nodeHeight);
                    break;
                }

                refPoint.x += offset.x;
                refPoint.y += offset.y;

                vh.Clear();

                float scaleSize = pixelsPerUnit;
                m_DisableFontTextureRebuiltCallback = true;

                if (alignByGeometry)
                {
                    offset = Vector2.zero;
                    mRenderCache.OnAlignByGeometry(ref offset, scaleSize, mLines[0].y);
                    refPoint -= offset;
                }

                for (int i = 0; i < mLines.Count; ++i)
                {
                    mLines[i].minY = float.MaxValue;
                    mLines[i].maxY = float.MinValue;
                }

                mRenderCache.OnCheckLineY(scaleSize);

                mRenderCache.Render(vh, inputRect, refPoint, scaleSize, workerMesh, material);

                m_DisableFontTextureRebuiltCallback = false;

                last_size = rectTransform.rect.size;
            }
            else
            {
                vh.Clear(); // clear the vertex helper so invalid graphics dont draw.
                canvasRenderer.Clear();

                last_size = Vector2.zero;
            }
        }

        public float getNodeHeight()
        {
            float height = 0;
            for (int i = 0; i < mLines.Count; ++i)
                height += mLines[i].y;
            return height;
        }

        public float getNodeWidth()
        {
            float width = 0;
            float maxWidth = 0;

            NodeBase node;
            LinkedListNode<NodeBase> itor = mNodeList.First;
            while (itor != null)
            {
                node = itor.Value;
                itor = itor.Next;

                width += node.getWidth();
                if (node.isNewLine())
                {
                    if (maxWidth < width)
                        maxWidth = width;
                    width = 0;
                }
                node = null;
            }

            if (maxWidth < width)
                maxWidth = width;

            return maxWidth;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ActiveList.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            FreeDraws();
            ActiveList.Remove(this);
        }

        protected override void OnDestroy()
        {
            if (mRenderCache != null)
                mRenderCache.Release();

            FreeDraws();
            base.OnDestroy();
            Clear();
        }

        protected void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < m_UsedDraws.Count; ++i)
                m_UsedDraws[i].UpdateSelf(deltaTime);
        }
    }
}
