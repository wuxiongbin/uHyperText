using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    [ExecuteInEditMode]
    public class AlphaOffsetDraw : OffsetDraw
    {
        public override DrawType type
        {
            get
            {
                return DrawType.OffsetAndAlpha;
            }
        }

        protected override void Init()
        {
            base.Init();
            m_Effects[1] = new AlphaEffect();
        }
    }
}