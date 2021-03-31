using System;
using DiamondEngine;

public class FPS_Controller : DiamondComponent
{
    public GameObject reference = null;
    public GameObject turret = null;
    public GameObject quitMenu = null;
    public GameObject win = null;
    public GameObject lose = null;
    public GameObject default_selected = null;
    public GameObject background = null;

    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;

    int X_dir = 0, Z_dir = 0, lastX_dir = 0, lastZ_dir = 0;
    bool dashing = false;
    float dashCD = 0.33f;
    float dashCD_counter = 0.0f;
    float dashDuration = 0.25f;
    float dashingCounter = 0.0f;
    //public void OnCollisionEnter(GameObject collidedGameObject)
    //{
    //    Debug.Log("Ayo i've been called.");
    //    InternalCalls.Destroy(reference);
    //}

    public void Update()
	{
        if (this.reference == null)
            return;

        if (Input.GetKey(DEKeyCode.W) == KeyState.KEY_REPEAT)
            gameObject.transform.localPosition += gameObject.transform.GetForward() * movementSpeed * Time.deltaTime;
        if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_REPEAT)
            gameObject.transform.localPosition += gameObject.transform.GetForward() * -movementSpeed * Time.deltaTime;
        if (Input.GetKey(DEKeyCode.A) == KeyState.KEY_REPEAT)
            gameObject.transform.localPosition += gameObject.transform.GetRight() * movementSpeed * Time.deltaTime;
        if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_REPEAT)
            gameObject.transform.localPosition += gameObject.transform.GetRight() * -movementSpeed * Time.deltaTime;

        if (Input.GetMouseX() != 0 && turret != null)
            turret.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -Input.GetMouseX() * mouseSens * Time.deltaTime) * turret.transform.localRotation;

        if (Input.GetKey(DEKeyCode.B) == KeyState.KEY_DOWN)
            Counter.SumToCounterType(Counter.CounterTypes.BOKATAN_RES);
        if (Input.GetKey(DEKeyCode.X) == KeyState.KEY_DOWN)
            Counter.SumToCounterType(Counter.CounterTypes.WRECKER_RES);
        if (Input.GetKey(DEKeyCode.Y) == KeyState.KEY_DOWN)
            Counter.SumToCounterType(Counter.CounterTypes.ENEMY_BANTHA);
        if (Input.GetKey(DEKeyCode.H) == KeyState.KEY_DOWN)
            Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOP);
        if (Input.GetKey(DEKeyCode.Z) == KeyState.KEY_DOWN)
            Show();
        if (Input.GetKey(DEKeyCode.R) == KeyState.KEY_DOWN)
            Counter.ResetCounters();

            if (Input.GetKey(DEKeyCode.W) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetForward() * movementSpeed * Time.deltaTime;
                lastZ_dir = Z_dir = 1;
            }
            if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetForward() * -movementSpeed * Time.deltaTime;
                lastZ_dir = Z_dir = -1;
            }
            if (Input.GetKey(DEKeyCode.A) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetRight() * movementSpeed * Time.deltaTime;
                lastX_dir = X_dir = 1;
            }
            if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetRight() * -movementSpeed * Time.deltaTime;
                lastX_dir = X_dir = -1;
            }

        }

        if (Input.GetMouseX() != 0 && turret != null)
            turret.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -Input.GetMouseX() * mouseSens * Time.deltaTime) * turret.transform.localRotation;

        if (Input.GetGamepadButton(DEControllerButton.START) == KeyState.KEY_DOWN)
        {
            if (quitMenu.IsEnabled())
                quitMenu.Enable(false);
            else {
                quitMenu.Enable(true);
                quitMenu.GetComponent<Pause>().DisplayBoons();
                background.Enable(true);
                Time.PauseGame();
                if (default_selected != null)
                    default_selected.GetComponent<Navigation>().Select();
            }

        }

        if (Input.GetKey(DEKeyCode.F1)== KeyState.KEY_DOWN && !lose.IsEnabled())
        {
            win.Enable(true);
        }
        if (Input.GetKey(DEKeyCode.F2)==KeyState.KEY_DOWN && !win.IsEnabled())
        {
            lose.Enable(true);
        }
        if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN && (win.IsEnabled() || lose.IsEnabled()))
        {
            SceneManager.LoadScene(1726826608);
        }
    }

    private void Dash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            //Diagonal movement has to be less in each axis to be equal to one axis movement
            //Last dir is used for when character is not moving --> moves in last direction
            if(Z_dir == 0 && X_dir == 0)
            {
                //Use last known direction
                if (lastZ_dir == 0 || lastX_dir == 0)
                {
                    if (lastZ_dir == 0) gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 2) * Time.deltaTime * lastX_dir;
                    else gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 2) * Time.deltaTime * lastZ_dir;
                }
                else
                {
                    gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 1.3f) * Time.deltaTime * lastZ_dir;
                    gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 1.3f) * Time.deltaTime * lastX_dir;
                }
            }
            else if(Z_dir == 0 || X_dir == 0)
            {
                if(Z_dir == 0) gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 2) * Time.deltaTime * X_dir;
                else gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 2) * Time.deltaTime * Z_dir;
            }
            else
            {
                gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 1.3f) * Time.deltaTime * Z_dir;
                gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 1.3f) * Time.deltaTime * X_dir;
            }
        }
        if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN && (win.IsEnabled() || lose.IsEnabled()))
        {
            SceneManager.LoadScene(1726826608);
        }
    }
   
}