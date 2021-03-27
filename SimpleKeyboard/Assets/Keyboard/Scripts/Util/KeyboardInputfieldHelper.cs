using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Z.Keyboard
{
    public class KeyboardInputfieldHelper : MonoBehaviour
    {
        public InputField inputField { get { if (_inputField == null) _inputField = GetComponent<InputField>(); return _inputField; } }
        private InputField _inputField;
        bool wasFocused;
        [SerializeField]
        KeyboardController _keyboard;
        public UnityEvent additionalActionsOnOpen;
        
        public KeyboardController keyboard
        {
            get
            {
                if (_keyboard == null)
                {
                    var keyboards = Resources.FindObjectsOfTypeAll<KeyboardController>();
                    if (keyboards.Length > 0) _keyboard = keyboards[0];
                }
                return _keyboard;
            }
        }

        void Update()
        {
            if (inputField.isFocused && !keyboard.isVisible)
            {
                keyboard.OpenForInputField(inputField);
                additionalActionsOnOpen.Invoke();
            }

        }
    }


}