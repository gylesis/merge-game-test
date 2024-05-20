using UnityEngine;

namespace Dev
{
    public class UFO : MonoBehaviour
    {
        [SerializeField] private Transform _view;
        [SerializeField] private float _rotationSpeed = 2;

        private void Update()
        {
            Vector3 eulerAngles = _view.rotation.eulerAngles;
            eulerAngles.y += Time.deltaTime * _rotationSpeed;
            _view.rotation = Quaternion.Euler(eulerAngles);
        }
    }
}