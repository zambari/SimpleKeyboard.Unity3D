using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// v.0.3
namespace Z.Keyboard
{
    public class KeyboardController : MonoBehaviour
    {
        [SerializeField] bool _closeOnReturn = false;
        public bool diableOnStart;
        public bool doNotNotifyOnEachCharacter { get { return _doNotNotifyOnEachCharacter; } set { _doNotNotifyOnEachCharacter = value; } }
        [SerializeField] bool _doNotNotifyOnEachCharacter = true;
        public bool isVisible { get { return gameObject.activeInHierarchy; } }
        public bool clearInputFieldOnEndEdit { get { return _clearInputFieldOnEndEdit; } set { _clearInputFieldOnEndEdit = value; } }
        [SerializeField] bool _clearInputFieldOnEndEdit;
        public bool closeOnReturn { get { return _closeOnReturn; } set { _closeOnReturn = value; } }
        public bool triggerEventOnClose { get { return _triggerEventOnClose; } set { _triggerEventOnClose = value; } }
        [SerializeField] bool _triggerEventOnClose = true;
        public InputField currentInputField;
        public Button blanker;
        public StringEvent OnEndEdit;
        public StringEvent OnValueChanged;
        public BoolEvent OnKeyboardActivated;
        public string currentText;
        public void OpenForInputField(InputField newInputField)
        {
            currentInputField = newInputField;
            currentText = currentInputField.text;
            gameObject.SetActive(true);
            OnKeyboardActivated.Invoke(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
            if (_triggerEventOnClose)
            {
                OnEndEdit.Invoke(currentText);
            }
            OnKeyboardActivated.Invoke(false);

        }
        public void AddLetter(string letter)
        {
            currentText += letter;
            OnValueChanged.Invoke(currentText);
            if (currentInputField)
            {
                if (doNotNotifyOnEachCharacter)
                    currentInputField.SetTextWithoutNotify(currentText);
                else
                    currentInputField.text = currentText;
            }

        }
        void Start()
        {
            if (Time.time < 1 && diableOnStart) Close();
            //   if (blanker)
            //blanker.onClick.AddListener(Close);
        }
        public void AddKeycode(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Backspace)
            {
                if (currentText.Length > 0)
                {
                    currentText = currentText.Substring(0, currentText.Length - 1);
                    if (doNotNotifyOnEachCharacter)
                    {
                        currentInputField.SetTextWithoutNotify(currentText);
                    }
                    else
                        currentInputField.text = currentText;
                    OnValueChanged.Invoke(currentText);
                }
            }
            else
            if (keyCode == KeyCode.Return)
            {

                if (clearInputFieldOnEndEdit)
                    currentInputField.SetTextWithoutNotify("");
                OnEndEdit.Invoke(currentText);

                if (currentInputField)
                    currentInputField.onEndEdit.Invoke(currentText);
                if (closeOnReturn)
                    Close();
                currentText = "";
            }
        }

    }
}