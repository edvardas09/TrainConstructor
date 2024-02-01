using System.Collections.Generic;
using System.Linq;
using TrainConstructor.Train;
using UnityEditor;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class TrainEditorManager : MonoBehaviour
    {
        [Tooltip("The order of the train parts in the editor, add only parts with different types or subtypes")]
        [SerializeField] private List<TrainPartSO> trainPartsOrder = new List<TrainPartSO>();

        [SerializeField] private TrainPartSelection trainPartSelectionPrefab;
        [SerializeField] private Transform trainPartSelectionsParent;

        [SerializeField] private TrainPart trainPartPrefab;
        [SerializeField] private Transform trainPartsParent;

        private List<TrainPartSO> trainParts = new List<TrainPartSO>();

        private void Awake()
        {
            LoadTrainParts();
            ValidateParts();
            SpawnPartSelections();
        }

        private void LoadTrainParts()
        {
            trainParts.Clear();
            string[] _assets = AssetDatabase.FindAssets("t:" + typeof(TrainPartSO).Name);
            foreach (string _asset in _assets)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_asset);
                TrainPartSO _loadedAsset = AssetDatabase.LoadAssetAtPath<TrainPartSO>(_path);
                trainParts.Add(_loadedAsset);
            }
        }

        private void ValidateParts()
        {
            foreach (TrainPartSO _trainPartSO in trainParts)
            {
                if (!IsTrainPartInOrder(_trainPartSO))
                {
                    Debug.LogError($"Train part {_trainPartSO.name} type is not added in order list, display may look wrong");
                }
            }
        }

        private bool IsTrainPartInOrder(TrainPartSO _trainPartSO)
        {
            return trainPartsOrder.Any(_partOrder => _partOrder.Type == _trainPartSO.Type && _partOrder.SubType == _trainPartSO.SubType);
        }

        private void SpawnPartSelections()
        {
            foreach (TrainPartSO _trainPartSO in trainParts)
            {
                TrainPartSelection _trainPartSelection = Instantiate(trainPartSelectionPrefab, trainPartSelectionsParent);
                _trainPartSelection.Setup(_trainPartSO);
                _trainPartSelection.TrainPartSelected += OnTrainPartSelected;
            }
        }

        private void OnTrainPartSelected(TrainPartSO _trainPartSO)
        {
            TrainPart _trainPart = Instantiate(trainPartPrefab, trainPartsParent);
            int _order = trainPartsOrder.FindIndex(_partOrder => _partOrder.Type == _trainPartSO.Type && _partOrder.SubType == _trainPartSO.SubType);
            _trainPart.Setup(_trainPartSO, _order);
        }
    }
}