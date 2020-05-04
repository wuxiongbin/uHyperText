//﻿using UnityEngine;
//using System.Collections.Generic;

//namespace WXB
//{
//    public class DelayDestory : MonoBehaviour
//    {
//        static List<GameObject> Destorys = new List<GameObject>();
//        static List<GameObject> DestroyImmediates = new List<GameObject>();

//#if UNITY_EDITOR
//        static DelayDestory()
//        {
//            UnityEditor.EditorApplication.update += UpdateEditor;
//        }

//        static void UpdateEditor()
//        {
//            if (!Application.isPlaying)
//            {
//                OnUpdate();
//            }
//        }
//#endif
//        static DelayDestory instance = null;
//        static void Init()
//        {
//#if UNITY_EDITOR
//            if (!Application.isPlaying)
//                return;
//#endif
//            if (instance == null)
//            {
//                instance = (new GameObject("DelayDestory")).AddComponent<DelayDestory>();
//                //instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
//            }
//        }

//        public static void Destroy(GameObject go)
//        {
//            Init();
//            Destorys.Add(go);
//        }

//        public static void DestroyImmediate(GameObject go)
//        {
//            Init();
//            DestroyImmediates.Add(go);
//        }

//        void LateUpdate()
//        {
//#if UNITY_EDITOR
//            if (!Application.isPlaying)
//                return;
//#endif
//            OnUpdate();
//        }

//        static void OnUpdate()
//        {
//            for (int i = 0; i < Destorys.Count; ++i)
//            {
//                if (Destorys[i] != null)
//                {
//                    Object.Destroy(Destorys[i]);
//                }
//            }
//            Destorys.Clear();

//            for (int i = 0; i < DestroyImmediates.Count; ++i)
//            {
//                if (DestroyImmediates[i] != null)
//                {
//                    Object.DestroyImmediate(DestroyImmediates[i]);
//                }
//            }
//            DestroyImmediates.Clear();
//        }
//    }
//}