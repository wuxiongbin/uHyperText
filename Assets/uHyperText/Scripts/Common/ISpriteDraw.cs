using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WXB
{
    class ISpriteData : SpriteData
    {
        public ISprite sprite;
        public bool renderer = false;
    }

    [ExecuteInEditMode]
    public class ISpriteDraw : EffectDrawObjec, ICanvasElement
    {
        public override DrawType type { get { return DrawType.ISprite; } }

        public bool isOpenAlpha
        {
            get { return GetOpen(0); }
            set { SetOpen<AlphaEffect>(0, value); }
        }

        public bool isOpenOffset
        {
            get { return GetOpen(1); }
            set { SetOpen<OffsetEffect>(1, value); }
        }

        List<ISpriteData> mData = new List<ISpriteData>();

        public void Add(ISprite sprite, Vector2 leftPos, float width, float height, Color color)
        {
            sprite.AddRef();
            mData.Add(new ISpriteData() { sprite = sprite, leftPos = leftPos, color = color, width = width, height = height });
            isRebuild = true;
        }

        bool isRebuild = false;
        bool isCurrentEmpty = false;

        public override void UpdateSelf(float deltaTime)
        {
            base.UpdateSelf(deltaTime);
            if (isNeedRebuild())
            {
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
        }
       
        bool isNeedRebuild()
        {
            if (isRebuild)
            {
                isRebuild = false;
                return true;
            }

            if (!isCurrentEmpty)
                return false;

            ISpriteData sd;
            for (int i = 0; i < mData.Count; ++i)
            {
                sd = mData[i];
                if (sd.renderer || (sd.sprite.Get() == null))
                    continue;
                return true;
            }
            
            return false;
        }

        public void Rebuild(CanvasUpdate executing)
        {
            if (executing != CanvasUpdate.PreRender)
                return;

            isCurrentEmpty = false;
            if (mData == null || mData.Count == 0)
                return;

            VertexHelper vh = Tools.vertexHelper;
            vh.Clear();

            Sprite s = null;
            ISpriteData sd;
            for (int i = 0; i < mData.Count; ++i)
            {
                s = (sd = mData[i]).sprite.Get();
                if (s == null)
                {
                    sd.renderer = false;
                    isCurrentEmpty = true;
                    continue;
                }

                sd.renderer = true;
                var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(s);
                sd.Gen(vh, uv);
            }

            if (s == null)
                return;
            
            Mesh workerMesh = SymbolText.WorkerMesh;
            vh.FillMesh(workerMesh);
            canvasRenderer.SetMesh(workerMesh);
            canvasRenderer.SetTexture(s.texture);
        }

        public override void Release()
        {
            base.Release();
            ReleaseSelf();
        }

        void ReleaseSelf()
        {
            foreach (var ator in mData)
                ator.sprite.SubRef();
            mData.Clear();
        }

        private void OnDestroy()
        {
            ReleaseSelf();
        }

        public void GraphicUpdateComplete() { }
        public bool IsDestroyed() { return this == null; }
        public void LayoutComplete() { }
    }
}