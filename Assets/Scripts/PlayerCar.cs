using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCar : MonoBehaviour
{
    public float speed;
    public Joystick joystick;
    public int level;
    private bool onRoad = true;
    private bool raceOver = false;
    private Track track;
    private Timer timer;
    private EnemyCarInstantiator instantiator;
    public Text gameOverText;
    public Text levelDisplay;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        levelDisplay.text = "LEVEL " + level;
        gameOverText.gameObject.SetActive(false);
        track = GameObject.Find("Track").GetComponent<Track>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        instantiator = GameObject.Find("EnemyInstantiator").GetComponent<EnemyCarInstantiator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!raceOver)
        {
            // float values only used for movement with keyboard on laptop
            float hDirection = Input.GetAxisRaw("Horizontal");
            float vDirection = Input.GetAxisRaw("Vertical");

            if (onRoad)
            {
                transform.Translate(Vector2.right * hDirection * Time.deltaTime * speed);
                transform.Translate(Vector2.right * joystick.Horizontal * Time.deltaTime * speed);
            }
            transform.Translate(Vector2.up * vDirection * Time.deltaTime * speed);
            transform.Translate(Vector2.up * joystick.Vertical * Time.deltaTime * speed);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        onRoad = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "leftBumper")
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
        }
        else { transform.Translate(Vector2.left * Time.deltaTime * speed); }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onRoad = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Finish Line")
        {
            gameOverText.text = "YOU WON!";
            timeOver();
            StartCoroutine("loadLevel");
        }
    }

    public void timeOver()
    {
        gameOverText.gameObject.SetActive(true);
        timer.gameOver = true;
        track.stopScrolling();
        raceOver = true;
    }

    IEnumerator loadLevel()
    {
        yield return new WaitForSeconds(3f);

        level++;
        levelDisplay.text = "LEVEL " + level;

        // allow player to control car again
        raceOver = false;

        // put car back at track start
        track.resetAfterFinish();

        // destroy all enemies and redo for new level
        instantiator.increaseEnemyCount();
        instantiator.instantiateEnemy();

        // reset timer
        timer.gameOver = false;
        timer.resetTimer();

        // reset and disable text overlay
        gameOverText.text = "TIME OVER";
        gameOverText.gameObject.SetActive(false);

        // reset minimap
        GameObject.Find("Player Indicator").GetComponent<MiniMapPlayerIndicator>().resetPos();
    }
}