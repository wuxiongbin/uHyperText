﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 文本解析
namespace WXB
{
    public partial class TextParser
    {
        Dictionary<string, Action<string, string>> TagFuns;

        static TagAttributes s_TagAttributes = new TagAttributes();

        void Reg(string type, Action<string, TagAttributes> fun)
        {
            TagFuns.Add(type, (string key, string param) => 
            {
                s_TagAttributes.Release();
                s_TagAttributes.Add(param);

                try
                {
                    fun(type, s_TagAttributes);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                s_TagAttributes.Release();
            });
        }

        static Color ParserColorName(string name, int startpos, Color c)
        {
            if (string.IsNullOrEmpty(name))
                return c;
            if (name[startpos] == '#')
            {
                return Tools.ParseColor(name, startpos + 1, c);
            }
            else
            {
                return ColorConst.Get(startpos == 0 ? name : name.Substring(startpos), c);
            }
        }

        static void SetDefaultConfig(NodeBase nb, TagAttributes att)
        {
            nb.d_bBlink = att.getValueAsBool("b", nb.d_bBlink);
            nb.d_color = ParserColorName(att.getValueAsString("c"), 0, nb.d_color);

            int offsetv = att.getValueAsInteger("x", -1);
            if (offsetv > 0)
            {
                nb.d_bOffset = true;
                nb.d_rectOffset.xMin = -offsetv / 2;
                nb.d_rectOffset.xMax = offsetv / 2;
            }

            offsetv = att.getValueAsInteger("y", -1);
            if (offsetv > 0)
            {
                nb.d_bOffset = true;
                nb.d_rectOffset.yMin = -offsetv / 2;
                nb.d_rectOffset.yMax = offsetv / 2;
            }
        }

        static void SetSizeConfig(RectNode nb, TagAttributes att, Vector2 size)
        {
            nb.width = att.getValueAsFloat("w", size.x);
            nb.height = att.getValueAsFloat("h", size.y);

            switch (att.getValueAsInteger("t", 0))
            {
            case 1: nb.height = nb.width * size.y / size.x; break;
            case 2: nb.width = nb.height * size.x / size.y; break;
            }
        }

        void RegTag()
        {
            TagFuns = new Dictionary<string, Action<string, string>>();

            Reg("sprite ", (string tag, TagAttributes att) =>
            {
                string name = att.getValueAsString("n");
                ISprite sprite = Tools.GetSprite(name);
                if (sprite == null)
                {
                    // 没有查找到
                    Debug.LogErrorFormat("not find sprite:{0}!", name);
                    return;
                }

                Vector2 size = new Vector2(sprite.width, sprite.height);

                SpriteNode sn = CreateNode<SpriteNode>();
                sn.SetSprite(sprite);
                sn.SetConfig(currentConfig);

                SetSizeConfig(sn, att, size);
                SetDefaultConfig(sn, att);

                d_nodeList.Add(sn);
            });

            Reg("pos ", (string tag, TagAttributes att) =>
            {
                SetPosNode node = CreateNode<SetPosNode>();
                node.d_value = att.getValueAsFloat("v", 0);
                node.type = (TypePosition)att.getValueAsInteger("t", (int)(TypePosition.Absolute));
                d_nodeList.Add(node);
            });

            Reg("RectSprite ", (string tag, TagAttributes att) =>
            {
                ISprite s = Tools.GetSprite(att.getValueAsString("n")); // 名字
                if (s == null)
                {
                    // 没有查找到
                    Debug.LogErrorFormat("not find sprite:{0}!", att.getValueAsString("n"));
                    return;
                }

                RectSpriteNode sn = CreateNode<RectSpriteNode>();
                sn.SetConfig(currentConfig);
                //Rect rect = s.rect;
                sn.SetSprite(s);

                sn.rect.width = att.getValueAsFloat("w", s.width);
                sn.rect.height = att.getValueAsFloat("h", s.height);

                switch (att.getValueAsInteger("t", 0))
                {
                case 1: sn.rect.height = sn.rect.width * s.height / s.width; break;
                case 2: sn.rect.width = sn.rect.height * s.width / s.height; break;
                }

                sn.rect.x = att.getValueAsFloat("px", 0f);
                sn.rect.y = att.getValueAsFloat("py", 0f);

                SetDefaultConfig(sn, att);

                d_nodeList.Add(sn);
            });

            Reg("hy ", (string tag, TagAttributes att) => 
            {
                HyperlinkNode node = CreateNode<HyperlinkNode>();
                node.SetConfig(currentConfig);
                node.d_text = att.getValueAsString("t");
                node.d_link = att.getValueAsString("l");
                node.d_fontSize = att.getValueAsInteger("fs", node.d_fontSize);
                node.d_fontStyle = (FontStyle)att.getValueAsInteger("ft", (int)node.d_fontStyle);

                if (att.exists("fn"))
                    node.d_font = Tools.GetFont(att.getValueAsString("fn"));

                node.d_color = ParserColorName(att.getValueAsString("fc"), 0, node.d_color);
                node.hoveColor = ParserColorName(att.getValueAsString("fhc"), 0, node.d_color);

                node.d_bUnderline = att.getValueAsBool("ul", node.d_bUnderline);
                node.d_bStrickout = att.getValueAsBool("so", node.d_bStrickout);
                d_nodeList.Add(node);
            });

            Reg("face ", (string tag, TagAttributes att) => 
            {
                string name = att.getValueAsString("n");
                Cartoon c = Tools.GetCartoon(name);
                if (c == null)
                    return;

                CartoonNode cn = CreateNode<CartoonNode>();
                cn.cartoon = c;
                cn.width = c.width;
                cn.height = c.height;

                cn.SetConfig(currentConfig);

                SetSizeConfig(cn, att, new Vector2(c.width, c.height));
                SetDefaultConfig(cn, att);

                d_nodeList.Add(cn);
            });

            TagFuns.Add("color=", (string tag, string param) => 
            {
                if (string.IsNullOrEmpty(param))
                    return;

                currentConfig.fontColor = ParserColorName(param, 0, currentConfig.fontColor);
            });

            TagFuns.Add("/color", (string tag, string param)=> 
            {
                currentConfig.fontColor = startConfig.fontColor;
            });

            TagFuns.Add("b", (string tag, string param) =>
            {
                currentConfig.fontStyle |= FontStyle.Bold;
            });

            TagFuns.Add("/b", (string tag, string param) =>
            {
                currentConfig.fontStyle &= ~FontStyle.Bold;
            });

            TagFuns.Add("i", (string tag, string param) =>
            {
                currentConfig.fontStyle |= FontStyle.Italic;
            });

            TagFuns.Add("/i", (string tag, string param) =>
            {
                currentConfig.fontStyle &= ~FontStyle.Italic;
            });

            TagFuns.Add("size=", (string tag, string param) =>
            {
                currentConfig.fontSize = (int)Tools.stringToFloat(param, currentConfig.fontSize);
            });

            TagFuns.Add("/size", (string tag, string param) =>
            {
                currentConfig.fontSize = startConfig.fontSize;
            });

            // 描边效果
            Reg("ol ", (string tag, TagAttributes att) => 
            {
                currentConfig.effectType = EffectType.Outline;
                ParamEffectType(ref currentConfig, att);
            });

            TagFuns.Add("/ol", (string tag, string param) =>
            {
                currentConfig.effectType = EffectType.Null;
            });

            // 阴影
            Reg("so ", (string tag, TagAttributes att) =>
            {
                currentConfig.effectType = EffectType.Shadow;
                ParamEffectType(ref currentConfig, att);
            });

            TagFuns.Add("/so", (string tag, string param) =>
            {
                currentConfig.effectType = EffectType.Null;
            });

            Reg("offset ", (string tag, TagAttributes att)=> 
            {
                float x = att.getValueAsFloat("x", 0f);
                float y = att.getValueAsFloat("y", 0f);

                if (x <= 0f && y <= 0f)
                    return;

                currentConfig.isOffset = true;
                currentConfig.offsetRect.xMin = -x / 2f;
                currentConfig.offsetRect.xMax = x / 2f;

                currentConfig.offsetRect.yMin = -x / 2f;
                currentConfig.offsetRect.yMax = x / 2f;
            });

            // 外部结点
            Reg("external ", (tag, att)=> 
            {
                if (getExternalNode == null)
                {
                    Debug.LogErrorFormat("external node but getExternalNode is null!");
                    return;
                }

                ExternalNode sn = CreateNode<ExternalNode>();
                sn.SetConfig(currentConfig);
                sn.Set(getExternalNode(att));

                d_nodeList.Add(sn);
            });
        }

        static void ParamEffectType(ref Config config, TagAttributes att)
        {
            config.effectColor = ParserColorName(att.getValueAsString("c"), 0, Color.black);
            config.effectDistance.x = att.getValueAsFloat("x", 1f);
            config.effectDistance.y = att.getValueAsFloat("y", 1f);
        }

        void TagParam(string tag, string param)
        {
            System.Action<string, string>  fun;
            if (TagFuns.TryGetValue(tag, out fun))
            {
                fun(tag, param);
            }
            else
            {
                Debug.LogErrorFormat("tag:{0} param:{1} not find!", tag, param);
            }
        }
    }
}

