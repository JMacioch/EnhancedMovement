using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{
    public GameObject player;
    public Text velocityText;
    Rigidbody rb;
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        velocityText.text = "Velocity: "+ rb.velocity.magnitude.ToString("F2");
    }
}
