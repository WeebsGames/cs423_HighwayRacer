using System.Collections.Generic;
using UnityEngine;

public class slideScript : MonoBehaviour
{
    private const float SlideDistance = 2f;
    private const float SlideDuration = 2f;
    private readonly HashSet<Collider> sirenContacts = new HashSet<Collider>();

    private Vector3 closedPosition;
    private float offset;
    private float direction;

    private void Start()
    {
        if (gameObject.name == "Right")
        {
            direction = 1f;
        }
        else if (gameObject.name == "Left")
        {
            direction = -1f;
        }
        else
        {
            Debug.LogError("A doorBox must be named Right or Left to determine its slide direction.", this);
            enabled = false;
            return;
        }

        closedPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        TrackSirenContact(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TrackSirenContact(other);
    }

    private void TrackSirenContact(Collider other)
    {
        if (other.isTrigger && other.gameObject.name == "SirenBox")
        {
            sirenContacts.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        sirenContacts.Remove(other);
    }

    private void OnDisable()
    {
        sirenContacts.Clear();
    }

    private void Update()
    {
        // Disabled or destroyed triggers do not always send OnTriggerExit.
        sirenContacts.RemoveWhere(contact => contact == null || !contact.enabled
            || !contact.gameObject.activeInHierarchy);
        bool shouldOpen = sirenContacts.Count > 0;

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
