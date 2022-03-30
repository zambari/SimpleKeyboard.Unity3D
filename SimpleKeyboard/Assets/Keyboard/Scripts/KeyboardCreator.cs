using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif
namespace Z.Keyboard
{
    [RequireComponent(typeof(KeyboardController))]
    [RequireComponent(typeof(RectTransform))]
    public class KeyboardCreator : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Simple Keyboard Creator")]
        public static void Create()
        {
            GameObject gameObject = new GameObject("KeyboardCreator");
            Canvas canvas = null;

            if (Selection.activeObject != null)
            {
                canvas = Selection.activeGameObject.GetComponentInParent<Canvas>();
                gameObject.transform.SetParent(Selection.activeGameObject.transform);
            }
            if (canvas == null)
            {
                canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas) gameObject.transform.SetParent(canvas.transform);
            }
            gameObject.AddComponent<KeyboardCreator>();
            gameObject.transform.localScale = Vector3.one;
            Selection.activeGameObject = gameObject;
        }
#endif
        public List<string> rowCharacters;
        public Vector2Int buttonSize = new Vector2Int(100, 100);
        // Update is called once per frame
        //  [ExposeMethodInEditor]

        public RectTransform rectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
        private RectTransform _rectTransform;
        public Vector2 offsetmax;
        [Header("TemplateObjects")]
        public KeyboardButtonLetter letterButtonTemplate;
        public HorizontalLayoutGroup rowLayoutGroupTemplate;

        [HideInInspector]
        public bool destroyChildreRowsOnly = true;
        [Header("Wish List")]

        public bool createRowTemplate = true;
        public bool createButtonTemplate = true;
        bool wasNumeric;
        [Header("Check after your key template is ready")]
        public bool populateAllKeyboard = true;
        public bool addReturnAndBackSpace = true;
        [Space(10)]
        public bool makeNumeric = false;
        public bool removeTheCreator = false;

        List<HorizontalLayoutGroup> rows = new List<HorizontalLayoutGroup>();
        public KeyboardController keyboardController { get { if (_keyboardController == null) _keyboardController = GetComponent<KeyboardController>(); return _keyboardController; } }
        private KeyboardController _keyboardController;
        public bool addBlanker = true;
        public UnityAction closeAction;
        void OnValidate()
        {
            if (removeTheCreator)
            {
                destroyChildreRowsOnly = false;
                createButtonTemplate = false;
                createButtonTemplate = false;
                populateAllKeyboard = false;
                addReturnAndBackSpace = false;

            }
            if (transform.childCount == 0) destroyChildreRowsOnly = false;
            // if (populateAllKeyboard) destroyChildreRowsOnly = false;
            if (!rowLayoutGroupTemplate) createRowTemplate = true;
            if (!letterButtonTemplate) createButtonTemplate = true;
            if (keyboardController.blanker != null) addBlanker = false;
            if (makeNumeric && !wasNumeric)
            {
                addReturnAndBackSpace = false;
                wasNumeric = true;
                PresetNumPad();
            }
            if (!makeNumeric && wasNumeric)
            {
                wasNumeric = false;
                PresetQwerty();
            }

        }

        void PresetQwerty()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Keyboard");
#endif
            rowCharacters = new List<string>();
            rowCharacters.Add("1234567890");
            rowCharacters.Add("QWERTYUIOP[]");
            rowCharacters.Add("ADFGHJKL;'\'");
            rowCharacters.Add("ZXCVBNM,./");
        }
        // [ExposeMethodInEditor]
        void PresetNumPad()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Keyboard");
#endif
            rowCharacters = new List<string>();
            rowCharacters.Add("789");
            rowCharacters.Add("456");
            rowCharacters.Add("321");
            rowCharacters.Add("#0X");
        }
        void AddBlankerObject()
        {
#if UNITY_EDITOR
            if (keyboardController.blanker != null)
            {
                Debug.Log("this object already has blanker");
                addBlanker = false;
                return;
            }
            var blankerObj = new GameObject("Blanker");
            var layouel = blankerObj.AddComponent<LayoutElement>();
            layouel.ignoreLayout = true;
            blankerObj.transform.SetParent(transform);
            blankerObj.transform.FillParent();
            blankerObj.transform.SetAsFirstSibling();
            var rect = blankerObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, 300);
            var img = blankerObj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.2f);
            var button = blankerObj.AddComponent<Button>();
            button.transform.localScale = Vector3.one * 2;

            UnityAction action = new UnityAction(keyboardController.Close);
            UnityEventTools.AddPersistentListener(button.onClick, action);
            keyboardController.blanker = button;
#endif
        }
        void DestroyChildrenRows()
        {
            int deleted = 0;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var thischildTransform = transform.GetChild(i);
                var thisHoriz = thischildTransform.GetComponent<HorizontalLayoutGroup>();
                if (thisHoriz && thischildTransform != rowLayoutGroupTemplate.transform)
                {
#if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(thischildTransform.gameObject);
#else
                    GameObject.DestroyImmediate(thischildTransform.gameObject);
#endif

                    deleted++;
                }
                else
                {

                    for (int j = thischildTransform.transform.childCount - 1; j >= 0; j--)
                    {
                        var thisLetterChild = thischildTransform.GetChild(j);
                        var thisLetter = thisLetterChild.GetComponent<KeyboardButtonLetter>();
                        if (thisLetter != letterButtonTemplate && thisLetter != null)
                        {
#if UNITY_EDITOR
                            Undo.DestroyObjectImmediate(thisLetter.gameObject);
#else
                    GameObject.DestroyImmediate(thisLetter.gameObject);
#endif

                            deleted++;
                        }
                    }
                }
            }
            Debug.Log(deleted == 0 ? "Nothing Deleted " : $"Deleted {deleted} rows and buttons");
            rows = new List<HorizontalLayoutGroup>();
        }


        HorizontalLayoutGroup GetRowInstance()
        {
            var thisRow = GameObject.Instantiate(rowLayoutGroupTemplate, rowLayoutGroupTemplate.transform.parent);
            thisRow.gameObject.SetActive(true);
            thisRow.name = "Row " + rows.Count;
            return thisRow.GetComponent<HorizontalLayoutGroup>();
        }

        void PrepareThisRect()
        {
            rectTransform.FillParent();
            rectTransform.anchorMax = new Vector2(1, 0.5f);
        }
        void Reset()
        {
            name = "Keyboard";
            PresetQwerty();
            PrepareThisRect();
            var verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            if (!verticalLayoutGroup) verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            createRowTemplate = rowLayoutGroupTemplate == null;
            createButtonTemplate = letterButtonTemplate == null;

            var image = GetComponent<Image>();
            if (image)
            {
#if UNITY_EDITOR
                Undo.RecordObject(image, "Keyboard");
#endif
                image.enabled = false;

            }
        }

        KeyboardButtonLetter CreateTemplateButton()
        {
            var letterGO = new GameObject("Letter", typeof(Image), typeof(LayoutElement));
            var letterImage = letterGO.AddOrGetComponent<Image>();
            letterImage.color = Color.gray;
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(letterGO, "Keyboard");
#endif
            var thisLauout = letterGO.GetComponent<LayoutElement>();
            thisLauout.preferredWidth = buttonSize.x;
            thisLauout.preferredHeight = buttonSize.y;
            letterGO.transform.SetParent(transform);
            var letterTextObj = new GameObject("Character", typeof(Text));
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(letterTextObj, "Keyboard");
#endif
            var text = letterTextObj.GetComponent<Text>();
            text.transform.FillParent(letterGO.transform);
            text.raycastTarget = false;
            text.text = "Aa";
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 24;
            if (rowLayoutGroupTemplate)
            {
                letterGO.transform.FillParent(rowLayoutGroupTemplate.transform);
            }

            return letterGO.AddComponent<KeyboardButtonLetter>();
        }

        HorizontalLayoutGroup CreateHorizontalLayoutTemplate()
        {
            var rowGO = new GameObject("Row"); //, typeof(LayoutElement)
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(rowGO, "Keyboard");
#endif
            var layoutGroup = rowGO.AddComponent<HorizontalLayoutGroup>();
            int spacig = 12;
            layoutGroup.spacing = spacig;
            spacig = 5;
            layoutGroup.padding = new RectOffset(spacig, spacig, spacig, spacig);
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            rowGO.transform.FillParent(transform);

            return layoutGroup;
        }


        List<KeyboardButtonLetter> PopulateRow(Transform row, string characters)
        {
            List<KeyboardButtonLetter> list = new List<KeyboardButtonLetter>();
            foreach (var c in characters)
            {
                var thisLetter = GameObject.Instantiate(letterButtonTemplate, row);
                list.Add(thisLetter);
                thisLetter.letter = c.ToString();
                thisLetter.gameObject.SetActive(true);
            }
            return list;
        }

        void AddReturnAndBackspace()
        {
            if (rows.Count > 0)
            {
                var addedKeys = PopulateRow(rows[0].transform, "⌫");
                var backspace = addedKeys[0];
                backspace.overrideLetter = true;
                backspace.keyCode = KeyCode.Backspace;
            }
            if (rows.Count > 2)
            {
                var addedKeys = PopulateRow(rows[2].transform, "⏎"); ;
                var returnKey = addedKeys[0];
                returnKey.overrideLetter = true;
                returnKey.keyCode = KeyCode.Return;
            }
        }
        void PopulateKeyboard()
        {
            if (!letterButtonTemplate)
            {
                Debug.Log("no button template", gameObject);
                return;
            }
            rows = new List<HorizontalLayoutGroup>();
            var orgParent = letterButtonTemplate.transform.parent;
            letterButtonTemplate.transform.SetParent(transform);
            if (!rowLayoutGroupTemplate)
            {
                Debug.Log("no row template", gameObject);
                return;
            }
            rowLayoutGroupTemplate.gameObject.SetActive(false);
            letterButtonTemplate.gameObject.SetActive(false);
            for (int i = 0; i < rowCharacters.Count; i++)
            {
                var thisRow = GetRowInstance();

                rows.Add(thisRow);
                PopulateRow(thisRow.transform, rowCharacters[i]);
            }
            letterButtonTemplate.transform.SetParent(orgParent);
            if (!makeNumeric)
            {
                var newRow = GetRowInstance();
                PopulateRow(newRow.transform, " ");
                var thisLetter = newRow.GetComponentInChildren<KeyboardButtonLetter>();
                var thisLayout = thisLetter.GetComponent<LayoutElement>();
                thisLayout.preferredWidth *= 5;
            }
            gameObject.AddOrGetComponent<KeyboardController>();
        }


        [ExposeMethodInEditor]
        void RunCreatorWishlist()
        {

#if UNITY_EDITOR
            if (removeTheCreator)
            {
                Undo.DestroyObjectImmediate(this);
            }
            if (destroyChildreRowsOnly || populateAllKeyboard)
            {
                DestroyChildrenRows();
            }
            if (keyboardController.blanker == null && addBlanker)
                AddBlankerObject();


            if (createRowTemplate)
                if (rowLayoutGroupTemplate == null)
                {
                    rowLayoutGroupTemplate = CreateHorizontalLayoutTemplate();
                    createRowTemplate = false;
                }

            if (!rowLayoutGroupTemplate)
            {
                Debug.Log("No Row templatea, sorry, please create");
                createRowTemplate = true;
                return;
            }
            if (makeNumeric)
            {
                PresetNumPad();
                makeNumeric = false;
            }

            if (letterButtonTemplate == null)
            {
                if (createButtonTemplate)
                {

                    letterButtonTemplate = CreateTemplateButton();
                    createButtonTemplate = false;
                }

            }
            if (!letterButtonTemplate)
            {
                Debug.Log("No button template. Please create");
                createButtonTemplate = true;
                return;
            }
            if (populateAllKeyboard)
            {
                //   populateAllKeyboard = false;
                PopulateKeyboard();
                if (addReturnAndBackSpace)
                    AddReturnAndBackspace();
            }

            // thisLayout.childControlWidth=trowLayoutGroup\
#else
    Debug.Log("Editor only, sorry");
#endif
        }

    }
}