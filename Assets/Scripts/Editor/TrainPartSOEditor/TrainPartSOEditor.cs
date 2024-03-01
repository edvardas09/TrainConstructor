#if UNITY_EDITOR
using System.Collections.Generic;
using TrainConstructor.TrainData;
using UnityEditor;
using UnityEngine;

namespace TrainConstructor.Editor
{
    [CustomEditor(typeof(TrainPartSO))]
    [CanEditMultipleObjects]
    public class TrainPartSOEditor : UnityEditor.Editor
    {
        private TrainPartSO trainPartSO;

        private SerializedProperty type;
        private SerializedProperty subType;
        private SerializedProperty mainTexture;
        private SerializedProperty outlineTexture;

        private Sprite previousMainTexture;
        private Sprite previousOutlineTexture;

        private readonly List<TrainPartTypeRelationsSO> typesRelations = new List<TrainPartTypeRelationsSO>();

        void OnEnable()
        {
            trainPartSO = (TrainPartSO)target;

            type = serializedObject.FindProperty("Type");
            subType = serializedObject.FindProperty("SubType");
            mainTexture = serializedObject.FindProperty("MainTexture");
            outlineTexture = serializedObject.FindProperty("OutlineTexture");

            previousMainTexture = trainPartSO.MainTexture;
            previousOutlineTexture = trainPartSO.OutlineTexture;

            string[] _assets = AssetDatabase.FindAssets("t:" + typeof(TrainPartTypeRelationsSO).Name);
            foreach (string _asset in _assets)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_asset);
                TrainPartTypeRelationsSO _loadedAsset = AssetDatabase.LoadAssetAtPath<TrainPartTypeRelationsSO>(_path);
                typesRelations.Add(_loadedAsset);
            }
        }

        public override void OnInspectorGUI()
        {
            if (trainPartSO.MainTexture == null)
            {
                trainPartSO.MainTexture = previousMainTexture;
                Debug.LogError("Main texture cannot be null");
            }
            if (trainPartSO.OutlineTexture == null)
            {
                trainPartSO.OutlineTexture = previousOutlineTexture;
                Debug.LogError("Outline texture cannot be null");
            }

            previousMainTexture = trainPartSO.MainTexture;
            previousOutlineTexture = trainPartSO.OutlineTexture;

            serializedObject.Update();

            EditorGUILayout.PropertyField(type);

            TrainPartTypeRelationsSO _typeRelations = typesRelations.Find((_relations) => _relations.Type == trainPartSO.Type);
            if (_typeRelations != null && _typeRelations.SubTypes.Count > 0)
            {
                EditorGUILayout.PropertyField(subType);
            }
            else
            {
                trainPartSO.SubType = default;
            }

            EditorGUILayout.PropertyField(mainTexture);
            EditorGUILayout.PropertyField(outlineTexture);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif