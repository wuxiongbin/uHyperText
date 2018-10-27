using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WXB
{
    public class TagAttributes
    {
        private Dictionary<string, string> d_attrs = new Dictionary<string, string>();

        public override string ToString()
        {
            using (PD<StringBuilder> psb = Pool.GetSB())
            {
                StringBuilder sb = psb.value;
                sb.AppendFormat("count:{0}", d_attrs.Count);
                sb.AppendLine();
                foreach (var itor in d_attrs)
                {
                    sb.AppendLine("key:{0} value:{1}", itor.Key, itor.Value);
                }

                string t = sb.ToString();
                sb.Length = 0;
                return t;
            }
        }

        public void Release()
        {
            d_attrs.Clear();
        }

        public void Add(string text)
        {
            int startpos = 0;
            int lenght = text.Length;
            string name = string.Empty;
            string value = string.Empty;
            int temppos = 0;
            bool issetname = false;
            while (startpos < lenght)
            {
                if (text[startpos] == '=')
                {
                    name = text.Substring(temppos, startpos - temppos);
                    temppos = startpos + 1;
                    issetname = true;
                }
                else if (text[startpos] == ' ')
                {
                    if (!issetname)
                    {
                        Debug.LogErrorFormat("error param!");
                        return;
                    }
                    value = text.Substring(temppos, startpos - temppos);
                    temppos = startpos + 1;
                    d_attrs[name] = value;
                    issetname = false;
                }

                ++startpos;
            }

            if (issetname)
            {
                value = text.Substring(temppos);
                d_attrs[name] = value;
            }
        }

        public void add(string attrName, string attrValue)
        {
            d_attrs[attrName] = attrValue;
        }

        public void remove(ref string attrName)
        {
            d_attrs.Remove(attrName);
        }

        public bool exists(string attrName)
        {
            return d_attrs.ContainsKey(attrName);
        }

        public int getCount()
        {
            return d_attrs.Count;
        }

        public string getValue(string attrName)
        {
            string name = "";
            d_attrs.TryGetValue(attrName, out name);
            return name;
        }

        public string getValueAsString(string attrName)
        {
            string name = "";
            if (d_attrs.TryGetValue(attrName, out name) == false)
                return "";

            return name;
        }

        public Color getValueAsColor(string attrName, Color color)
        {
            string name = "";
            if (d_attrs.TryGetValue(attrName, out name) == false)
                return color;

            return Tools.ParseColor(name, 0, Color.white);
        }

        public string getValueAsString(string attrName, string def)
        {
            string name = "";
            if (d_attrs.TryGetValue(attrName, out name) == false)
                return def;

            return name;
        }

        public bool getValueAsBool(string attrName, bool def)
        {
            string name = "";
            if (d_attrs.TryGetValue(attrName, out name) == false)
                return def;

            bool value = def;
            if (bool.TryParse(name, out value) == false)
            {
                int v = 0;
                if (int.TryParse(name, out v))
                    return v == 0 ? false : true;

                return def;
            }

            return value;
        }

        public int getValueAsInteger(string attrName, int def)
        {
            string name = "";
            if (d_attrs.TryGetValue(attrName, out name) == false)
                return def;

            int value = def;
            if (int.TryParse(name, out value) == false)
                return def;

            return value;
        }

        public float getValueAsFloat(string attrName, float def)
        {
            string name = "";
            if (d_attrs.TryGetValue(attrName, out name) == false)
                return def;

            float value = def;
            if (float.TryParse(name, out value) == false)
                return def;

            return value;
        }
    }
}