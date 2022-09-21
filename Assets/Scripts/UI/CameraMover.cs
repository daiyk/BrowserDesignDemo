using UnityEngine;

public class CameraMover : MonoBehaviour
{

    private float sensivity = 1f;
    private float moveSensivity = 10f;
    private Vector2 rot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rot.x -= Input.GetAxis("Mouse Y")*sensivity;
        rot.y += Input.GetAxis("Mouse X")*sensivity;

        //rotation
        transform.localRotation = Quaternion.Euler(rot.x,rot.y,0.0f);
   
        //update movement
        if (Input.GetKey(KeyCode.W))
        {
           transform.position += transform.forward*(moveSensivity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * (moveSensivity * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * (moveSensivity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * (moveSensivity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += new Vector3(0.0f,moveSensivity * Time.deltaTime,0.0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position -= new Vector3(0.0f, moveSensivity * Time.deltaTime, 0.0f);
        }
    }
}
