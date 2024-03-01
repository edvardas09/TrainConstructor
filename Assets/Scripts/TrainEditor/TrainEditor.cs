#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrainConstructor.TrainData;
using UnityEditor;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class TrainEditor : MonoBehaviour
    {
        public static TrainEditor Instance;

        [SerializeField] private CreatedTrainsSO createdTrainsSO;
        [SerializeField] private TrainPartsSO trainPartsSO;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TrainUIManager trainUIManager;

        [Tooltip("The order of the train parts in the editor, add only parts with different types or subtypes")]
        [SerializeField] private List<TrainPartSO> trainPartsOrder = new List<TrainPartSO>();

        [Header("UI References")]
        [SerializeField] private DeletePartObject deletePartObject;
        [SerializeField] private GameObject canvasObject;
        [SerializeField] private GameObject snapshotCanvasObject;

        [Header("Train part selection references")]
        [SerializeField] private TrainPartSelection trainPartSelectionPrefab;
        [SerializeField] private Transform trainPartSelectionsParent;

        [Header("Train part references")]
        [SerializeField] private TrainPart trainPartPrefab;
        [SerializeField] private Transform trainObjectParent;

        [Header("Snapshot references")]
        [SerializeField] private Camera snapshotCamera;
        [SerializeField] private RenderTexture snapshotRenderTexture;

        private const string DEFAULT_TRAIN_OBJECT_NAME = "Train";

        public Train TrainObject => trainObject;

        private List<TrainPartSO> trainParts = new List<TrainPartSO>();
        private List<Train> createdTrains = new List<Train>();

        private Train trainObject;
        private int offsetMultiplier;
        private bool isOverDeleteObject;

        private void OnValidate()
        {
            if (mainCamera == null && Camera.main != null)
            {
                mainCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            deletePartObject.MouseOverStateChanged += OnDeleteStateChanged;
        }

        private void OnDisable()
        {
            deletePartObject.MouseOverStateChanged -= OnDeleteStateChanged;
        }

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
        }

        private void Start()
        {
            TrainDataManager.Instance.SetCreatedTrainsSO(createdTrainsSO, trainPartsSO);
            createdTrains = TrainDataManager.Instance.CreatedTrains;
            trainParts = TrainDataManager.Instance.TrainParts;

            ValidateParts();

            CreateNewTrain();
            SpawnPartSelections();

            trainUIManager.SpawnCreatedTrainButtons(TrainDataManager.Instance.CreatedTrains);
        }

        public void LoadTrain(Train _train)
        {
            Destroy(trainObject.gameObject);
            trainObject = Instantiate(_train, trainObjectParent);
            offsetMultiplier = trainObject.transform.childCount;

            foreach (TrainPart _part in trainObject.PartList)
            {
                DraggablePart _trainPartComponent = _part.gameObject.GetComponent<DraggablePart>();
                _trainPartComponent.enabled = true;
                int _order = trainPartsOrder.FindIndex(_partOrder => _partOrder.Type == _part.TrainPartSO.Type && _partOrder.SubType == _part.TrainPartSO.SubType);
                _trainPartComponent.Setup(_part, _order, mainCamera, false);
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
            StartCoroutine(CaptureScreenshot(_trainId, _isLockedWithAnAd));
        }

        private void SetupNewTrain(string _trainId, bool _isLockedWithAnAd)
        {
            Sprite _snapshot = AssetDatabase.LoadAssetAtPath<Sprite>($"{Paths.SNAPSHOTS_PATH}/{_trainId}.png");
            trainObject.Setup(_trainId, _isLockedWithAnAd, _snapshot);
            string _path = $"{Paths.CREATED_TRAINS_PATH}/{_trainId}.prefab";

            foreach (TrainPart _part in trainObject.PartList)
            {
                DraggablePart _partComponent = _part.gameObject.GetComponent<DraggablePart>();
                _partComponent.enabled = false;
            }

            GameObject _savedTrain = PrefabUtility.SaveAsPrefabAsset(trainObject.gameObject, _path);
            AssetDatabase.Refresh();

            foreach (TrainPart _part in trainObject.PartList)
            {
                DraggablePart _partComponent = _part.gameObject.GetComponent<DraggablePart>();
                _partComponent.enabled = true;
            }

            if (createdTrains.Any(_train => _train.Id == trainObject.Id))
            {
                return;
            }

            Train _savedTrainComponent = _savedTrain.GetComponent<Train>();
            createdTrains.Add(_savedTrainComponent);
            SaveCreatedTrainsSO();
            trainUIManager.TrainAdded(_savedTrainComponent);
            TrainDataManager.Instance.SaveCreatedTrains();
        }

        public void DeleteTrain()
        {
            if (!string.IsNullOrWhiteSpace(trainObject.Id))
            {
                Train _savedTrain = createdTrains.Find(_train => _train.Id == trainObject.Id);

                string _path = AssetDatabase.GetAssetPath(_savedTrain);
                AssetDatabase.DeleteAsset(_path);
                AssetDatabase.Refresh();

                createdTrains.Remove(_savedTrain);
                SaveCreatedTrainsSO();
                trainUIManager.TrainDeleted(_savedTrain);
            }

            Sprite _snapshot = trainObject.Snapshot;
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
            trainObject = new GameObject(DEFAULT_TRAIN_OBJECT_NAME).AddComponent<Train>();
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
            _trainPartComponent.Setup(_trainPart, _order, mainCamera, true);
            _trainPartComponent.PartPutDown += OnPartPutDown;
            TrainObject.AddPart(_trainPart);
        }

        private void OnPartPutDown(TrainPart _trainPart)
        {
            if (isOverDeleteObject)
            {
                TrainObject.RemovePart(_trainPart);
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

        private IEnumerator CaptureScreenshot(string _trainId, bool _isLockedWithAnAd)
        {
            canvasObject.SetActive(false);
            snapshotCanvasObject.SetActive(false);

            yield return new WaitForEndOfFrame();
            string _path = $"{Directory.GetCurrentDirectory()}/{Paths.SNAPSHOTS_PATH}/{_trainId}.png";

            snapshotCamera.Render();
            RenderTexture.active = snapshotRenderTexture;
            Texture2D _snapshot = new Texture2D(snapshotRenderTexture.width, snapshotRenderTexture.height, TextureFormat.RGB24, false);
            _snapshot.ReadPixels(new Rect(0, 0, snapshotRenderTexture.width, snapshotRenderTexture.height), 0, 0);
            _snapshot.Apply();

            byte[] _bytes = _snapshot.EncodeToPNG();
            File.WriteAllBytes(_path, _bytes);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            TextureImporter _textureImporter = (TextureImporter)AssetImporter.GetAtPath($"{Paths.SNAPSHOTS_PATH}/{_trainId}.png");
            _textureImporter.textureType = TextureImporterType.Sprite;
            _textureImporter.SaveAndReimport();

            Debug.Log("Snapshot saved to " + _path);

            canvasObject.SetActive(true);
            snapshotCanvasObject.SetActive(true);

            trainUIManager.SnapshotUpdated(trainObject);

            SetupNewTrain(_trainId, _isLockedWithAnAd);
        }

        private void SaveCreatedTrainsSO()
        {
            createdTrainsSO.CreatedTrains = createdTrains;
            EditorUtility.SetDirty(createdTrainsSO);
            AssetDatabase.SaveAssets();
        }

    }
}
#endif