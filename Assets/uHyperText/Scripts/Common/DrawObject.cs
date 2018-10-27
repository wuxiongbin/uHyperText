using UnityEngine;
using System;

namespace WXB
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class DrawObject : MonoBehaviour, Draw
    {
        protected virtual void OnTransformParentChanged()
        {
            if (!isActiveAndEnabled)
                return;

            UpdateRect(Vector2.zero);
        }

        protected virtual void OnDisable()
        {
            if (canvasRenderer == null)
                return;

            canvasRenderer.Clear();
        }

        protected void OnEnable()
        {
            UpdateRect(Vector2.zero);
        }

        public void OnInit()
        {
            enabled = true;
            UpdateRect(Vector2.zero);
        }

        protected void Start()
        {
            UpdateRect(Vector2.zero);
        }

        protected virtual void Init()
        {

        }

        protected void Awake()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
            rectTransform = GetComponent<RectTransform>();
            Init();
        }

        public RectTransform rectTransform { get; private set; }

        public virtual DrawType type { get { return DrawType.Default; } }
        public virtual long key { get; set; }

        public CanvasRenderer canvasRenderer
        {
            get;
            private set;
        }

        protected void UpdateRect(Vector2 offset)
        {
            Tools.UpdateRect(rectTransform, offset);
        }

        public virtual void UpdateSelf(float deltaTime)
        {

        }

        Material m_Material;
        Texture m_Texture;

        public Material srcMat { get { return m_Material; } set { m_Material = value; } }

        public Texture texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        public void FillMesh(Mesh workerMesh)
        {
            canvasRenderer.SetMesh(workerMesh);
        }

        public virtual void UpdateMaterial(Material mat)
        {
            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(mat, 0);
            canvasRenderer.SetTexture(m_Texture);
        }

        public virtual void Release()
        {
            m_Material = null;
            m_Texture = null;
            key = 0;
            if (canvasRenderer != null)
            {
                canvasRenderer.Clear();
            }
        }

        public void DestroySelf()
        {
            Tools.Destroy(gameObject);
        }        
    }
}