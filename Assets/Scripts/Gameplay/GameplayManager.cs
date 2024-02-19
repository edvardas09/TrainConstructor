using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrainConstructor.Shared;
using TrainConstructor.Train;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrainConstructor.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private int maxTrainParts = 5;
        [SerializeField] private GameObject trainPartOptionPrefab;
        [SerializeField] private Transform trainPartOptionsParent;
        [SerializeField] private ParticleSystem levelFinishedParticles;

        private Train.Train spawnedTrain;
        private List<TrainPart> trainParts = new List<TrainPart>();
        private readonly List<TrainPart> availablePartSelections = new List<TrainPart>();
        private readonly List<GameObject> spawnedPartOptions = new List<GameObject>();

        private Vector3 partScale;

        private void Start()
        {
            SetupSelectedTrain();
            SpawnInitialPartSelections();
        }

        private void SetupSelectedTrain()
        {
            //spawn selected train
            Train.Train _selectedTrain = TrainDataManager.Instance.SelectedTrain;
            if (_selectedTrain == null)
            {
                Debug.LogWarning("No train selected, opening train selection scene");
                SceneManager.LoadScene(Scenes.TrainSelection.ToString());
                return;
            }

            spawnedTrain = Instantiate(_selectedTrain, Vector3.zero, Quaternion.identity);

            //remove DraggablePart component from all parts
            trainParts = spawnedTrain.GetComponentsInChildren<TrainPart>().ToList();
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

                Destroy(_part.GetComponent<DraggablePart>());
                _part.ShowOutlineTexture();
                _part.GetComponent<Collider2D>().enabled = false;
            }

            //fit loaded train to the screen
            Vector2 _padding = new Vector2(0.25f, 0.25f);
            Bounds _bounds = spawnedTrain.GetBounds();

            float _maxWidth = Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(_padding.x, 0.5f, 0)),
                Camera.main.ViewportToWorldPoint(new Vector3(1f - _padding.x, 0.5f, 0)));
            float _maxHeight = Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, _padding.y, 0)),
                               Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f - _padding.y, 0)));

            float _scale = Mathf.Min(_maxWidth / _bounds.size.x, _maxHeight / _bounds.size.y);
            spawnedTrain.transform.localScale = Vector3.one * _scale;

            //center the train
            Vector3 _center = _bounds.center;
            Vector3 _newPosition = -_center * _scale;
            _newPosition.y += 2;
            spawnedTrain.transform.position = _newPosition;

            partScale = trainParts[0].transform.localScale * _scale;
        }

        private void SpawnInitialPartSelections()
        {
            if (availablePartSelections.Count == 0)
            {
                return;
            }

            for (int i = 0; i < maxTrainParts; i++)
            {
                TrainPart _part = availablePartSelections[Random.Range(0, availablePartSelections.Count)];
                availablePartSelections.Remove(_part);
                GameObject _partSelectionObject = Instantiate(trainPartOptionPrefab, trainPartOptionsParent);
                TrainPartOption _partSelection = _partSelectionObject.GetComponentInChildren<TrainPartOption>();
                _partSelection.Setup(_part.TrainPartSO, partScale);
                _partSelection.OnTrainPartSelected += OnTrainPartSelected;
                _partSelection.OnTrainPartReleased += OnTrainPartReleased;

                spawnedPartOptions.Add(_partSelectionObject);
            }
        }

        private void OnTrainPartReleased(TrainPartOption _trainPartOption)
        {
            RaycastHit2D _hit = Physics2D.Raycast(_trainPartOption.transform.position, Vector2.zero);

            List<TrainPart> _trainPartsOfType = GetPartsOfType(_trainPartOption.TrainPartSO);
            foreach (TrainPart _part in _trainPartsOfType)
            {
                _part.GetComponent<Collider2D>().enabled = false;
            }

            if (_hit.collider == null)
            {
                return;
            }

            TrainPart _trainPart = _hit.collider.GetComponent<TrainPart>();
            if (_trainPart == null)
            {
                return;
            }

            _trainPart.ShowMainTexture();
            _trainPart.GetComponent<Collider2D>().enabled = false;
            trainParts.Remove(_trainPart);

            if (availablePartSelections.Count == 0)
            {
                spawnedPartOptions.Remove(_trainPartOption.transform.parent.gameObject);
                Destroy(_trainPartOption.transform.parent.gameObject);

                if (spawnedPartOptions.Count == 0)
                {
                    StartCoroutine(LevelFinished());
                }

                return;
            }

            _trainPartOption.gameObject.SetActive(false);
            TrainPart _nextPart = availablePartSelections[Random.Range(0, availablePartSelections.Count)];
            _trainPartOption.Setup(_nextPart.TrainPartSO, partScale);
            availablePartSelections.Remove(_nextPart);
        }

        private void OnTrainPartSelected(TrainPartOption _trainPartOption)
        {
            List<TrainPart> _trainPartsOfType = GetPartsOfType(_trainPartOption.TrainPartSO);
            if (_trainPartsOfType.Count == 0)
            {
                Debug.LogError($"No parts of type {_trainPartOption.TrainPartSO.Type} and subtype {_trainPartOption.TrainPartSO.SubType} found in the train");
                return;
            }

            foreach (TrainPart _part in _trainPartsOfType)
            {
                _part.GetComponent<Collider2D>().enabled = true;
            }
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
    }
}