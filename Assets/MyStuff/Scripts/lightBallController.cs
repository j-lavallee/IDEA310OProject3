using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class lightBallController : MonoBehaviour
{
    public GameObject lightBallPrefab;
    public GameObject lightBall;
    public GameObject holder;
    private GameObject currentBall;

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (currentBall == null)
            {
                lightBall.SetActive(false);
                currentBall = Instantiate(lightBallPrefab, holder.transform.position, holder.transform.rotation);
                currentBall.GetComponent<lightBall>().holder = holder;
                currentBall.GetComponent<lightBall>().player = gameObject;
            }
            else if (currentBall != null)
            {
                currentBall.GetComponent<lightBall>().recall = true;
                currentBall.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }
    }
}
