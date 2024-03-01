using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TrainConstructor.TrainData;
using TrainConstructor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class TrainUIManager : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] private TMP_InputField trainIdInputField;
        [SerializeField] private Toggle isLockedWithAnAdToggle;
        [SerializeField] private Button createNewButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Transform createdTrainsParentTransform;
        [SerializeField] private ConfirmationPanel confirmationPanel;
        [SerializeField] private Camera snapshotCamera;

        [Header("Prefabs")]
        [SerializeField] private CreatedTrainButton createdTrainButtonPrefab;

        public event Action<Train> TrainSaveSelected;
        public event Action<Train> TrainLoadSelected;
        public event Action<Train> TrainDeleteSelected;

        private const string DELETE_TRAIN_CONFIRMATION_TEXT = "Are you sure you want to delete this train?";
        private const string CREATE_NEW_TRAIN_WARNING_TEXT  = "Are you sure you want to create new train? All unsaved changes will be deleted";
        private const string LOAD_TRAIN_WARNING_TEXT        = "Are you sure you want to load this train? All unsaved changes will be deleted";
        private const string TRAIN_ID_EMPTY_WARNING_TEXT    = "Train ID cannot be empty!";
        private const string TRAIN_ID_EXISTS_WARNING_TEXT   = "Train with this ID already exists!";

        private readonly List<CreatedTrainButton> createdTrainButtons = new List<CreatedTrainButton>();

        private void OnEnable()
        {
            createNewButton.onClick.AddListener(ShowCreationConfirmationPanel);
            saveButton.onClick.AddListener(SaveButtonPressed);
            deleteButton.onClick.AddListener(ShowTrainDeletionConfirmationPanel);
        }

        private void OnDisable()
        {
            createNewButton.onClick.RemoveListener(ShowCreationConfirmationPanel);
            saveButton.onClick.RemoveListener(SaveButtonPressed);
            deleteButton.onClick.RemoveListener(ShowTrainDeletionConfirmationPanel);
        }

        public void SpawnCreatedTrainButtons(List<Train> _createdTrains)
        {
            foreach (Train _train in _createdTrains)
            {
                TrainAdded(_train);
            }
        }

        public void TrainAdded(Train _train)
        {
            CreatedTrainButton _createdTrainButton = Instantiate(createdTrainButtonPrefab, createdTrainsParentTransform);
            _createdTrainButton.Setup(_train, ShowTrainLoadConfirmationPanel);
            createdTrainButtons.Add(_createdTrainButton);
        }

        public void TrainDeleted(Train _train)
        {
            CreatedTrainButton _createdTrainButton = createdTrainButtons.Find(_button => _button.Train.Id == _train.Id);
            createdTrainButtons.Remove(_createdTrainButton);
            Destroy(_createdTrainButton.gameObject);
        }

        public void SnapshotUpdated(Train _train)
        {
            CreatedTrainButton _createdTrainButton = createdTrainButtons.Find(_button => _button.Train.Id == _train.Id);

            if (_createdTrainButton == null)
            {
                return;
            }

            _createdTrainButton.UpdateSnapshot(_train.Snapshot);
        }

        private void ShowTrainLoadConfirmationPanel(Train _train)
        {
            confirmationPanel.Show(LOAD_TRAIN_WARNING_TEXT, () => LoadTrain(_train), null);
        }

        private void LoadTrain(Train _train)
        {
            TrainEditor.Instance.LoadTrain(_train);
            trainIdInputField.text = _train.Id;
            isLockedWithAnAdToggle.isOn = _train.IsLockedWithAnAd;

            CreatedTrainButton _createdTrainButton = createdTrainButtons.Find(_button => _button.Train.Id == _train.Id);
            foreach (CreatedTrainButton _button in createdTrainButtons)
            {
                _button.SetSelected(_button == _createdTrainButton);
            }
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
            TrainEditor.Instance.DeleteTrain();
            ResetOptions();
        }

        private void ShowCreationConfirmationPanel()
        {
            confirmationPanel.Show(CREATE_NEW_TRAIN_WARNING_TEXT, CreateNewTrain, null);
        }

        private void CreateNewTrain()
        {
            TrainEditor.Instance.ResetEditor();
            ResetOptions();
        }

        private void SaveButtonPressed()
        {
            string _trainId = trainIdInputField.text;
            warningText.text = string.Empty;

            if (string.IsNullOrWhiteSpace(_trainId))
            {
                warningText.text = TRAIN_ID_EMPTY_WARNING_TEXT;
                return;
            }

            if (TrainDataManager.Instance.CreatedTrains.Any(_train => _train.Id == _trainId) &&
                TrainEditor.Instance.TrainObject.Id != _trainId)
            {
                warningText.text = TRAIN_ID_EXISTS_WARNING_TEXT;
                return;
            }

            TrainEditor.Instance.SaveTrain(_trainId, isLockedWithAnAdToggle.isOn);
        }
    }
}