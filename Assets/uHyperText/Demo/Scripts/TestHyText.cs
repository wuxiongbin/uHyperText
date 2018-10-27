using UnityEngine;
using WXB;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestHyText : MonoBehaviour
{
    // 点击超链接
    public void OnClickHy(NodeBase node)
    {
        Debug.LogFormat("type:{0}", node.GetType().Name);
#if UNITY_EDITOR
        if (node is HyperlinkNode)
        {
            HyperlinkNode hn = (HyperlinkNode)node;
            EditorUtility.DisplayDialog("超链接", string.Format("点击了超链接:{0}", hn.d_text), "OK");
        }
#endif
    }
}
