#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TrainConstructor.TrainData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TrainConstructor.Editor
{
    public class PartsEditor : EditorWindow
    {
        private const string EDITOR_NAME                    = "Parts Editor";

        private const string EDITOR_PATH                    = "Assets/Scripts/Editor/PartsEditor/PartsEditor.uxml";
        private const string PARTS_FOLDER                   = "Assets/ScriptableObjects/TrainParts";

        private const string ID_TEXT_FIELD_NAME             = "id-text-field";
        private const string TYPE_DROPDOWN_NAME             = "type-dropdown";
        private const string SUBTYPE_DROPDOWN_NAME          = "subtype-dropdown";
        private const string MAIN_TEXTURE_FIELD_NAME        = "main-texture-field";
        private const string OUTLINE_TEXTURE_FIELD_NAME     = "outline-texture-field";
        private const string CREATE_BUTTON_NAME             = "create-button";
        private const string EXISTING_PARTS_LIST_NAME       = "existing-parts-list";
        private const string ERROR_LABEL_NAME               = "error-label";

        private const string TRAIN_PARTS_SO_PATH            = "Assets/ScriptableObjects/TrainParts/TrainPartsSO.asset";

        private TextField idTextField;
        private EnumField typeDropdown;
        private EnumField subtypeDropdown;
        private ObjectField mainTextureField;
        private ObjectField outlineTextureField;
        private Button createButton;
        private ScrollView existingPartsList;
        private Label errorLabel;

        private TrainPartsSO trainPartsSO;

        private List<TrainPartSO> existingParts = new List<TrainPartSO>();
        private List<TrainPartTypeRelationsSO> typesRelations = new List<TrainPartTypeRelationsSO>();

        [MenuItem("Tools/TrainConstructor/PartsEditor")]
        public static void ShowWindow()
        {
            EditorWindow _editorWindow = GetWindow(typeof(PartsEditor));
            _editorWindow.titleContent = new GUIContent(EDITOR_NAME);
        }

        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EDITOR_PATH);
            VisualElement root = visualTree.CloneTree();
            rootVisualElement.Add(root);

            existingParts = LoadAssets<TrainPartSO>();
            typesRelations = LoadAssets<TrainPartTypeRelationsSO>();
            trainPartsSO = AssetDatabase.LoadAssetAtPath<TrainPartsSO>(TRAIN_PARTS_SO_PATH);

            idTextField = rootVisualElement.Q<TextField>(ID_TEXT_FIELD_NAME);

            typeDropdown = rootVisualElement.Q<EnumField>(TYPE_DROPDOWN_NAME);
            typeDropdown.RegisterValueChangedCallback((evt) => { CheckTypeSubtypes(evt.newValue); });
            subtypeDropdown = rootVisualElement.Q<EnumField>(SUBTYPE_DROPDOWN_NAME);

            mainTextureField = rootVisualElement.Q<ObjectField>(MAIN_TEXTURE_FIELD_NAME);
            outlineTextureField = rootVisualElement.Q<ObjectField>(OUTLINE_TEXTURE_FIELD_NAME);
            errorLabel = rootVisualElement.Q<Label>(ERROR_LABEL_NAME);

            createButton = rootVisualElement.Q<Button>(CREATE_BUTTON_NAME);
            createButton.RegisterCallback<ClickEvent>((evt) => CreateTrainPart());

            existingPartsList = rootVisualElement.Q<ScrollView>(EXISTING_PARTS_LIST_NAME);
            foreach (TrainPartSO _part in existingParts)
            {
                existingPartsList.Add(new Label(_part.Id));
            }

            CheckTypeSubtypes(typeDropdown.value);
        }

        private List<T> LoadAssets<T>() where T : UnityEngine.Object
        {
            string[] _assets = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            List<T> _result = new List<T>();
            foreach (string _asset in _assets)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_asset);
                T _loadedAsset = AssetDatabase.LoadAssetAtPath<T>(_path);
                _result.Add(_loadedAsset);
            }

            return _result;
        }

        private void CheckTypeSubtypes(Enum _newValue)
        {
            TrainPartType _type = (TrainPartType)_newValue;
            TrainPartTypeRelationsSO _typeRelations = typesRelations.Find((_relations) => _relations.Type == _type);
            if (_typeRelations != null && _typeRelations.SubTypes.Count > 0)
            {
                subtypeDropdown.SetEnabled(true);
                subtypeDropdown.value = _typeRelations.SubTypes[0];
                subtypeDropdown.style.display = DisplayStyle.Flex;
            }
            else
            {
                subtypeDropdown.SetEnabled(false);
                subtypeDropdown.style.display = DisplayStyle.None;
            }
        }

        private void CreateTrainPart()
        {
            if (!ValidateInput())
            {
                return;
            }

            TrainPartSO _trainPart = CreateInstance<TrainPartSO>();
            _trainPart.Setup(
                idTextField.value, 
                (TrainPartType)typeDropdown.value, 
                (TrainPartSubtype)subtypeDropdown.value,
                (Sprite)mainTextureField.value,
                (Sprite)outlineTextureField.value);

            AssetDatabase.CreateAsset(_trainPart, $"{PARTS_FOLDER}/{_trainPart.Id}.asset");
            AssetDatabase.SaveAssets();

            trainPartsSO.AddPart(_trainPart);

            existingParts.Add(_trainPart);
            existingPartsList.Add(new Label(_trainPart.Id));

            idTextField.value = string.Empty;
            mainTextureField.value = null;
            outlineTextureField.value = null;
        }

        private bool ValidateInput()
        {
            bool _result = true;
            errorLabel.text = string.Empty;

            if (idTextField.value == string.Empty)
            {
                errorLabel.text += "ID is empty\n";
                _result = false;
            }

            if (existingParts.Find((_part) => _part.Id == idTextField.value) != null)
            {
                errorLabel.text += "ID already exists\n";
                _result = false;
            }

            if (mainTextureField.value == null)
            {
                errorLabel.text += "Main texture is not set\n";
                _result = false;
            }

            if (outlineTextureField.value == null)
            {
                errorLabel.text += "Outline texture is not set\n";
                _result = false;
            }

            return _result;
        }
    }
}
#endif