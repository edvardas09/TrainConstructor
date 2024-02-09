using System.Collections.Generic;
using System.Linq;
using TMPro;
using TrainConstructor.Train;
using TrainConstructor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class TrainSettings : MonoBehaviour
    {
        [SerializeField] private TrainEditor trainEditor;

        [Header("UI references")]
        [SerializeField] private TMP_InputField trainIdInputField;
        [SerializeField] private Toggle isLockedWithAnAdToggle;
        [SerializeField] private Button createNewButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button snapshotButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Transform createdTrainsParentTransform;
        [SerializeField] private ConfirmationPanel confirmationPanel;

        [Header("Prefabs")]
        [SerializeField] private CreatedTrainButton createdTrainButtonPrefab;

        private const string DELETE_TRAIN_CONFIRMATION_TEXT = "Are you sure you want to delete this train?";
        private const string CREATE_NEW_TRAIN_WARNING_TEXT  = "Are you sure you want to create new train? All unsaved changes will be deleted";
        private const string LOAD_TRAIN_WARNING_TEXT        = "Are you sure you want to load this train? All unsaved changes will be deleted";
        private const string TRAIN_NOT_SAVED_WARNING_TEXT   = "Train must be saved before taking snapshot";
        private const string TRAIN_ID_EMPTY_WARNING_TEXT    = "Train ID cannot be empty!";
        private const string TRAIN_ID_EXISTS_WARNING_TEXT   = "Train with this ID already exists!";

        private List<CreatedTrainButton> createdTrainButtons = new List<CreatedTrainButton>();

        private void OnEnable()
        {
            trainEditor.CreatedTrainsLoaded += SpawnCreatedTrainButtons;
            trainEditor.TrainAdded += OnTrainAdded;
            trainEditor.TrainDeleted += OnTrainDeleted;
            trainEditor.SnapshotUpdated += OnSnapshotUpdated;

            createNewButton.onClick.AddListener(ShowCreationConfirmationPanel);
            saveButton.onClick.AddListener(SaveTrain);
            snapshotButton.onClick.AddListener(TakeSnapshot);
            deleteButton.onClick.AddListener(ShowTrainDeletionConfirmationPanel);
        }

        private void OnDisable()
        {
            trainEditor.CreatedTrainsLoaded -= SpawnCreatedTrainButtons;
            trainEditor.TrainAdded -= OnTrainAdded;
            trainEditor.TrainDeleted -= OnTrainDeleted;
            trainEditor.SnapshotUpdated -= OnSnapshotUpdated;

            createNewButton.onClick.RemoveListener(ShowCreationConfirmationPanel);
            saveButton.onClick.RemoveListener(SaveTrain);
            snapshotButton.onClick.RemoveListener(TakeSnapshot);
            deleteButton.onClick.RemoveListener(ShowTrainDeletionConfirmationPanel);
        }

        private void SpawnCreatedTrainButtons(List<Train.Train> _createdTrains)
        {
            foreach (Train.Train _train in _createdTrains)
            {
                OnTrainAdded(_train);
            }
        }

        private void ShowTrainLoadConfirmationPanel(Train.Train _train)
        {
            confirmationPanel.Show(LOAD_TRAIN_WARNING_TEXT, () => LoadTrain(_train), null);
        }

        private void LoadTrain(Train.Train _train)
        {
            trainEditor.LoadTrain(_train);
            trainIdInputField.text = _train.Id;
            isLockedWithAnAdToggle.isOn = _train.IsLockedWithAnAd;
        }

        private void ResetOptions()
        {
            trainIdInputField.text = string.Empty;
            isLockedWithAnAdToggle.isOn = false;
        }

        private void ShowTrainDeletionConfirmationPanel()
        {
            confirmationPanel.Show(DELETE_TRAIN_CONFIRMATION_TEXT, DeleteTrain, null);
        }

        private void DeleteTrain()
        {
            trainEditor.DeleteTrain();
            ResetOptions();
        }

        private void ShowCreationConfirmationPanel()
        {
            confirmationPanel.Show(CREATE_NEW_TRAIN_WARNING_TEXT, CreateNewTrain, null);
        }

        private void CreateNewTrain()
        {
            trainEditor.ResetEditor();
            ResetOptions();
        }

        private void TakeSnapshot()
        {
            SaveTrain();

            if (string.IsNullOrEmpty(trainEditor.TrainObject.Id))
            {
                warningText.text = TRAIN_NOT_SAVED_WARNING_TEXT;
                return;
            }

            trainEditor.TakeSnapshot();
        }

        private void SaveTrain()
        {
            string _trainId = trainIdInputField.text;
            bool _isLockedWithAnAd = isLockedWithAnAdToggle.isOn;

            warningText.text = string.Empty;

            if (string.IsNullOrWhiteSpace(_trainId))
            {
                warningText.text = TRAIN_ID_EMPTY_WARNING_TEXT;
                return;
            }

            if (TrainDataManager.Instance.CreatedTrains.Any(_train => _train.Id == _trainId) &&
                trainEditor.TrainObject.Id != _trainId)
            {
                warningText.text = TRAIN_ID_EXISTS_WARNING_TEXT;
                return;
            }

            trainEditor.SaveTrain(_trainId, _isLockedWithAnAd);
        }

        private void OnTrainAdded(Train.Train _train)
        {
            CreatedTrainButton _createdTrainButton = Instantiate(createdTrainButtonPrefab, createdTrainsParentTransform);
            Texture2D _snapshot = TrainDataManager.Instance.GetTrainSnapshot(_train.Id);
            _createdTrainButton.Setup(_train, _snapshot);
            Button _button = _createdTrainButton.GetComponent<Button>();
            _button.onClick.AddListener(() => ShowTrainLoadConfirmationPanel(_train));
            createdTrainButtons.Add(_createdTrainButton);
        }

        private void OnTrainDeleted(Train.Train _train)
        {
            CreatedTrainButton _createdTrainButton = createdTrainButtons.Find(_button => _button.Train.Id == _train.Id);
            createdTrainButtons.Remove(_createdTrainButton);
            Destroy(_createdTrainButton.gameObject);
        }

        private void OnSnapshotUpdated(Train.Train _train, Texture2D _snapshot)
        {
            CreatedTrainButton _createdTrainButton = createdTrainButtons.Find(_button => _button.Train.Id == _train.Id);
            _createdTrainButton.Setup(_train, _snapshot);
        }
    }
}