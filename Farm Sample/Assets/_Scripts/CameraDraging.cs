using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDraging : MonoBehaviour
{
    public float dragSpeed = 0.5f;
    private Vector3 dragOrigin;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector2 move = new Vector2(-pos.x * dragSpeed, -pos.y * dragSpeed);

        transform.Translate(move, Space.World);
    }
}
