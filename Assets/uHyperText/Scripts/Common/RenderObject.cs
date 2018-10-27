using UnityEngine;
using System;

namespace WXB
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class RenderObject : MonoBehaviour
    {
        protected virtual void OnTransformParentChanged()
        {
            if (isActiveAndEnabled)
                return;

            UpdateRect();
        }

        protected void OnDisable()
        {
            if (m_CanvasRender == null)
                return;

            m_CanvasRender.Clear();
        }

        protected void Start()
        {
            UpdateRect();
        }

        RectTransform rect;

        [NonSerialized]
        private CanvasRenderer m_CanvasRender;

        public CanvasRenderer canvasRenderer
        {
            get
            {
                if (m_CanvasRender == null)
                    m_CanvasRender = GetComponent<CanvasRenderer>();
                return m_CanvasRender;
            }
        }

        void UpdateRect()
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();

            RectTransform parent = rect.parent as RectTransform;
            if (parent == null)
                return;

            rect.pivot = parent.pivot;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }

        public void FillMesh(Mesh workerMesh)
        {
            canvasRenderer.SetMesh(workerMesh);
        }

        public void UpdateMaterial(Material mat, Texture texture)
        {
            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(mat, 0);
            canvasRenderer.SetTexture(texture);
        }
    }
}