using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Z.Keyboard
{
    [RequireComponent(typeof(Button))]
    public class KeyboardButtonLetter : MonoBehaviour
    {
        public bool overrideLetter;
        public KeyCode keyCode;

        [SerializeField]
        [Space(20)]
        [TextArea(3, 3)]
        string _letter;

        [Space(20)]



        public bool makeUpperCase;
        public bool makeLowerCase;
        KeyboardController controller { get { if (_controller == null) _controller = GetComponentInParent<KeyboardController>(); return _controller; } }
        private KeyboardController _controller;
        public Text text { get { if (_text == null) _text = GetComponentInChildren<Text>(); return _text; } }
        private Text _text;
        public string letter
        {
            get { return _letter; }
            set
            {
                _letter = value;
                if (text & !overrideLetter)
                    text.text = overrideLetter ? keyCode.ToString() : letter;
                ApplyName();
            }
        }
        [ExposeMethodInEditor]
        void ApplyName()
        {
            string newName = "Key " + (overrideLetter ? keyCode.ToString() : letter);

            if (name != newName)
                name = newName;
        }
        void Start()
        {
            Button b = GetComponent<Button>();
            b.onClick.AddListener(OnCLick);
        }
        void OnCLick()
        {
            if (controller)
            {
                if (overrideLetter)

                    controller.AddKeycode(keyCode);
                else
                    controller.AddLetter(text.text);
            }
            if (gameObject.activeInHierarchy)
                StartCoroutine(ImageFlash());
        }
        void OnValidate()
        {
            _letter = letter;

            if (makeUpperCase)
            {
                letter = letter.ToUpper();
                makeUpperCase = false;
            }
            if (makeLowerCase)
            {
                letter = letter.ToLower();
                makeLowerCase = false;
            }



        }
        protected virtual IEnumerator ImageFlash()
        {
            var image = GetComponent<Image>();
            image.enabled = false;
            yield return null;
            yield return null;
            image.enabled = true;
        }

    }
}