using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class lightBall : MonoBehaviour
{
    public GameObject holder;
    public GameObject player;
    public float speed = 5f;
    public bool recall = false;
    private Rigidbody rb;
    public float maxRecallTime = 7f;
    private float currentRecallTime = 0f;
    private lightBallController controller;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = player.GetComponent<lightBallController>();
        rb.linearVelocity = holder.transform.forward * speed;
    }

    void FixedUpdate()
    {
        if (recall)
        {
            currentRecallTime += Time.fixedDeltaTime;

            if (currentRecallTime >= maxRecallTime)
            {
                recall = false;
                currentRecallTime = 0f;
                controller.lightBall.SetActive(true);
                controller.recalled();
                Destroy(gameObject);
                return;
            }

            rb.isKinematic = false;
            Vector3 toTarget = holder.transform.position - rb.position;
            float distanceToTarget = toTarget.magnitude;
            Vector3 direction = toTarget.normalized;

            if (distanceToTarget < 0.25f)
            {
                rb.linearVelocity = Vector3.zero;
                controller.lightBall.SetActive(true);
                controller.recalled();
                Destroy(gameObject);
                return;
            }

            RaycastHit hit;
            float castDistance = 1.25f;
            float sphereRadius = 0.3f;
            Vector3 desiredDirection = direction;

            if (Physics.SphereCast(rb.position, sphereRadius, direction, out hit, castDistance))
            {
                Vector3 avoid = Vector3.Cross(hit.normal, Vector3.up).normalized;
                desiredDirection = Vector3.Lerp(direction, avoid, 0.5f).normalized;
            }

            rb.linearVelocity = desiredDirection * speed;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sticky") && !recall)
        {
            rb.isKinematic = true;
        }
    }
}
