using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [System.NonSerialized] public float horizontal = 0f;
    [System.NonSerialized] public float vertical = 0f;

    public float mouseHorizontal = 0f;
    public float mouseVertical = 0f;

    private void Awake()
    {
        if(Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        SetMouseHorizontalAndVertial();
    }

    private void FixedUpdate()
    {
        SetHorizontal();
        SetVertical();
    }

    private void SetHorizontal()
    {
        int flag = 0;
        if (Input.GetKey(KeyCode.D)) flag += 1;
        if (Input.GetKey(KeyCode.A)) flag += -1;
        horizontal = Mathf.Lerp(horizontal, flag, 0.5f);

        if (horizontal > 0.9999f) horizontal = 1;
        else if (horizontal > -0.0001f && horizontal < 0.0001f) horizontal = 0;
        else if (horizontal < -0.9999f) horizontal = -1;
    }

    private void SetVertical()
    {
        int flag = 0;
        if (Input.GetKey(KeyCode.W)) flag += 1;
        if (Input.GetKey(KeyCode.S)) flag += -1;
        vertical = Mathf.Lerp(vertical, flag, 0.5f);

        if (vertical > 0.9999f) vertical = 1;
        else if (vertical > -0.0001f && vertical < 0.0001f) vertical = 0;
        else if (vertical < -0.9999f) vertical = -1;
    }

    private void SetMouseHorizontalAndVertial()
    {
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
    }
}
