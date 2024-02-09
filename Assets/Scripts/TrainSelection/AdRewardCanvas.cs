using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TrainConstructor.TrainSelection
{
    public class AdRewardCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject adRewardCanvas;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private Button closeButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;

        public Action<bool> AdFinished;

        private void OnEnable()
        {
            videoPlayer.loopPointReached += EndReached;
            closeButton.onClick.AddListener(CancelAd);
        }

        private void OnDisable()
        {
            videoPlayer.loopPointReached -= EndReached;
            closeButton.onClick.RemoveListener(CancelAd);
        }

        public void ShowAd()
        {
            videoPlayer.frame = 0;
            adRewardCanvas.SetActive(true);

            LeanTween.cancel(canvasGroup.gameObject);
            LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration)
                .setOnComplete(() => videoPlayer.Play());
        }

        private void EndReached(VideoPlayer _source)
        {
            _source.Stop();
            AdFinished?.Invoke(true);
            Hide();
        }

        private void CancelAd()
        {
            videoPlayer.Stop();
            AdFinished?.Invoke(false);
            Hide();
        }

        private void Hide()
        {
            LeanTween.cancel(canvasGroup.gameObject);
            LeanTween.alphaCanvas(canvasGroup, 0, fadeDuration)
                .setOnComplete(() => adRewardCanvas.SetActive(false));
        }
    }
}