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
        UpdatePhysicsState();
    }

    private void FixedUpdate()
    {
        if (boxBody != null)
        {
            UpdatePhysicsState();
        }
    }

    private void UpdatePhysicsState()
    {
        bool shouldBeKinematic = voiceInput == null || !voiceInput.shieldActive;
        if (boxBody.isKinematic != shouldBeKinematic)
        {
            boxBody.isKinematic = shouldBeKinematic;
        }
    }
}
