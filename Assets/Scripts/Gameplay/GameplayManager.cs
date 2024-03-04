using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrainConstructor.Shared;
using TrainConstructor.TrainData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrainConstructor.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private int maxTrainParts = 5;
        [SerializeField] private Button backButton;
        [SerializeField] private TrainPartOption trainPartOptionPrefab;
        [SerializeField] private Transform trainPartOptionsParent;
        [SerializeField] private ParticleSystem levelFinishedParticles;
        [SerializeField] private ParticleSystem partPlacedParticles;

        private Train spawnedTrain;
        private List<TrainPart> trainParts = new List<TrainPart>();
        private readonly List<TrainPart> availablePartSelections = new List<TrainPart>();
        private readonly List<TrainPartOption> spawnedPartOptions = new List<TrainPartOption>();

        private Vector3 partScale;

        private void OnValidate()
        {
            if (mainCamera == null && Camera.main != null)
            {
                mainCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            backButton.onClick.AddListener(LoadTrainSelectionScene);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(LoadTrainSelectionScene);
        }

        private void Start()
        {
            SetupSelectedTrain();
            SpawnInitialPartSelections();
        }

        private void SetupSelectedTrain()
        {
            //spawn selected train
            Train _selectedTrain = TrainDataManager.Instance.SelectedTrain;
            if (_selectedTrain == null)
            {
                Debug.LogWarning("No train selected, opening train selection scene");
                SceneManager.LoadScene(Scenes.TrainSelection.ToString());
                return;
            }

            spawnedTrain = Instantiate(_selectedTrain, Vector3.zero, Quaternion.identity);
            SetupTrainParts();
            PositionTrain();
        }

        private void PositionTrain()
        {
            Vector2 _padding = new Vector2(0.25f, 0.25f);
            Bounds _bounds = spawnedTrain.GetBounds();

            float _maxWidth = Vector3.Distance(mainCamera.ViewportToWorldPoint(new Vector3(_padding.x, 0, 0)),
                mainCamera.ViewportToWorldPoint(new Vector3(1f - _padding.x, 0, 0)));
            float _maxHeight = Vector3.Distance(mainCamera.ViewportToWorldPoint(new Vector3(0, _padding.y, 0)),
                               mainCamera.ViewportToWorldPoint(new Vector3(0, 1f - _padding.y, 0)));

            float _scale = Mathf.Min(_maxWidth / _bounds.size.x, _maxHeight / _bounds.size.y);
            spawnedTrain.transform.localScale = Vector3.one * _scale;

            //center the train
            Vector3 _center = _bounds.center;
            Vector3 _newPosition = -_center * _scale;
            _newPosition.y += 2;
            spawnedTrain.transform.position = _newPosition;
            partScale = trainParts[0].transform.localScale * _scale;
        }

        private void SetupTrainParts()
        {
            trainParts = spawnedTrain.PartList;
            if (trainParts.Count == 0)
            {
                Debug.LogError("No parts found in the selected train");
                return;
            }

            availablePartSelections.AddRange(trainParts);
            foreach (TrainPart _part in trainParts)
            {
                if (TrainDataManager.Instance.IsRandom)
                {
                    _part.Randomize();
                }

                _part.ShowOutlineTexture();
            }
        }

        private void SpawnInitialPartSelections()
        {
            if (availablePartSelections.Count == 0)
            {
                return;
            }

            int _initialPartCount = Mathf.Min(maxTrainParts, availablePartSelections.Count);
            for (int i = 0; i < _initialPartCount; i++)
            {
                TrainPart _part = availablePartSelections[Random.Range(0, availablePartSelections.Count)];
                availablePartSelections.Remove(_part);
                TrainPartOption _partSelection = Instantiate(trainPartOptionPrefab, trainPartOptionsParent);
                _partSelection.Setup(_part.TrainPartSO, partScale, mainCamera);
                _partSelection.OnTrainPartReleased += OnTrainPartReleased;

                spawnedPartOptions.Add(_partSelection);
            }
        }

        private void OnTrainPartReleased(TrainPartOption _trainPartOption)
        {
            TrainPart _trainPart = _trainPartOption.HoveredTrainPart;
            if (_trainPart == null)
            {
                return;
            }

            _trainPart.Setup(_trainPartOption.TrainPartSO);
            _trainPart.ShowMainTexture();
            _trainPart.BoxCollider2D.enabled = false;
            trainParts.Remove(_trainPart);

            ShowPartPlacedParticle(_trainPart.transform);
            SetupNextPart(_trainPartOption);
        }

        private void SetupNextPart(TrainPartOption _trainPartOption)
        {
            if (availablePartSelections.Count == 0)
            {
                spawnedPartOptions.Remove(_trainPartOption);
                Destroy(_trainPartOption.gameObject);

                if (spawnedPartOptions.Count == 0)
                {
                    StartCoroutine(LevelFinished());
                }

                return;
            }

            _trainPartOption.HidePart();
            TrainPart _nextPart = availablePartSelections[Random.Range(0, availablePartSelections.Count)];
            _trainPartOption.Setup(_nextPart.TrainPartSO, partScale, mainCamera);
            availablePartSelections.Remove(_nextPart);
        }

        private List<TrainPart> GetPartsOfType(TrainPartSO _trainPartSO)
        {
            return trainParts.FindAll(x => x.TrainPartSO.Type == _trainPartSO.Type && x.TrainPartSO.SubType == _trainPartSO.SubType);
        }

        private IEnumerator LevelFinished()
        {
            levelFinishedParticles.Play();
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(Scenes.TrainSelection.ToString());
        }

        private void ShowPartPlacedParticle(Transform _partTransform)
        {
            partPlacedParticles.transform.localScale = _partTransform.localScale;
            partPlacedParticles.transform.position = _partTransform.position;
            partPlacedParticles.Play();
        }

        private void LoadTrainSelectionScene()
        {
            SceneManager.LoadScene(Scenes.TrainSelection.ToString());
        }
    }
}