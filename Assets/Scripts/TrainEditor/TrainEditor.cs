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

        public Train.Train TrainObject => trainObject;

        private List<TrainPartSO> trainParts = new List<TrainPartSO>();
        private List<Train.Train> createdTrains = new List<Train.Train>();

        private Train.Train trainObject;
        private int offsetMultiplier;
        private bool isOverDeleteObject;

        private void Awake()
        {
            deletePartObject.MouseOverStateChanged += OnDeleteStateChanged;

            TrainDataManager.Instance.LoadSnapshots();
            createdTrains = TrainDataManager.Instance.LoadCreatedTrains();
            trainParts = TrainDataManager.Instance.LoadTrainParts();

            CreatedTrainsLoaded?.Invoke(TrainDataManager.Instance.CreatedTrains);

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
                DraggablePart _trainPartComponent = _child.GetComponent<DraggablePart>();
                _trainPartComponent.SetOrder(_order);
                _trainPartComponent.PartPutDown += OnPartPutDown;
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

            Texture2D _snapshot = TrainDataManager.Instance.GetTrainSnapshot(trainObject.Id);
            if (_snapshot != null)
            {
                string _path = AssetDatabase.GetAssetPath(_snapshot);
                AssetDatabase.DeleteAsset(_path);
                AssetDatabase.Refresh();
            }

            ResetEditor();
        }

        private void CreateNewTrain()
        {
            trainObject = new GameObject(DEFAULT_TRAIN_OBJECT_NAME).AddComponent<Train.Train>();
            trainObject.transform.SetParent(trainObjectParent);
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
            DraggablePart _trainPartComponent = _trainPart.GetComponent<DraggablePart>();
            _trainPart.Setup(_trainPartSO);
            _trainPartComponent.Setup(_trainPart, _order);
            _trainPartComponent.PartPutDown += OnPartPutDown;
        }

        private void OnPartPutDown(TrainPart _trainPart)
        {
            if (isOverDeleteObject)
            {
                Destroy(_trainPart.gameObject);
                return;
            }

            offsetMultiplier++;
            DraggablePart _trainPartComponent = _trainPart.GetComponent<DraggablePart>();
            _trainPartComponent.OffsetPart(offsetMultiplier);
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

            TrainDataManager.Instance.LoadSnapshots();
            SnapshotUpdated?.Invoke(trainObject, TrainDataManager.Instance.GetTrainSnapshot(trainObject.Id));
        }

    }
}