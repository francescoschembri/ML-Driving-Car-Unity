using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float reward = 0.1f;

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("car"))
        {
            collision.GetComponentInParent<IACarController>().AddReward(reward);
        }
    }

}
