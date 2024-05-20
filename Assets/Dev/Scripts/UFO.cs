using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Dev
{
    public class UFO : MonoBehaviour
    {
        [SerializeField] private Transform _view;
        [SerializeField] private float _rotationSpeed = 2;

        [SerializeField] private Image _progressBar;
        private TweenerCore<float, float, FloatOptions> _doFillAmountTween;

        private void Update()
        {
            Vector3 eulerAngles = _view.rotation.eulerAngles;
            eulerAngles.y += Time.deltaTime * _rotationSpeed;
            _view.rotation = Quaternion.Euler(eulerAngles);
        }

        public void StartProgressBar(float value, float duration)
        {
            _progressBar.fillAmount = 0;
            _doFillAmountTween.Kill();
            _doFillAmountTween = _progressBar.DOFillAmount(value, duration);
        }

        public void SetProgressBarState(bool isOn)
        {
            _doFillAmountTween.Kill();
            _progressBar.gameObject.SetActive(isOn);
        }
    }   
}