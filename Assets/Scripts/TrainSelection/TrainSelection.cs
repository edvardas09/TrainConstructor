using System;
using TMPro;
using TrainConstructor.Train;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.TrainSelection
{
    public class TrainSelection : MonoBehaviour
    {
        [SerializeField] private Image trainImage;
        [SerializeField] private TextMeshProUGUI trainId;
        [SerializeField] private Button trainButton;
        [SerializeField] private Button adButton;

        private Train.Train train;
        private AdRewardCanvas adRewardCanvas;

        public void Setup(Train.Train _train, Texture2D _snapshot, AdRewardCanvas _adRewardCanvas)
        {
            train = _train;
            adRewardCanvas = _adRewardCanvas;

            trainId.text = _train.Id;
            trainButton.onClick.AddListener(TrainSelected);

            adButton.gameObject.SetActive(_train.IsLockedWithAnAd);
            adButton.onClick.AddListener(UnlockTrainWithAd);

            if (_snapshot == null)
            {
                Debug.LogError($"Snapshot for train {_train.Id} does not exists");
                return;
            }

            trainImage.sprite = Sprite.Create(_snapshot, new Rect(0, 0, _snapshot.width, _snapshot.height), new Vector2(0.5f, 0.5f));
            trainImage.gameObject.SetActive(true);
        }

        private void TrainSelected()
        {
            TrainDataManager.Instance.SetSelectedTrain(train);
        }

        private void UnlockTrainWithAd()
        {
            adRewardCanvas.AdFinished += OnAdFinished;
            adRewardCanvas.ShowAd();
        }

        private void OnAdFinished(bool _isEndReached)
        {
            adRewardCanvas.AdFinished -= OnAdFinished;

            if (_isEndReached)
            {
                TrainDataManager.Instance.SetSelectedTrain(train);
                adButton.gameObject.SetActive(false);
            }
        }
    }
}