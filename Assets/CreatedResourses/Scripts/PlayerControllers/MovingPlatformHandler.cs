using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovingPlatformHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _platformLayers;

    private CharacterController controller;
    [ReadOnlyInspector] public Transform activePlatform;
    private Vector3 lastPlatformPosition;
    private Quaternion lastPlatformRotation;
    public Transform _groundCheck;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        CheckPlatform();

        if (activePlatform != null)
        {
            // 1. Вычисляем смещение позиции
            Vector3 worldPos = activePlatform.TransformPoint(Vector3.zero); // Текущая позиция платформы
            Vector3 movement = activePlatform.position - lastPlatformPosition;

            //// 2. Вычисляем вращение (если платформа крутится)
            //Quaternion rotationDiff = activePlatform.rotation * Quaternion.Inverse(lastPlatformRotation);

            //if (rotationDiff != Quaternion.identity)
            //{
            //    // Поворачиваем позицию игрока относительно центра платформы
            //    Vector3 direction = transform.position - activePlatform.position;
            //    Vector3 rotatedDirection = rotationDiff * direction;
            //    movement += (rotatedDirection - direction);

            //    // Поворачиваем самого игрока
            //    transform.rotation = rotationDiff * transform.rotation;
            //}

            // 3. Применяем движение через CharacterController
            if (movement.magnitude > 0.0001f)
            {
                controller.Move(movement);
            }

            // Обновляем данные для следующего кадра
            lastPlatformPosition = activePlatform.position;
            //lastPlatformRotation = activePlatform.rotation;
        }
    }

    private void CheckPlatform()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_groundCheck.position, 0.3f, _platformLayers);

        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (LayerMask.LayerToName(hitCollider.gameObject.layer) == "Moving Platform")
                {
                    if (hitCollider.transform != activePlatform)
                    {
                        activePlatform = hitCollider.transform;
                        lastPlatformPosition = activePlatform.position;
                        lastPlatformRotation = activePlatform.rotation;
                    }
                }
            }
        }
        else
        {
            activePlatform = null;
        }
    }
}
