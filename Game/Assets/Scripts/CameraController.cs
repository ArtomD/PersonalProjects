using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private ShipController ship;

    // Start is called before the first frame update
    void Start()
    {
        ship = ShipController.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3 (ship.transform.position.x, ship.transform.position.y, -10);
    }
}
