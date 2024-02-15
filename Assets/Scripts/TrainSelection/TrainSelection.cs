using TMPro;
using TrainConstructor.Shared;
using TrainConstructor.Train;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrainConstructor.TrainSelection
{
    public class TrainSelection : MonoBehaviour
    {
        [SerializeField] private Image trainImage;
        [SerializeField] private TextMeshProUGUI trainId;
        [SerializeField] private Button trainButton;
        [SerializeField] private Button adButton;
        [SerializeField] private bool isRandom;

        private Train.Train train;
        private AdRewardCanvas adRewardCanvas;

        private void OnEnable()
        {
            trainButton.onClick.AddListener(TrainSelected);
        }

        private void OnDisable()
        {
            trainButton.onClick.RemoveListener(TrainSelected);
        }

        public void Setup(Train.Train _train, Texture2D _snapshot, AdRewardCanvas _adRewardCanvas)
        {
            train = _train;
            adRewardCanvas = _adRewardCanvas;

            trainId.text = _train.Id;

            adButton.gameObject.SetActive(_train.IsLockedWithAnAd);

            if (_train.IsLockedWithAnAd)
            {
                adButton.onClick.AddListener(UnlockTrainWithAd);
                trainButton.onClick.RemoveListener(TrainSelected);
            }

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
            if (isRandom)
            {
                TrainDataManager.Instance.SetRandomTrain();
            }
            else
            {
                TrainDataManager.Instance.SetSelectedTrain(train);
            }

            SceneManager.LoadScene(Scenes.Gameplay.ToString());
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
                TrainSelected();
            }
        }
    }
}