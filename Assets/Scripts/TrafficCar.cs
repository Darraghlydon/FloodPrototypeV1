using System.Collections.Generic;
using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    private List<Vector3> route;
    private float speed;
    private int currentIndex;
    private bool loop;

    public void Initialize(List<Vector3> routePositions, float moveSpeed, bool loop)
    {
        route = routePositions;
        speed = moveSpeed;
        this.loop = loop;

        currentIndex = 0;
        transform.position = route[0];
    }

    private void Update()
    {
        if (route == null || route.Count < 2)
            return;

        Vector3 target = route[currentIndex + 1];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir.normalized;

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= route.Count - 1)
            {
                if (loop)
                {
                    currentIndex = 0;
                    transform.position = route[0];
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
