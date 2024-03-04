using TMPro;
using TrainConstructor.Shared;
using TrainConstructor.TrainData;
using TutoTOONS;
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
        [SerializeField] private Image adImage;
        [SerializeField] private bool isRandom;

        private Train train;

        private void OnEnable()
        {
            trainButton.onClick.AddListener(TrainSelected);
        }

        private void OnDisable()
        {
            trainButton.onClick.RemoveListener(TrainSelected);
        }

        public void Setup(Train _train)
        {
            train = _train;

            trainId.text = _train.Id;

            adImage.gameObject.SetActive(_train.IsLockedWithAnAd);

            if (_train.IsLockedWithAnAd)
            {
                trainButton.onClick.AddListener(UnlockTrainWithAd);
                trainButton.onClick.RemoveListener(TrainSelected);
            }

            if (_train.Snapshot == null)
            {
                Debug.LogError($"Snapshot for train {_train.Id} does not exists");
                return;
            }

            trainImage.sprite = _train.Snapshot;
            trainImage.gameObject.SetActive(true);
        }

        private void TrainSelected()
        {
            if (isRandom)
            {
                TrainDataManager.Instance.SetSelectedTrain(null);
            }
            else
            {
                TrainDataManager.Instance.SetSelectedTrain(train);
            }

            SceneManager.LoadScene(Scenes.Gameplay.ToString());
        }

        private void UnlockTrainWithAd()
        {
            TestAdsController.instance.SetCloseCallback(OnAdFinished);
            TestAdsController.instance.ShowAd("Rewarded", "WATCH_AD_BUTTON");
        }

        private void OnAdFinished(bool _success)
        {
            if (_success)
            {
                TrainSelected();
            }
        }
    }
}