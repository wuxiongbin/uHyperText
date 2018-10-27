﻿using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    [ExecuteInEditMode]
    public class OutlineDraw : EffectDrawObjec, ICanvasElement
    {
        public override DrawType type { get { return DrawType.Outline; } }

        public bool isOpenAlpha
        {
            get { return GetOpen(0); }
            set { SetOpen<AlphaEffect>(0, value); }
        }

        public override void UpdateSelf(float deltaTime)
        {
            base.UpdateSelf(deltaTime);

            if (currentWidth >= maxWidth || m_Data == null)
                return;

            float temp = currentWidth;
            for (int i = 0; i < m_Data.lines.Count; ++i)
            {
                DrawLineStruct.Line l = m_Data.lines[i];
                if (temp >= l.width)
                {
                    temp -= l.width;
                }
                else
                {
                    // 还未达到，使用这个速度来
                    float t = (l.width - temp) / l.dynSpeed;
                    if (t >= deltaTime)
                    {
                        // 所用的时间要大于当前间隔时间
                        currentWidth += deltaTime * l.dynSpeed;
                        break;
                    }
                    else
                    {
                        currentWidth += l.dynSpeed * t;
                        deltaTime -= t;
                        temp -= l.dynSpeed * t;
                    }
                }
            }

            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this); // 重绘
        }

        DrawLineStruct m_Data;

        float currentWidth = 0f;

        float maxWidth = 0f;

        public void AddLine(TextNode n, Vector2 left, float width, float height, Color color, Vector2 uv, int speed)
        {
            if (m_Data == null)
            {
                m_Data = new DrawLineStruct();
                maxWidth = 0f;
                currentWidth = 0f;
            }

            maxWidth += width;
            m_Data.lines.Add(new DrawLineStruct.Line() { leftPos = left, width = width, height = height, color = color, uv = uv, node = n, dynSpeed = speed });
        }

        public override void UpdateMaterial(Material mat)
        {
            base.UpdateMaterial(mat);
            rectTransform.SetAsLastSibling();
        }

        public void Rebuild(CanvasUpdate executing)
        {
            if (m_Data == null)
                return;

            if (executing != CanvasUpdate.PreRender)
                return;

            float width = currentWidth;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                width = maxWidth;
#endif
            VertexHelper vh = Tools.vertexHelper;
            vh.Clear();
            m_Data.Render(width, vh);

            Mesh workerMesh = SymbolText.WorkerMesh;
            vh.FillMesh(workerMesh);
            canvasRenderer.SetMesh(workerMesh);
        }

        public override void Release()
        {
            base.Release();
            m_Data = null;
        }

        public void GraphicUpdateComplete() { }
        public bool IsDestroyed() { return this == null; }
        public void LayoutComplete() { }
    }
}