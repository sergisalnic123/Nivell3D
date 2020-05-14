using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulsePlayer : MonoBehaviour
{
    [Range(1, 50)]
    public float mass = 1.0f; 
    Vector3 impulse = Vector3.zero;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (impulse.magnitude > 0.2f) controller.Move(impulse * Time.deltaTime);
        impulse = Vector3.Lerp(impulse, Vector3.zero, 5 * Time.deltaTime);
    }

    public void AddImpulse(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0) direction.y = -direction.y;
        impulse += direction.normalized * force / mass;
    }
}
