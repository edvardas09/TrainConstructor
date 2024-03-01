using System;
using TMPro;
using TrainConstructor.TrainData;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class CreatedTrainButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private Image snapshotImage;
        [SerializeField] private GameObject border;
        [SerializeField] private Button button;

        private Train train;

        public Train Train => train;

        public void Setup(Train _train, Action<Train> _callback)
        {
            train = _train;
            idText.text = _train.Id;

            if (_train.Snapshot == null)
            {
                Debug.LogError($"Snapshot for train {_train.Id} does not exists");
                return;
            }

            snapshotImage.sprite = _train.Snapshot;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _callback?.Invoke(train));
        }

        public void UpdateSnapshot(Sprite _snapshot)
        {
            snapshotImage.sprite = _snapshot;
        }

        public void SetSelected(bool _isSelected)
        {
            border.SetActive(_isSelected);
        }
    }
}