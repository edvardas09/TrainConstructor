using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrainConstructor.TrainData
{
    public class TrainDataManager : Singleton<TrainDataManager>
    {
        public CreatedTrainsSO CreatedTrainsSO { get; private set; }

        public List<TrainPartSO> TrainParts { get; } = new List<TrainPartSO>();
        public List<Train> CreatedTrains { get; private set; } = new List<Train>();

        public Train SelectedTrain { get; private set; }
        public bool IsRandom { get; private set; }

        public void SetCreatedTrainsSO(CreatedTrainsSO _createdTrainsSO)
        {
            CreatedTrainsSO = _createdTrainsSO;
            CreatedTrains = _createdTrainsSO.CreatedTrains;
        }

        public void SetSelectedTrain(Train _train)
        {
            if (_train == null)
            {
                IsRandom = true;
                SelectedTrain = CreatedTrains[Random.Range(0, CreatedTrains.Count)];
                return;
            }

            IsRandom = false;
            SelectedTrain = _train;
        }

        public List<TrainPartSO> LoadTrainParts()
        {
            TrainParts.Clear();
            string[] _assets = AssetDatabase.FindAssets("t:" + nameof(TrainPartSO));
            foreach (string _asset in _assets)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_asset);
                TrainPartSO _loadedAsset = AssetDatabase.LoadAssetAtPath<TrainPartSO>(_path);
                TrainParts.Add(_loadedAsset);
            }

            return TrainParts;
        }

        public void SaveCreatedTrains()
        {
            CreatedTrainsSO.CreatedTrains = CreatedTrains;
            EditorUtility.SetDirty(CreatedTrainsSO);
            AssetDatabase.SaveAssets();
        }
    }
}