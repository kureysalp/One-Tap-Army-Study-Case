using System.Collections;
using OneTapArmyCase.Game;
using UnityEngine;

namespace OneTapArmyCase.Army
{
    public class ArcherUnit : Soldier
    {
        [SerializeField] private float _arrowSpeed;
        [SerializeField] private Transform _shootPosition;

        private GameObject _currentArrow;

        private void ShootArrow()
        {
            var arrow = ObjectPooling.Instance.GetArrowFromPool();
            _currentArrow = arrow;
            StartCoroutine(CO_ShootArrow(arrow));
        }

        private IEnumerator CO_ShootArrow(GameObject arrow)
        {
            var elapsedTime = 0f;
            var travelTime = 1 / _arrowSpeed;
            while (elapsedTime < travelTime)
            {
                var targetPosition = _currentTarget.UnitTransform.position + Vector3.up;
                var arrowPosition = Vector3.Lerp(_shootPosition.position, targetPosition, elapsedTime / travelTime);
                arrow.transform.position = arrowPosition;
                arrow.transform.LookAt(targetPosition);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ObjectPooling.Instance.ReleaseArrow(arrow);
            DealDamage();
        }

        protected override void Die()
        {
            base.Die();
            ObjectPooling.Instance.ReleaseArrow(_currentArrow);

        }
    }
}
