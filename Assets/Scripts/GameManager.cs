using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool onBeat;
    [SerializeField] float beatInterval = 1f;
    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;
    private Camera cam;

    //Används ej just nu, men ni kan använda de senare
    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        onBeat = false;
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        bunkers = FindObjectsOfType<Bunker>();
        cam = Camera.main;
        StartCoroutine(ToTheBeat());
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }
    }

    private IEnumerator ToTheBeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(beatInterval);
            onBeat = true;
            foreach (GameObject invader in Invaders.invaders)
            {
                Vector2 size = invader.transform.localScale;
                invader.transform.localScale = size * 1.5f;
            }
            yield return new WaitForSeconds(0.1f);
            onBeat = false;
            foreach (GameObject invader in Invaders.invaders)
            {
                Vector2 size = invader.transform.localScale;
                invader.transform.localScale = size / 1.5f;
            }
        }
    }

    public void ScreenThrust(float intensity, float duration)
    {
        StopCoroutine("ScreenThrustCo");
        StartCoroutine(ScreenThrustCo(intensity, duration));
    }

    private IEnumerator ScreenThrustCo(float intensity, float duration)
    {
        float zoom = cam.orthographicSize;
        cam.orthographicSize = zoom - (intensity / 10);
        yield return new WaitForSeconds(duration);
        cam.orthographicSize = zoom;
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        
    }

    private void SetLives(int lives)
    {
       
    }

    public void OnPlayerKilled(Player player)
    {

        player.gameObject.SetActive(false);

    }

    public void OnInvaderKilled(Invader invader)
    {
        invader.gameObject.SetActive(false);

       

        if (invaders.GetInvaderCount() == 0)
        {
            NewRound();
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        mysteryShip.gameObject.SetActive(false);
    }

    public void OnBoundaryReached()
    {
        if (invaders.gameObject.activeSelf)
        {
            invaders.gameObject.SetActive(false);
            OnPlayerKilled(player);
        }
    }

}
