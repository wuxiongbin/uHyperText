using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    public partial class SymbolText
    {
        public override float preferredWidth
        {
            get
            {
                UpdateByDirty();
                return getNodeWidth() / pixelsPerUnit;
            }
        }

        public override float preferredHeight
        {
            get
            {
                UpdateByDirty();
                return getNodeHeight() / pixelsPerUnit;
            }
        }

        public override void SetAllDirty()
        {
            base.SetAllDirty();
            SetTextDirty();
        }

        void UpdateByDirty()
        {
            if (m_textDirty)
            {
                UpdateByTextDirty();
                m_textDirty = false;

                UpdateTextHeight();
                m_layoutDirty = false;

                UpdateRenderElement();
                m_renderNodeDirty = false;
            }

            if (m_layoutDirty)
            {
                UpdateTextHeight();
                m_layoutDirty = false;

                UpdateRenderElement();
                m_renderNodeDirty = false;
            }

            if (m_renderNodeDirty)
            {
                UpdateRenderElement();
                m_renderNodeDirty = false;
            }
        }

        public override void Rebuild(CanvasUpdate update)
        {
            if (canvasRenderer.cull)
                return;

            if (pixelsPerUnit >= 10f)
                return;

            switch (update)
            {
            case CanvasUpdate.PreRender:
                {
                    UpdateByDirty();

                    base.Rebuild(update);
                }
                break;
            }
        }

        protected virtual void SetTextDirty()
        {
            m_textDirty = true;
            m_renderNodeDirty = true;

            SetMaterialDirty();
            FreeDraws();

            if (isActiveAndEnabled)
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }

        protected override void UpdateMaterial()
        {
            if (!IsActive())
                return;

            if (m_UsedDraws.Count == 0)
            {
                return;
            }

            var components = ListPool<Component>.Get();
            GetComponents(typeof(IMaterialModifier), components);

            for (int i = 0; i < m_UsedDraws.Count; ++i)
            {
                Draw draw = m_UsedDraws[i];
                if (draw.srcMat == null)
                    draw.srcMat = material;

                Material currentMat = draw.srcMat;
                for (var m = 0; m < components.Count; m++)
                    currentMat = (components[m] as IMaterialModifier).GetModifiedMaterial(currentMat);

                draw.UpdateMaterial(currentMat);
            }

            ListPool<Component>.Release(components);
        }

        Around d_Around = new Around();

        public Around around { get { return d_Around; } }

        public ElementSegment elementSegment
        {
            get
            {
                if (string.IsNullOrEmpty(m_SegmentElement))
                    return null;

                return ESFactory.Get(m_SegmentElement);
            }
        }

        protected static List<NodeBase> s_nodebases = new List<NodeBase>();

        // 得到外部结点
        public System.Func<TagAttributes, IExternalNode> getExternalNode { get; set; }

        // 根据新文本，解析结点
        public void UpdateByTextDirty()
        {
            Clear();

            s_nodebases.Clear();
            Parser.parser(this, text, CreateConfig(), s_nodebases, getExternalNode);
            s_nodebases.ForEach((NodeBase nb) =>
            {
                mNodeList.AddLast(nb);
            });

            s_nodebases.Clear();
        }

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            SetMaterialDirty();
        }

        Vector2 last_size = new Vector2(-1000f, -1000f);

        protected override void OnRectTransformDimensionsChange()
        {
            if (gameObject.activeInHierarchy)
            {
                // prevent double dirtying...
                if (CanvasUpdateRegistry.IsRebuildingLayout())
                {
                    if (last_size == rectTransform.rect.size)
                        return;

                    SetLayoutDirty();
                }
                else
                {
                    if (last_size != rectTransform.rect.size)
                        SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        public override void SetLayoutDirty()
        {
            base.SetLayoutDirty();
            SetMaterialDirty();
            SetRenderDirty();
            m_layoutDirty = true;
        }

        public void UpdateTextHeight()
        {
            renderCache.Release();
            mLines.Clear();
            float width = rectTransform.rect.width;
            if (width <= 0f)
                width = 10f;
            
            d_Around.Clear();
            foreach (NodeBase node in mNodeList)
            {
                if (node is RectSpriteNode)
                {
                    RectSpriteNode rsn = node as RectSpriteNode;
                    d_Around.Add(rsn.rect);
                }
            }

            mLines.Add(new Line(Vector2.zero));
            Vector2 currentpos = Vector2.zero;
            float scale = pixelsPerUnit;
            foreach (NodeBase node in mNodeList)
                node.fill(ref currentpos, mLines, width, scale);

            for (int i = 0; i < mLines.Count; ++i)
            {
                mLines[i].y = Mathf.Max(mLines[i].y, m_MinLineHeight);
            }
        }

        // 更新渲染的文本
        void UpdateRenderElement()
        {
            if (pixelsPerUnit <= 0f)
                return;

            FreeDraws();
            renderCache.Release();
            Rect inputRect = rectTransform.rect;
            float w = inputRect.size.x/* * pixelsPerUnit*/;
            if (w <= 0f)
                return;

            float x = 0;
            uint yline = 0;
            LinkedListNode<NodeBase> itor = mNodeList.First;
            while (itor != null)
            {
                itor.Value.render(
                    w,
                    renderCache,
                    ref x,
                    ref yline,
                    mLines,
                    0f,
                    0f);

                itor = itor.Next;
            }
        }

        public void FontTextureChangedOther()
        {
            // Only invoke if we are not destroyed.
            if (!this)
            {
                return;
            }

            if (m_DisableFontTextureRebuiltCallback)
                return;

            if (!IsActive())
                return;

            // this is a bit hacky, but it is currently the
            // cleanest solution....
            // if we detect the font texture has changed and are in a rebuild loop
            // we just regenerate the verts for the new UV's
            if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
                UpdateGeometry();
            else
                SetAllDirty();
        }
    }
}
