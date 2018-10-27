using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    [ExecuteInEditMode]
    public class OffsetDraw : EffectDrawObjec
    {
        public override DrawType type { get { return DrawType.Offset; } }

        protected override void Init()
        {
            m_Effects[0] = new OffsetEffect();
        }

        public void Set(Rect rect)
        {
            Set(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
        }

        public void Set(float xMin, float yMin, float xMax, float yMax)
        {
            OffsetEffect oe = m_Effects[0] as OffsetEffect;
            oe.xMin = xMin;
            oe.yMin = yMin;

            oe.xMax = xMax;
            oe.yMax = yMax;
        }
    }
}