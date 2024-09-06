using System;
using System.Numerics;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera in the Inspector
    public Camera topDownCamera; // Assign your top-down camera in the Inspector
    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float panSpeed = 0.5f; // Speed of panning
    public float minOrthoSize = 5f; // Minimum orthographic size for zooming in
    public float maxOrthoSize = 70f; // Maximum orthographic size for zooming out
    public float margin = 2f; // Margin to ensure all tiles are visible
    public UnityEngine.Vector3 panLimitMin; // Minimum limit for panning
    public UnityEngine.Vector3 panLimitMax; // Maximum limit for panning

    private UnityEngine.Vector3 initialCameraPosition;

    public Button toggleViewButton;
    public TextMeshProUGUI viewModeText;

    private bool isTopView = false;

    void Start()
    {
        // Add listener to the button
        toggleViewButton.onClick.AddListener(ToggleView);
        mainCamera.enabled = true;
        topDownCamera.enabled = false;
        initialCameraPosition = topDownCamera.transform.position;
        UpdateViewModeText();

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mainCamera.enabled = !mainCamera.enabled;
            topDownCamera.enabled = !topDownCamera.enabled;
            UpdateViewModeText();

            if (topDownCamera.enabled)
            {
                AdjustTopDownCamera();
            }
        }

        if (topDownCamera.enabled)
        {
            HandleZoom();
            HandlePan();
        }
    }

    void AdjustTopDownCamera()
    {
        // Adjust the orthographic size of the top-down camera to fit all tiles
        // Assume the tiles are tagged with "Tile" and the top-down camera is looking straight down

        UnityEngine.Vector3 stageCenter = UnityEngine.Vector3.zero;
        float stageWidth = 0f;
        float stageHeight = 0f;

        // Find all tiles with the tag "Tile"
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        if (tiles.Length > 0)
        {
            Bounds bounds = new Bounds(tiles[0].transform.position, UnityEngine.Vector3.zero);
            foreach (GameObject tile in tiles)
            {
                bounds.Encapsulate(tile.transform.position);
            }

            stageCenter = bounds.center;
            stageWidth = bounds.size.x + margin; // Add margin to width
            stageHeight = bounds.size.z + margin; // Add margin to height
        }

        // Adjust the position of the top-down camera to center on the stage
        topDownCamera.transform.position = new UnityEngine.Vector3(stageCenter.x, topDownCamera.transform.position.y, stageCenter.z);
        initialCameraPosition = topDownCamera.transform.position;

        // Adjust the orthographic size of the top-down camera to fit the stage
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Mathf.Max(stageWidth / aspectRatio, stageHeight);
        topDownCamera.orthographicSize = cameraHeight / 2f;

        // Set pan limits based on stage size
        panLimitMin = new UnityEngine.Vector3(stageCenter.x - stageWidth / 2, topDownCamera.transform.position.y, stageCenter.z - stageHeight / 2);
        panLimitMax = new UnityEngine.Vector3(stageCenter.x + stageWidth / 2, topDownCamera.transform.position.y, stageCenter.z + stageHeight / 2);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.Equals) || scroll > 0) // '+' key or scroll up
        {
            topDownCamera.orthographicSize -= zoomSpeed * Time.deltaTime * (scroll > 0 ? 10 : 1);
        }
        else if (Input.GetKey(KeyCode.Minus) || scroll < 0) // '-' key or scroll down
        {
            topDownCamera.orthographicSize += zoomSpeed * Time.deltaTime * (scroll < 0 ? 10 : 1);
        }

        topDownCamera.orthographicSize = Mathf.Clamp(topDownCamera.orthographicSize, minOrthoSize, maxOrthoSize);
    }

    void HandlePan()
    {
        if (Input.GetMouseButton(0)) // Left mouse button to pan
        {
            float h = -Input.GetAxis("Mouse X") * panSpeed;
            float v = -Input.GetAxis("Mouse Y") * panSpeed;

            UnityEngine.Vector3 newPosition = topDownCamera.transform.position + new UnityEngine.Vector3(h, 0, v);
            newPosition.x = Mathf.Clamp(newPosition.x, panLimitMin.x, panLimitMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, panLimitMin.z, panLimitMax.z);

            topDownCamera.transform.position = newPosition;
        }
    }

    void UpdateViewModeText()
    {
        if (viewModeText != null)
        {
            viewModeText.text = topDownCamera.enabled ? "Top View" : "Front View";
        }
    }

    public void ToggleView()
    {
        Debug.Log("CLicked");
        isTopView = !isTopView;
        mainCamera.enabled = !isTopView;
        topDownCamera.enabled = isTopView;
        AdjustTopDownCamera();
        UpdateViewModeText();
    }
}
