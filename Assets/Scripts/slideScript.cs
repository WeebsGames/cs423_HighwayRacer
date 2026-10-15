using UnityEngine;

public class slideScript : MonoBehaviour
{
    private const float SlideDistance = 1f;
    private const float SlideDuration = 2f;
    private const float ActivationRadius = 3f;

    private VoiceInput voiceInput;
    private Transform car;
    private Vector3 closedPosition;
    private float offset;
    private float direction;
    private bool wasOpen;

    public void Initialize(VoiceInput input, Transform carTransform)
    {
        voiceInput = input;
        car = carTransform;
        closedPosition = transform.position;
    }

    private void Update()
    {
        bool shouldOpen = voiceInput != null && car != null && voiceInput.sirenActive
            && (car.position - closedPosition).sqrMagnitude <= ActivationRadius * ActivationRadius;

        if (shouldOpen && !wasOpen)
        {
            direction = car.position.x <= closedPosition.x ? 1f : -1f;
        }
        wasOpen = shouldOpen;

        float targetOffset = shouldOpen ? direction * SlideDistance : 0f;
        float nextOffset = Mathf.MoveTowards(offset, targetOffset,
            SlideDistance / SlideDuration * Time.deltaTime);
        if (nextOffset != offset)
        {
            offset = nextOffset;
            transform.position = closedPosition + Vector3.right * offset;
        }
    }
}
