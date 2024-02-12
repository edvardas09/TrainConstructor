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
        [SerializeField] private GameObject trainPartSelectionPrefab;
        [SerializeField] private Transform trainPartSelectionsParent;
        [SerializeField] private TrainPartOption TrainPartOption;

        private Train.Train spawnedTrain;
        private List<TrainPart> trainParts = new List<TrainPart>();
        private List<TrainPart> availablePartSelections = new List<TrainPart>();

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
                Debug.LogError("No train selected, opening train selection scene");
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
                Destroy(_part.GetComponent<DraggablePart>());
                _part.ShowOutlineTexture();
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
            
            TrainPartOption.Setup(trainParts[0].TrainPartSO, partScale);
        }

        private void SpawnInitialPartSelections()
        {
            for (int i = 0; i < maxTrainParts; i++)
            {
                TrainPart _part = availablePartSelections[Random.Range(0, availablePartSelections.Count)];
                availablePartSelections.Remove(_part);
                //TrainPartSelection _partSelection = Instantiate(trainPartSelectionPrefab, trainPartSelectionsParent);
                //_partSelection.Setup(_part.TrainPartSO);
                //_partSelection.TrainPartSelected += OnTrainPartSelected;
            }
        }

        private void OnTrainPartSelected(TrainPartSO sO)
        {
            throw new System.NotImplementedException();
        }
    }
}