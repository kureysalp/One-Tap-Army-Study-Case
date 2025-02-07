using System;
using UnityEngine;

namespace OneTapArmyCase.Player
{
    public class UserInput : MonoBehaviour
    {
        public static event Action<Vector3> OnPlayerTapMove;

        [SerializeField] private LayerMask _groundLayer;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            TapInput();
        }

        private void TapInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, _groundLayer))
                    OnPlayerTapMove?.Invoke(hit.point);
            }
        }
    }
}