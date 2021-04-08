using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Z.Keyboard
{

    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class VoidEvent : UnityEvent { }
    public static class KeyboardExtensions
    {
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            T t = gameObject.GetComponent<T>();
            if (t == null)
            {
                t = gameObject.AddComponent<T>();
#if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(t, "Added component");
#endif
            }
            return t;
        }
        public static Image AddImageChild(this GameObject g, float opacity = 0.3f)
        {
            Image image = g.AddChildRectTransform().gameObject.AddComponent<Image>();
            image.color = new Color(Random.value * 0.3f + 0.7f,
                Random.value * 0.3f + 0.7f,
                Random.value * 0.2f, opacity);
            image.sprite = Resources.Load("Background") as Sprite;
            image.name = "Image";
            return image;
        }
        public static RectTransform AddChildRectTransform(this GameObject parent)
        {
            RectTransform parentRect = parent.GetComponent<RectTransform>();
            return parentRect.AddChild();
        }

        public static Transform FillParent(this Transform rect, Transform parentToSet = null)
        {
            if (parentToSet != null)
                rect.SetParent(parentToSet);
            var thisRect = rect.gameObject.AddOrGetComponent<RectTransform>();
            if (parentToSet)
            {
                rect.SetParent(parentToSet);
            }
            return thisRect.FillParent();
        }
        public static RectTransform AddChild(this RectTransform parentRect)
        {
            GameObject go = new GameObject();
            RectTransform rect = go.GetComponent<RectTransform>();

            go.transform.SetParent(parentRect);

            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(10, 10);
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);
            rect.localPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            //Debug.Log(" added child to ",parentRect.gameObject);
            //	Debug.Log("new object is",rect.gameObject);

            return rect;
        }
        public static RectTransform FillParent(this RectTransform rect, Transform parentToSet = null)
        {
            if (parentToSet != null)
                rect.SetParent(parentToSet);
            if (parentToSet == null)
                parentToSet = rect.transform.parent.GetComponent<RectTransform>();
            if (parentToSet)
            {
                rect.localScale = Vector3.one;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.localPosition = Vector3.zero;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }
            return rect;
        }
    } // extensions end
} // namepsace end