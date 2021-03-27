using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Z.Keyboard
{
    public class KeyboardLongPressButtonHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public KeyboardLongPressObjectReferences longPressObjectReferences;

        [SerializeField]
        [Space(20)]
        [TextArea(3, 5)]
        string alternativeLetters;
        [Space(20)]
        float holdTime = 0.5f;
        IEnumerator Holder()
        {
            yield return new WaitForSeconds(longPressObjectReferences.waitTime);
            OnLongPress();
        }
        public RectTransform rectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
        private RectTransform _rectTransform;
        void OnLongPress()
        {
            longPressObjectReferences.SetLetters(alternativeLetters, rectTransform.position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Holder());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
        }
        void OnValidate()
        {
            if (longPressObjectReferences == null)
                longPressObjectReferences = GameObject.FindObjectOfType<KeyboardLongPressObjectReferences>();
        }
    }
}