using WXB;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class ShowAllFace : MonoBehaviour
{
    [SerializeField]
    SymbolText text;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Begin());
    }

    IEnumerator Begin()
    {
        List<Cartoon> cartoons = new List<Cartoon>();
        Tools.GetAllCartoons(cartoons);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cartoons.Count; ++i)
        {
            sb.AppendFormat("#{0}", cartoons[i].name);
            text.text = sb.ToString();
        }

        yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
