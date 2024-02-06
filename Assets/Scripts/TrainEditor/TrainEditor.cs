using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private ToolsView toolsView;
        [SerializeField] private GameObject SideUIObject;

        [Header("Train part selection references")]
        [SerializeField] private TrainPartSelection trainPartSelectionPrefab;
        [SerializeField] private Transform trainPartSelectionsParent;

        [Header("Train part references")]
        [SerializeField] private TrainPart trainPartPrefab;
        [SerializeField] private Train.Train trainObject;

        public static TrainEditor Instance { get; private set; }

        private List<TrainPartSO> trainParts = new List<TrainPartSO>();
        private int offsetMultiplier;
        private bool isOverDeleteObject;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            deletePartObject.MouseOverStateChanged += OnDeleteStateChanged;

            LoadTrainParts();
            ValidateParts();
            SpawnPartSelections();
        }

        public void ResetEditor()
        {
            foreach (Transform _child in trainObject.transform)
            {
                Destroy(_child.gameObject);
            }

            offsetMultiplier = 0;
            trainObject.Setup(string.Empty, false);
        }

        public void SaveTrain(string _trainId, bool _isLockedWithAnAd)
        {
            trainObject.Setup(_trainId, _isLockedWithAnAd);
            string _path = $"{Paths.CREATED_TRAINS_PATH}/{_trainId}.prefab";
            PrefabUtility.SaveAsPrefabAsset(trainObject.gameObject, _path);
            AssetDatabase.Refresh();
        }

        public void TakeSnapshot()
        {
            StartCoroutine(CaptureScreenshot());
        }

        public void DeleteTrain()
        {
            foreach (Transform _child in trainObject.transform)
            {
                Destroy(_child.gameObject);
            }

            if (trainObject.Id == string.Empty)
            {
                return;
            }

            //TODO: this wqon't work, change
            string _path = AssetDatabase.GetAssetPath(trainObject);
            AssetDatabase.DeleteAsset(_path);
            AssetDatabase.Refresh();
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
            toolsView.gameObject.SetActive(false);
            SideUIObject.SetActive(false);

            yield return new WaitForEndOfFrame();
            string _path = $"{Application.dataPath}/{Paths.SNAPSHOTS_PATH}/snapshot.png";
            ScreenCapture.CaptureScreenshot(_path);
            Debug.Log("Snapshot saved to " + _path);

            toolsView.gameObject.SetActive(true);
            SideUIObject.SetActive(true);
        }
    }
}