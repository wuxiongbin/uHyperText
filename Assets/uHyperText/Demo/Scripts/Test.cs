using UnityEngine;
using System.Collections;
using WXB;

public class Test : MonoBehaviour
{
    [SerializeField]
    SymbolLabel label;

    int total = 0;

    void OnGUI()
    {
        if (GUILayout.Button("添加一个测试结点"))
        {
            label.Append(string.Format("<sprite n=CG h=19 t=2>#&35[你丫的:{0}]你在说什么啊，你这是啥啊，这是测试啊，这是测试啊，这是测试啊，这是测试啊，这是测试啊，这是测试啊，这是测试啊，这是测试啊，哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3哦哦哦@#1#2#3", total++));
        }
    }
}
