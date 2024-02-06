using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using TrainConstructor.ReusableComponents;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class TrainSettingsCanvas : MonoBehaviour
    {
        [SerializeField] private Train.Train train;

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
        private const string CREATE_NEW_TRAIN_WARNING_TEXT = "Are you sure you want to create new train? All unsaved changes will be deleted";

        private List<string> createdTrainsIds = new List<string>();

        private void OnEnable()
        {
            createNewButton.onClick.AddListener(ShowCreationConfirmationPanel);
            saveButton.onClick.AddListener(SaveTrain);
            snapshotButton.onClick.AddListener(TakeSnapshot);
            deleteButton.onClick.AddListener(ShowTrainDeletionConfirmationPanel);
        }

        private void OnDisable()
        {
            saveButton.onClick.RemoveListener(SaveTrain);
            snapshotButton.onClick.RemoveListener(TakeSnapshot);
            deleteButton.onClick.RemoveListener(ShowTrainDeletionConfirmationPanel);
        }

        private void Start()
        {
            LoadCreatedTrains();
            SpawnCreatedTrainButtons();
        }

        private void LoadCreatedTrains()
        {
            string[] files = Directory.GetFiles(Paths.CREATED_TRAINS_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                GameObject trainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                Train.Train train = trainPrefab.GetComponent<Train.Train>();
                createdTrainsIds.Add(train.Id);
            }
        }

        private void SpawnCreatedTrainButtons()
        {
            foreach (string _trainId in createdTrainsIds)
            {
                CreatedTrainButton createdTrainButton = Instantiate(createdTrainButtonPrefab, createdTrainsParentTransform);
                createdTrainButton.Setup(_trainId);
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

        private void TakeSnapshot()
        {
            TrainEditor.Instance.TakeSnapshot();
        }

        private void SaveTrain()
        {
            string _trainId = trainIdInputField.text;
            bool _isLockedWithAnAd = isLockedWithAnAdToggle.isOn;

            if (string.IsNullOrEmpty(_trainId))
            {
                warningText.text = "Train ID cannot be empty!";
                return;
            }

            if (createdTrainsIds.Exists(_id => _id == _trainId) &&
                train.Id != _trainId)
            {
                warningText.text = "Train with this ID already exists!";
                return;
            }

            TrainEditor.Instance.SaveTrain(_trainId, _isLockedWithAnAd);
            createdTrainsIds.Add(train.Id);

            CreatedTrainButton createdTrainButton = Instantiate(createdTrainButtonPrefab, createdTrainsParentTransform);
            createdTrainButton.Setup(_trainId);
        }
    }
}