using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ConfirmationPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private float fadeDuration = 0.3f;

        private void OnValidate()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void Show(string message, Action onConfirm, Action onCancel)
        {
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            messageText.text = message;
            confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                Hide();
            });
            cancelButton.onClick.AddListener(() =>
            {
                onCancel?.Invoke();
                Hide();
            });

            gameObject.SetActive(true);
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration);
        }

        private void Hide()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 0, fadeDuration)
                .setOnComplete(() => gameObject.SetActive(false));
        }
    }
}