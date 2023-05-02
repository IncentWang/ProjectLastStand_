using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// Player camera and movements
public class PlayerMovement : MonoBehaviour
{
    private Camera _camera;
    private Players player;

    void Start()
    {
        player = GetComponent<Players>();
        Plane plane = new Plane(Vector3.up, transform.position);
        _camera = Camera.main;
    }

    void Update()
    {
        if (!player.photonView.IsMine || !Application.isFocused)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            player.fire();
        }
        float horizontalInput = 0.0f;
        float verticalInput = 0.0f;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1.0f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1.0f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1.0f;
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);
        float distanceToPlane;
        Vector3 mousePosition = transform.position;

        if (plane.Raycast(ray, out distanceToPlane))
        {
            mousePosition = ray.GetPoint(distanceToPlane);
        }

        // Rotate towards the mouse position
        transform.LookAt(new Vector3(mousePosition.x, transform.position.y, mousePosition.z));

        // Move based on the direction of the WASD keys
        Vector3 direction = new Vector3(horizontalInput, 0.0f, verticalInput);
        direction = Vector3.ClampMagnitude(direction, 1.0f);
        Vector3 movement = direction * player.Speed * Time.deltaTime;

        // Check for collision with the Base collider
        Vector3 newPosition = transform.position + direction * player.Speed * Time.deltaTime;
        Vector3 displacement = newPosition - transform.position;
        Vector3 directionNormalized = displacement.normalized;
        float distance = displacement.magnitude;

        int defaultLayer = LayerMask.NameToLayer("Default");
        Debug.DrawRay(transform.position, directionNormalized * distance, Color.red);
        float playerRadius = player.GetComponent<Collider>().bounds.extents.magnitude;
        float totalDistance = distance + playerRadius;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, directionNormalized, out hitInfo, totalDistance, 1 << defaultLayer))
        {
            if (hitInfo.collider.CompareTag("Base"))
            {
                movement = Vector3.zero;
                Debug.Log("Hit!");
            }

        }

        // Move the objects
        transform.position += movement;
        _camera.transform.position = new Vector3(transform.position.x, _camera.transform.position.y, transform.position.z - 51.9f);
    }
}
