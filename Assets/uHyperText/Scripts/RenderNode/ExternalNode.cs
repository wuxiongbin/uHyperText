﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public interface IExternalNode
    {
        // 删除
        void OnDestroy();

        void OnRender(Owner owner, Rect rect);

        float width { get; }
        float height { get; }
    }

    public class RectTransformNode : IExternalNode
    {
        RectTransform root;

        public RectTransformNode(RectTransform root)
        {
            this.root = root;
            root.pivot = new Vector2(0, 1);
            root.anchorMin = new Vector2(0, 1);
            root.anchorMax = new Vector2(0, 1);
            this.root.gameObject.SetActive(true);
        }

        // 删除
        void IExternalNode.OnDestroy()
        {
            if (root != null)
            {
                UnityEngine.Object.Destroy(root.gameObject);
                root = null;
            }
        }

        void IExternalNode.OnRender(Owner owner, Rect rect)
        {
            if (root != null)
            {
                root.anchoredPosition = new Vector2(rect.x, -rect.y);
            }
        }

        float IExternalNode.width { get { return root.sizeDelta.x; } }
        float IExternalNode.height { get { return root.sizeDelta.y; } }
    }

    // 外部结点
    public class ExternalNode : RectNode
    {
        public void Set(IExternalNode node)
        {
            this.node = node;
        }

        IExternalNode node;

        public override float getHeight()
        {
            return node.height;
        }

        public override float getWidth()
        {
            return node.width;
        }

        protected override void ReleaseSelf()
        {
            base.ReleaseSelf();
            if (node != null)
                node.OnDestroy();
            node = null;
        }

        protected override void OnRectRender(RenderCache cache, Line line, Rect rect)
        {
            node.OnRender(owner, rect);
        }
	};
}