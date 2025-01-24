using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviours : MonoBehaviour
{
    public enum Behavior { Seek, Flee, Arrive, Avoid }
    public Behavior currentBehavior;

    public Transform target;
    public Transform enemy;
    public float speed = 5f;
    public float arrivalRadius = 1f;
    public float avoidDistance = 1f;

    void Update()
    {
        switch (currentBehavior)
        {
            case Behavior.Seek:
                Seek(target);
                break;
            case Behavior.Flee:
                Flee(enemy);
                break;
            case Behavior.Arrive:
                Arrive(target);
                break;
            case Behavior.Avoid:
                Avoid();
                break;
        }
    }

    void Seek(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 newVelocity = direction * speed;
        transform.position += (Vector3)newVelocity * Time.deltaTime;

        RotateTowards(direction);
    }

    void Flee(Transform enemy)
    {
        Vector2 direction = (transform.position - enemy.position).normalized;
        Vector2 newVelocity = direction * speed;
        transform.position += (Vector3)newVelocity * Time.deltaTime;

        ClampPositionWithinBounds();

        RotateTowards(direction);
    }

    void Arrive(Transform target)
    {
        //Vector2 direction = (target.position - transform.position).normalized;
        //float distance = Vector2.Distance(target.position, transform.position);
        //float desiredSpeed = (distance < arrivalRadius) ? speed * (distance / arrivalRadius) : speed;
        //Vector2 newVelocity = direction * desiredSpeed;
        //transform.position += (Vector3)newVelocity * Time.deltaTime;
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(target.position, transform.position);

        float stoppingRadius = 1f;

        //float desiredSpeed = (distance < arrivalRadius) ? speed * (distance - arrivalRadius) : speed;

        if (distance > stoppingRadius)
        {
            float desiredSpeed = Mathf.Min(speed, (speed * (distance / arrivalRadius)));
            Vector2 newVelocity = direction * desiredSpeed;
            transform.position += (Vector3)newVelocity * Time.deltaTime;
        }
        else
        {
            transform.position = target.position - (Vector3)(direction * stoppingRadius);
        }

        RotateTowards(direction);
    }

    void Avoid()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, avoidDistance);

        if (hit.collider != null)
        {
            Vector2 avoidanceForce = Vector2.Reflect(transform.right, hit.normal) * speed;
            transform.position += (Vector3)avoidanceForce * Time.deltaTime;

            RotateTowards(avoidanceForce);
        }
    }

    void RotateTowards(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void ClampPositionWithinBounds()
    {
        Vector3 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        float clampedX = Mathf.Clamp(transform.position.x, screenBottomLeft.x, screenTopRight.x);
        float clampedY = Mathf.Clamp(transform.position.y, screenBottomLeft.y, screenTopRight.y);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
