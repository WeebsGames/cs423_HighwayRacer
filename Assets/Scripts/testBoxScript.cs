using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class testBoxScript : MonoBehaviour
{
    private const float ShieldForceMultiplier = 5f;

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
        AmplifyCarImpact(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        AmplifyCarImpact(collision);
    }

    private void AmplifyCarImpact(Collision collision)
    {
        if (boxBody == null || voiceInput == null || !voiceInput.shieldActive)
        {
            return;
        }

        Rigidbody otherBody = collision.rigidbody;
        if (otherBody == null ||
            (!otherBody.CompareTag("Player") && otherBody.GetComponent<CarControl>() == null))
        {
            return;
        }

        if (collision.contactCount == 0)
        {
            return;
        }

        ContactPoint contact = collision.GetContact(0);
        Vector3 impulse = collision.impulse;
        if (Vector3.Dot(impulse, contact.normal) < 0f)
        {
            impulse = -impulse;
        }

        // Physics already applied 1x; add only the remaining 4x to the box.
        boxBody.AddForce(impulse * (ShieldForceMultiplier - 1f), ForceMode.Impulse);
    }
}
