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
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(target.position, transform.position);

        float stoppingRadius = 1f;


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

        float avoidanceRadius = 3f;
        float avoidStrength = 2.5f;
        float stoppingThreshold = 0.1f;

        Vector2 avoidanceForce = Vector2.zero;

        if (enemy != null)
        {
            Vector2 directionToEnemy = (Vector2)enemy.position - (Vector2)transform.position;
            if (directionToEnemy.magnitude < avoidanceRadius)
            {
                Vector2 perpendicularDirection = Vector2.Perpendicular(directionToEnemy).normalized;
                avoidanceForce = perpendicularDirection * avoidStrength;
            }
        }

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        Vector2 desiredVelocity = (directionToTarget * speed) + avoidanceForce;
        desiredVelocity = Vector2.ClampMagnitude(desiredVelocity, speed);

        if (!float.IsNaN(desiredVelocity.x) && !float.IsNaN(desiredVelocity.y))
        {
            if (Vector2.Distance(transform.position, target.position) > stoppingThreshold)
            {
                transform.position += (Vector3)desiredVelocity * Time.deltaTime;
            }
            else
            {
                transform.position = target.position;
            }

            RotateTowards(desiredVelocity);
        }

    }

    void RotateTowards(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
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
