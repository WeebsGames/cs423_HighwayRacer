using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class testBoxScript : MonoBehaviour
{
    private Rigidbody boxBody;
    private VoiceInput voiceInput;

    public void Initialize(VoiceInput input)
    {
        voiceInput = input;
        boxBody = GetComponent<Rigidbody>();
        boxBody.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDestroyForShield(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDestroyForShield(other);
    }

    private void TryDestroyForShield(Collider other)
    {
        if (voiceInput == null || !voiceInput.shieldActive ||
            other.GetComponentInParent<CarControl>() == null)
        {
            return;
        }

        Destroy(gameObject);
    }
}
