using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    [ExecuteInEditMode]
    public class AlphaDraw : EffectDrawObjec
    {
        protected override void Init()
        {
            m_Effects[0] = new AlphaEffect();
        }

        public override DrawType type { get { return DrawType.Alpha; } }

        public override void Release()
        {
            base.Release();
            m_Effects[0].Release();
        }
    }
}