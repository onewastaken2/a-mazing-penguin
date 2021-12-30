using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float panSpeed = 10;   //How fast camera moves when panning


    private void Update()
    {
        //Player pressed W to pan up
        if(Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * panSpeed * Time.deltaTime;
        }

        //Player pressed A to pan left
        if(Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * panSpeed * Time.deltaTime;
        }

        //Player pressed S to pan down
        if(Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.back * panSpeed * Time.deltaTime;
        }

        //Player pressed D to pan right
        if(Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * panSpeed * Time.deltaTime;
        }
    }
}
