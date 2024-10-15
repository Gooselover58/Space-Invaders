using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using TMPro;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public bool onBeat;
    [HideInInspector] public float pitch;
    [SerializeField] float beatInterval;
    [SerializeField] float beatDuration;
    [SerializeField] TextMeshProUGUI roundText;
    private Player player;
    private Invaders invaders;
    private Bunker[] bunkers;
    private Camera cam;
    private ParticleSystem deathParticles;
    private ParticleSystem collisionParticles;
    private AudioSource hitSound;
    private AudioClip shootSound;
    private int round;
    private int playerLives;
    private int playerScore;
    public bool playerDead;

    //Används ej just nu, men ni kan använda de senare
    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private void Awake()
    {
        playerDead = false;
        collisionParticles = GameObject.Find("CollisionParticles").GetComponent<ParticleSystem>();
        shootSound = Resources.Load<AudioClip>("Sound/35678__jobro__laser10");
        roundText.gameObject.SetActive(false);
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
        bunkers = FindObjectsOfType<Bunker>();
        cam = Camera.main;
        deathParticles = GameObject.Find("DeathParticles").GetComponent<ParticleSystem>();
        hitSound = GameObject.Find("SoundPlayer").GetComponent<AudioSource>();
        StartCoroutine(ToTheBeat());
        NewGame();
    }

    private void Update()
    {
        if (playerDead && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }
    }

    private IEnumerator ToTheBeat()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            onBeat = true;
            foreach (GameObject invader in Invaders.invaders)
            {
                Vector2 size = invader.transform.localScale;
                invader.transform.localScale = size * 1.5f;
            }
            yield return new WaitForSeconds(beatDuration);
            onBeat = false;
            foreach (GameObject invader in Invaders.invaders)
            {
                Vector2 size = invader.transform.localScale;
                invader.transform.localScale = size / 1.5f;
            }
            yield return new WaitForSeconds(beatInterval);
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

    private void PlayDeathParticles(Vector2 pos)
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.applyShapeToPosition = true;
        emitParams.position = pos;
        deathParticles.Emit(emitParams, 20);
    }

    private void NewGame()
    {
        round = 1;
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        StartCoroutine(RoundText());
    }

    private IEnumerator RoundText()
    {
        roundText.text = $"Round {round}";
        roundText.gameObject.SetActive(true);
        round++;
        yield return new WaitForSeconds(2);
        roundText.gameObject.SetActive(false);
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }
        //StartCoroutine(RoundText());
        invaders.CreateInvaderGrid();
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
        playerScore = score;
    }

    private void SetLives(int lives)
    {
        playerLives = lives;
    }

    public void ChangeLives(int change)
    {
        playerLives -= change;
    }

    public void OnPlayerKilled(Player player)
    {
        player.gameObject.SetActive(false);
    }

    public void PlayShootSound()
    {
        float thisPitch = Random.Range(0.2f, 0.8f);
        hitSound.PlayOneShot(shootSound);
    }

    public void OnInvaderKilled(Invader invader)
    {
        PlayDeathParticles(invader.transform.position);
        hitSound.pitch = pitch;
        hitSound.PlayOneShot(hitSound.clip, pitch);
        pitch += 0.05f;
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

    public void PlayCollisionParts(Vector2 pos)
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.applyShapeToPosition = true;
        emitParams.position = pos;
        collisionParticles.Emit(emitParams, 100);
    }
}
