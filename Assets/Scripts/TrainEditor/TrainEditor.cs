using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrainConstructor.Train;
using UnityEditor;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class TrainEditor : MonoBehaviour
    {
        [Tooltip("The order of the train parts in the editor, add only parts with different types or subtypes")]
        [SerializeField] private List<TrainPartSO> trainPartsOrder = new List<TrainPartSO>();

        [Header("UI References")]
        [SerializeField] private DeletePartObject deletePartObject;
        [SerializeField] private GameObject canvasObject;

        [Header("Train part selection references")]
        [SerializeField] private TrainPartSelection trainPartSelectionPrefab;
        [SerializeField] private Transform trainPartSelectionsParent;

        [Header("Train part references")]
        [SerializeField] private TrainPart trainPartPrefab;
        [SerializeField] private Transform trainObjectParent;

        private const string DEFAULT_TRAIN_OBJECT_NAME = "Train";


        public Action<List<Train.Train>> CreatedTrainsLoaded;
        public Action<Train.Train> TrainAdded;
        public Action<Train.Train> TrainDeleted;
        public Action<Train.Train, Texture2D> SnapshotUpdated;

        public List<Train.Train> CreatedTrains => createdTrains;
        public Train.Train TrainObject => trainObject;


        private Train.Train trainObject;

        private List<TrainPartSO> trainParts = new List<TrainPartSO>();
        private List<Train.Train> createdTrains = new List<Train.Train>();
        private List<Texture2D> snapshots = new List<Texture2D>();

        private int offsetMultiplier;
        private bool isOverDeleteObject;

        private void Awake()
        {
            deletePartObject.MouseOverStateChanged += OnDeleteStateChanged;

            LoadSnapshots();
            LoadCreatedTrains();
            LoadTrainParts();

            ValidateParts();

            CreateNewTrain();
            SpawnPartSelections();
        }

        public void LoadTrain(Train.Train _train)
        {
            Destroy(trainObject.gameObject);
            trainObject = Instantiate(_train, trainObjectParent);
            offsetMultiplier = trainObject.transform.childCount;

            foreach (Transform _child in trainObject.transform)
            {
                TrainPart _trainPart = _child.GetComponent<TrainPart>();
                int _order = trainPartsOrder.FindIndex(_partOrder => _partOrder.Type == _trainPart.TrainPartSO.Type && _partOrder.SubType == _trainPart.TrainPartSO.SubType);
                _trainPart.SetOrder(_order);
                _trainPart.PartPutDown += OnPartPutDown;
            }
        }

        public void ResetEditor()
        {
            Destroy(trainObject.gameObject);
            CreateNewTrain();
            offsetMultiplier = 0;
        }

        public void SaveTrain(string _trainId, bool _isLockedWithAnAd)
        {
            trainObject.Setup(_trainId, _isLockedWithAnAd);
            string _path = $"{Paths.CREATED_TRAINS_PATH}/{_trainId}.prefab";
            GameObject _savedTrain = PrefabUtility.SaveAsPrefabAsset(trainObject.gameObject, _path);
            AssetDatabase.Refresh();

            if (createdTrains.Any(_train => _train.Id == trainObject.Id))
            {
                return;
            }

            Train.Train _savedTrainComponent = _savedTrain.GetComponent<Train.Train>();
            createdTrains.Add(_savedTrainComponent);
            TrainAdded?.Invoke(_savedTrainComponent);
        }

        public void TakeSnapshot()
        {
            StartCoroutine(CaptureScreenshot());
        }

        public void DeleteTrain()
        {
            if (!string.IsNullOrWhiteSpace(trainObject.Id))
            {
                Train.Train _savedTrain = createdTrains.Find(_train => _train.Id == trainObject.Id);

                string _path = AssetDatabase.GetAssetPath(_savedTrain);
                AssetDatabase.DeleteAsset(_path);
                AssetDatabase.Refresh();

                createdTrains.Remove(_savedTrain);
                TrainDeleted?.Invoke(_savedTrain);
            }

            Texture2D _snapshot = GetTrainSnapshot(trainObject.Id);
            if (_snapshot != null)
            {
                string _path = AssetDatabase.GetAssetPath(_snapshot);
                AssetDatabase.DeleteAsset(_path);
                AssetDatabase.Refresh();
            }

            ResetEditor();
        }

        public Texture2D GetTrainSnapshot(string _trainId)
        {
            return snapshots.Find(_snapshot => _snapshot.name == _trainId);
        }

        private void CreateNewTrain()
        {
            trainObject = new GameObject(DEFAULT_TRAIN_OBJECT_NAME).AddComponent<Train.Train>();
            trainObject.transform.SetParent(trainObjectParent);
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

        private void LoadCreatedTrains()
        {
            createdTrains.Clear();
            string[] _files = Directory.GetFiles(Paths.CREATED_TRAINS_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
            foreach (string _file in _files)
            {
                GameObject _trainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_file);
                Train.Train _train = _trainPrefab.GetComponent<Train.Train>();
                createdTrains.Add(_train);
            }

            CreatedTrainsLoaded?.Invoke(createdTrains);
        }

        private void LoadSnapshots()
        {
            snapshots.Clear();
            string[] files = Directory.GetFiles(Paths.SNAPSHOTS_PATH, "*.png", SearchOption.TopDirectoryOnly);
            foreach (string _file in files)
            {
                Texture2D _snapshot = AssetDatabase.LoadAssetAtPath<Texture2D>(_file);
                snapshots.Add(_snapshot);
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
            TrainPart _trainPart = Instantiate(trainPartPrefab, trainObject.transform);
            int _order = trainPartsOrder.FindIndex(_partOrder => _partOrder.Type == _trainPartSO.Type && _partOrder.SubType == _trainPartSO.SubType);
            _trainPart.Setup(_trainPartSO, _order);
            _trainPart.PartPutDown += OnPartPutDown;
        }

        private void OnPartPutDown(TrainPart _trainPart)
        {
            if (isOverDeleteObject)
            {
                Destroy(_trainPart.gameObject);
                return;
            }

            offsetMultiplier++;
            _trainPart.OffsetPart(offsetMultiplier);
        }

        private void OnDeleteStateChanged()
        {
            isOverDeleteObject = !isOverDeleteObject;
        }

        private IEnumerator CaptureScreenshot()
        {
            canvasObject.SetActive(false);

            yield return new WaitForEndOfFrame();
            string _path = $"{Directory.GetCurrentDirectory()}/{Paths.SNAPSHOTS_PATH}/{trainObject.Id}.png";
            ScreenCapture.CaptureScreenshot(_path);
            yield return new WaitForSeconds(0.5f);
            AssetDatabase.Refresh();

            Debug.Log("Snapshot saved to " + _path);

            canvasObject.SetActive(true);

            LoadSnapshots();
            SnapshotUpdated?.Invoke(trainObject, GetTrainSnapshot(trainObject.Id));
        }

    }
}