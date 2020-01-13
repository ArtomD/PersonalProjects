using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{

    public GameObject gameMenu;
    public GameObject settingsMenu;
    public GameObject resumeButton;

    private ShipController ship;
    private Vector3 _direction;
    private Quaternion _lookRotation;

    private Vector2 direction;

    private bool mobile;

    // Start is called before the first frame update
    void Start()
    {
        ship = ShipController.getInstance();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            mobile = false;
        }
        else
        {
            mobile = true;
        }

        mobile = true ;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount == 1 && !ship.isDead)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 pos = Camera.main.ScreenToWorldPoint( touch.position);
            AsteroidController[] asteroids = FindObjectsOfType<AsteroidController>();
            float minDistance = 3.5f;
            int asteroidIndex = -1;
            for(int i =0; i< asteroids.Length; i++)
            {
                float distance = Vector3.Distance(pos, asteroids[i].transform.position);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    asteroidIndex = i;
                    
                }
            }
            if (asteroidIndex > -1)
            {
                ship.setAsteroidTarget(asteroids[asteroidIndex]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AsteroidController[] asteroids = FindObjectsOfType<AsteroidController>();
            float minDistance = 5f;
            int asteroidIndex = -1;
            for (int i = 0; i < asteroids.Length; i++)
            {
                float distance = Vector3.Distance(pos, asteroids[i].transform.position);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    asteroidIndex = i;

                }
            }
            
            if (asteroidIndex > -1)
            {
                ship.setAsteroidTarget(asteroids[asteroidIndex]);
            }
        } 
        
    }

    private void FixedUpdate()
    {
        Vector3 calib = new Vector3(PlayerPrefs.GetFloat("calibX", 0.0f), PlayerPrefs.GetFloat("calibY", 0.0f));
        direction = Input.acceleration - calib;
        if (mobile)

        {
            if (!ship.isDead) {
                if (direction.magnitude > 0.08)
                {
                    ship.moveShip(direction);
                    ship.rotateShipInstant(direction);
                }
                else if (direction.magnitude < 0.05)
                {
                    ship.stopShip();
                }
            }
            
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ship.moveShip(ship.transform.up);
            }
            if (Input.GetKey(KeyCode.W))
            {
                ship.moveShip(ship.transform.up * 5);
            }
            if (Input.GetKey(KeyCode.S))
            {
                ship.moveShip(-ship.transform.up * 5);
            }
            if (Input.GetKey(KeyCode.D))
            {
                ship.moveShip(ship.transform.right * 5);
            }
            if (Input.GetKey(KeyCode.A))
            {
                ship.moveShip(-ship.transform.right * 5);
            }
            else
            {
                ship.stopShip();
            }
        }
    }

    public void pause(bool canUnpause)
    {
        openIngameMenu();
        if (canUnpause)
        {
            resumeButton.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            resumeButton.SetActive(false);
        }
        
    }

    public void resume()
    {
        closeIngameMenu();
        Time.timeScale = 1f;
    }

    public void openIngameMenu()
    {
        gameMenu.SetActive(true);

    }

    public void restartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void openSettingsMenu()
    {
        settingsMenu.SetActive(true);
        closeIngameMenu();
    }

    public void calibrate()
    {
        Vector3 calib = Input.acceleration;
        PlayerPrefs.SetFloat("calibX", calib.x);
        PlayerPrefs.SetFloat("calibY", calib.y);
        PlayerPrefs.SetFloat("calibZ", calib.z);
        PlayerPrefs.Save();
    }

    public void closeSettingsMenu()
    {
        settingsMenu.SetActive(false);
        openIngameMenu();
    }

    public void closeIngameMenu()
    {
        gameMenu.SetActive(false);
    }


}
