using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WXB;

public class Test : MonoBehaviour
{
    [SerializeField]
    SymbolLabel label;

    [SerializeField]
    GameObject prefab;

    int total = 0;

    void Awake()
    {
        prefab.SetActive(false);
        label.getExternalNode = GetExternalNode;
    }

    Dictionary<string, IExternalNode> Nodes = new Dictionary<string, IExternalNode>();

    IExternalNode GetExternalNode(TagAttributes tag)
    {
        IExternalNode node;
        var id = tag.getValue("id");
        if (Nodes.TryGetValue(id, out node))
            return node;

        var child = Object.Instantiate(prefab);
        Tools.AddChild(label.gameObject, child);
        child.name = "external:" + id;

        node = new RectTransformNode(child.transform as RectTransform);
        Nodes.Add(id, node);
        return node;
    }

    int id_flag = 0;
    void OnGUI()
    {
        if (GUILayout.Button("添加一个测试结点"))
        {
            label.Append($"<sprite n=CG h=19 t=2>#&35[你丫的:{total++}]你在说什么啊,哦哦哦@#1#2#3#r<external id=\"{id_flag++}\">");
        }
    }
}
