using UnityEngine;
using System.Collections;

public class Camerobo : MonoBehaviour
{

    public float speed_move = 0.1f;
    public float speed_look = 2f;

    public KeyCode key_jump = KeyCode.Space;
    public KeyCode key_crouch = KeyCode.LeftShift;
    public KeyCode key_menu = KeyCode.Escape;
    public KeyCode key_goforward = KeyCode.W;
    public KeyCode key_goleft = KeyCode.A;
    public KeyCode key_goback = KeyCode.S;
    public KeyCode key_goright = KeyCode.D;
    public KeyCode key_lookup = KeyCode.UpArrow;
    public KeyCode key_lookleft = KeyCode.LeftArrow;
    public KeyCode key_lookdown = KeyCode.DownArrow;
    public KeyCode key_lookright = KeyCode.RightArrow;

    void Start()
    {
    }

    void Update()
    {
        MoveAxis();
        JumpButton();
        CrouchButton();
        RotateCamera();
    }

    void MoveAxis()
    {
        float input_x = 0, input_y = 0;
        if (Input.GetKey(key_goright)) input_x += 1;
        if (Input.GetKey(key_goleft)) input_x -= 1;
        if (Input.GetKey(key_goforward)) input_y += 1;
        if (Input.GetKey(key_goback)) input_y -= 1;
        Vector3 vector_right = transform.rotation * new Vector3(1, 0, 0);
        Vector3 vector_forward = transform.rotation * new Vector3(0, 0, 1);
        vector_forward = new Vector3(vector_forward.x, 0, vector_forward.z).normalized;
        transform.position += vector_right * input_x * speed_move;
        transform.position += vector_forward * input_y * speed_move;
    }

    void JumpButton()
    {
        if (Input.GetKey(key_jump))
        {
            transform.position += new Vector3(0, 1, 0) * speed_move;
        }
    }

    void CrouchButton()
    {
        if (Input.GetKey(key_crouch))
        {
            transform.position += new Vector3(0, -1, 0) * speed_move;
        }
    }

    void RotateCamera()
    {
        float rotate_x = 0, rotate_y = 0;
        if (Input.GetKey(key_lookup)) rotate_x += 1;
        if (Input.GetKey(key_lookdown)) rotate_x -= 1;
        if (Input.GetKey(key_lookright)) rotate_y += 1;
        if (Input.GetKey(key_lookleft)) rotate_y -= 1;
        float y_rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        transform.rotation = Quaternion.Euler(
            -speed_look * rotate_x * Mathf.Cos(y_rad),
            speed_look * rotate_y,
            speed_look * rotate_x * Mathf.Sin(y_rad)
        ) * transform.rotation;
        ClampAngleX();
    }

    void ClampAngleX()
    {
        float x_deg = transform.eulerAngles.x;
        Vector3 angles = transform.eulerAngles;
        if (x_deg <= 90 && x_deg >= 90 - speed_look)
        {
            transform.eulerAngles = new Vector3(90 - speed_look - 1, angles.y, angles.z);
        }
        else if (x_deg >= 270 && x_deg <= 270 + speed_look)
        {
            transform.eulerAngles = new Vector3(270 + speed_look + 1, angles.y, angles.z);
        }
    }
}
