using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] objects;
    private GameObject pendingObject;

    private Vector3 position;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;

    public float gridSize;
    private bool gridOn;
    [SerializeField] private Toggle gridToggle;

    public bool canPlace = true;

    [SerializeField] private Material[] materials;
    // Update is called once per frame
    void Update()
    {
        if (pendingObject != null)
        {
            if (gridOn)
            {
                pendingObject.transform.position = new Vector3(RoundToNearestGrid(position.x), 0.51f, RoundToNearestGrid(position.z));
            }
            else
                pendingObject.transform.position = position;

            UpdateMaterials();

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }
            if (Input.GetKeyDown(KeyCode.R))
                RotateObject();

        }
    }
    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,1000, layerMask))
        {
            position = hit.point;
            position.y += 0.55f;
        }
    }
    void UpdateMaterials()
    {
        if (canPlace)
            pendingObject.GetComponent<MeshRenderer>().material = materials[0];
        else
            pendingObject.GetComponent<MeshRenderer>().material = materials[1];

    }
    public void PlaceObject()
    {
        pendingObject.GetComponent<MeshRenderer>().material = materials[2];
        pendingObject = null;
    }
    public void SelectObject(int index)
    {
        pendingObject = Instantiate(objects[index], position, transform.rotation);
    }
    public void ToggleGrid()
    {
        if (gridToggle.isOn)
            gridOn = true;
        else
            gridOn = false;
    }
    private float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if(xDiff > (gridSize / 2)){
            pos += gridSize;
        }
        return pos;
    }
    public void RotateObject()
    {
        pendingObject.transform.Rotate(Vector3.up, 45);
    }
}
