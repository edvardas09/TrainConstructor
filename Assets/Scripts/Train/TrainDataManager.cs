using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TrainConstructor.Train
{
    public class TrainDataManager : Singleton<TrainDataManager>
    {
        public List<TrainPartSO> TrainParts => trainParts;
        public List<Train> CreatedTrains => createdTrains;
        public List<Texture2D> Snapshots => snapshots;

        public Train SelectedTrain => selectedTrain;
        public bool IsRandom => isRandom;


        private List<TrainPartSO> trainParts = new List<TrainPartSO>();
        private List<Train> createdTrains = new List<Train>();
        private List<Texture2D> snapshots = new List<Texture2D>();

        private Train selectedTrain;
        private bool isRandom;

        public void SetSelectedTrain(Train _train)
        {
            isRandom = false;
            selectedTrain = _train;
        }

        public void SetRandomTrain()
        {
            isRandom = true;
            selectedTrain = createdTrains[Random.Range(0, createdTrains.Count)];
        }

        public List<TrainPartSO> LoadTrainParts()
        {
            trainParts.Clear();
            string[] _assets = AssetDatabase.FindAssets("t:" + typeof(TrainPartSO).Name);
            foreach (string _asset in _assets)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_asset);
                TrainPartSO _loadedAsset = AssetDatabase.LoadAssetAtPath<TrainPartSO>(_path);
                trainParts.Add(_loadedAsset);
            }

            return trainParts;
        }

        public List<Train> LoadCreatedTrains()
        {
            createdTrains.Clear();
            string[] _files = Directory.GetFiles(Paths.CREATED_TRAINS_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
            foreach (string _file in _files)
            {
                GameObject _trainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_file);
                Train _train = _trainPrefab.GetComponent<Train>();
                createdTrains.Add(_train);
            }

            return createdTrains;
        }

        public List<Texture2D> LoadSnapshots()
        {
            snapshots.Clear();
            string[] _files = Directory.GetFiles(Paths.SNAPSHOTS_PATH, "*.png", SearchOption.TopDirectoryOnly);
            foreach (string _file in _files)
            {
                Texture2D _snapshot = AssetDatabase.LoadAssetAtPath<Texture2D>(_file);
                snapshots.Add(_snapshot);
            }

            return snapshots;
        }

        public Texture2D GetTrainSnapshot(string _trainId)
        {
            return snapshots.Find(_snapshot => _snapshot.name == _trainId);
        }
    }
}