using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Z.Keyboard
{
    public class KeyboardLongPressObjectReferences : MonoBehaviour
    {
        [SerializeField]
        KeyboardButtonLetter letterTemplate;
        [SerializeField]
        RectTransform horizontalLayoutTransform;
        [Range(0.1f, 2f)]
        public float waitTime = 0.3f;
        public RectTransform rectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
        private RectTransform _rectTransform;
        void Reset()
        {
            letterTemplate = GetComponentInChildren<KeyboardButtonLetter>();

        }
        public void Show()
        {
            gameObject.SetActive(true);

        }
        public void Hide()
        {
            Debug.Log("hiding");
            gameObject.SetActive(false);
        }
        void DestroyOtherKeys()
        {
            Transform parent = letterTemplate.transform.parent;
            for (int i = parent.childCount - 1; i > 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        void Start()
        {
            if (letterTemplate == null)
            {
                Debug.Log("no letter template", gameObject);
            }
            else
            {

                Debug.Log("added hide callback");
                if (Time.time < 1)
                    Hide();
                if (horizontalLayoutTransform == null)
                    horizontalLayoutTransform = letterTemplate.transform.parent.GetComponent<RectTransform>();
            }
            letterTemplate.gameObject.SetActive(false);
        }
        public void SetLetters(string letters, Vector3 position)
        {
            Show();
            DestroyOtherKeys();
            float width = Screen.width; 
            float ratioX = position.x / width;
            horizontalLayoutTransform.pivot = new Vector2(ratioX, horizontalLayoutTransform.pivot.y);
            horizontalLayoutTransform.position = position;
            Debug.Log($"set pos {position} {ratioX}");
            foreach (var l in letters)
            {
                var thisLetter = GameObject.Instantiate(letterTemplate, letterTemplate.transform.parent) as KeyboardButtonLetter;
                thisLetter.gameObject.SetActive(true);
                thisLetter.letter = l.ToString();
                thisLetter.GetComponent<Button>().onClick.AddListener(Hide);
            }
        }

    }
}