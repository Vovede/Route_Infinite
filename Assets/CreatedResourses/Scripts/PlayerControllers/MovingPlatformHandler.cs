using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovingPlatformHandler : MonoBehaviour
{
    private CharacterController controller;
    private Transform activePlatform;
    private Vector3 lastPlatformPosition;
    private Quaternion lastPlatformRotation;

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

            // 2. Вычисляем вращение (если платформа крутится)
            Quaternion rotationDiff = activePlatform.rotation * Quaternion.Inverse(lastPlatformRotation);

            if (rotationDiff != Quaternion.identity)
            {
                // Поворачиваем позицию игрока относительно центра платформы
                Vector3 direction = transform.position - activePlatform.position;
                Vector3 rotatedDirection = rotationDiff * direction;
                movement += (rotatedDirection - direction);

                // Поворачиваем самого игрока
                transform.rotation = rotationDiff * transform.rotation;
            }

            // 3. Применяем движение через CharacterController
            if (movement.magnitude > 0.0001f)
            {
                controller.Move(movement);
            }

            // Обновляем данные для следующего кадра
            lastPlatformPosition = activePlatform.position;
            lastPlatformRotation = activePlatform.rotation;
        }
    }

    private void CheckPlatform()
    {
        RaycastHit hit;
        // Пускаем луч вниз от центра контроллера
        // Длина луча чуть больше половины высоты, чтобы доставать до пола
        float rayLength = (controller.height / 2f) + 0.3f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, 7))
        {
            // Если мы приземлились на новую платформу или сменили её
            if (hit.transform != activePlatform)
            {
                activePlatform = hit.transform;
                Debug.Log(hit.collider.name);
                lastPlatformPosition = activePlatform.position;
                lastPlatformRotation = activePlatform.rotation;
            }
        }
        else
        {
            // Мы в воздухе
            activePlatform = null;
        }
    }
}
