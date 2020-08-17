using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace WXB
{
    [RequireComponent(typeof(SymbolText))]
    public class SymbolTextEvent : MonoBehaviour, /*IPointerEnterHandler, IPointerExitHandler,*/ IPointerDownHandler, IPointerUpHandler
    {
        SymbolText d_symbolText;

        //RenderCache.BaseData d_baseData;

        [System.Serializable]
        public class OnClickEvent : UnityEvent<NodeBase>
        {

        }

        public OnClickEvent OnClick = new OnClickEvent(); // 点击了此结点

        void OnEnable()
        {
            if (d_symbolText == null)
            {
                d_symbolText = GetComponent<SymbolText>();
            }
        }

        void OnDisable()
        {
            //isEnter = false;
            //d_baseData = null;
            d_down_basedata = null;
            localPosition = Vector2.zero;
        }

        //bool isEnter = false;

        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    isEnter = true;
        //}

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    isEnter = false;
        //}

        RenderCache.BaseData d_down_basedata;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Tools.ScreenPointToWorldPointInRectangle(d_symbolText.rectTransform, eventData.position, d_symbolText.canvas.worldCamera, out localPosition))
                return;

            //localPosition *= d_symbolText.pixelsPerUnit;
            d_down_basedata = d_symbolText.renderCache.Get(localPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            if (!Tools.ScreenPointToWorldPointInRectangle(d_symbolText.rectTransform, eventData.position, d_symbolText.canvas.worldCamera, out localPosition))
                return;

            //localPosition *= d_symbolText.pixelsPerUnit;
            var up_node = d_symbolText.renderCache.Get(localPosition);
            if (d_down_basedata != up_node)
                return;

            if (d_down_basedata != null)
            {
                OnClick.Invoke(d_down_basedata.node);
            }
        }

        Vector2 localPosition;

        //void Update()
        //{
        //    if (isEnter)
        //    {
        //        Tools.ScreenPointToWorldPointInRectangle(d_symbolText.rectTransform, Input.mousePosition, d_symbolText.canvas.worldCamera, out localPosition);

        //        localPosition *= d_symbolText.pixelsPerUnit;
        //        //Debug.LogFormat("Pos:{0}", localPosition);
        //        RenderCache.BaseData bd = d_symbolText.renderCache.Get(localPosition);
        //        if (d_baseData != bd)
        //        {
        //            if (d_baseData != null)
        //            {
        //                d_baseData.OnMouseLevel();
        //            }

        //            d_baseData = bd;

        //            if (d_baseData != null)
        //                d_baseData.OnMouseEnter();
        //        }
        //    }
        //    else
        //    {
        //        if (d_baseData != null)
        //        {
        //            d_baseData.OnMouseLevel();

        //            d_baseData = null;
        //        }
        //    }
        //}
    }
}
