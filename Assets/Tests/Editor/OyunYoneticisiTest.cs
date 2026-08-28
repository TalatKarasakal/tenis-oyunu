using NUnit.Framework;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.TestTools;

public class OyunYoneticisiTest
{
    private GameObject oyunYoneticisiObject;
    private OyunYoneticisi oyunYoneticisi;
    private GameObject dilYoneticisiObject;

    [SetUp]
    public void SetUp()
    {
        // Setup DilYoneticisi Singleton to avoid NullReferenceExceptions during UI updates
        if (DilYoneticisi.Instance == null)
        {
            dilYoneticisiObject = new GameObject();
            dilYoneticisiObject.AddComponent<DilYoneticisi>();
            // The Awake method of DilYoneticisi will set Instance and initialize dictionaries
        }

        oyunYoneticisiObject = new GameObject();
        oyunYoneticisi = oyunYoneticisiObject.AddComponent<OyunYoneticisi>();

        // Mock required UI components
        oyunYoneticisi.playerScoreText = new GameObject().AddComponent<TextMeshProUGUI>();
        oyunYoneticisi.aiScoreText = new GameObject().AddComponent<TextMeshProUGUI>();
        oyunYoneticisi.gameStatusText = new GameObject().AddComponent<TextMeshProUGUI>();

        // Set minimal game objects so the Initialize/Start routines don't crash
        // Not strictly necessary since we test core logic, but better safe.
        // We will just let StartGame run and not actually let the GameObjects interact
        oyunYoneticisi.ball = new GameObject();
        oyunYoneticisi.ball.AddComponent<Rigidbody2D>();
        oyunYoneticisi.ball.AddComponent<TopKontrol>();

        oyunYoneticisi.player = new GameObject();
        oyunYoneticisi.player.AddComponent<Rigidbody2D>();
        oyunYoneticisi.player.AddComponent<OyuncuKontrol>();

        oyunYoneticisi.aiPaddle = new GameObject();
        oyunYoneticisi.aiPaddle.AddComponent<Rigidbody2D>();
        oyunYoneticisi.aiPaddle.AddComponent<YapayZekaKontrol>();
        oyunYoneticisi.aiPaddle.AddComponent<SpriteRenderer>(); // Needed for color reset in YapayZekaKontrol
    }

    [TearDown]
    public void TearDown()
    {
        if (oyunYoneticisiObject != null)
        {
            if (oyunYoneticisi.playerScoreText != null) Object.DestroyImmediate(oyunYoneticisi.playerScoreText.gameObject);
            if (oyunYoneticisi.aiScoreText != null) Object.DestroyImmediate(oyunYoneticisi.aiScoreText.gameObject);
            if (oyunYoneticisi.gameStatusText != null) Object.DestroyImmediate(oyunYoneticisi.gameStatusText.gameObject);

            if (oyunYoneticisi.ball != null) Object.DestroyImmediate(oyunYoneticisi.ball);
            if (oyunYoneticisi.player != null) Object.DestroyImmediate(oyunYoneticisi.player);
            if (oyunYoneticisi.aiPaddle != null) Object.DestroyImmediate(oyunYoneticisi.aiPaddle);

            Object.DestroyImmediate(oyunYoneticisiObject);
        }

        if (dilYoneticisiObject != null)
        {
            Object.DestroyImmediate(dilYoneticisiObject);
        }
        else if (DilYoneticisi.Instance != null)
        {
             Object.DestroyImmediate(DilYoneticisi.Instance.gameObject);
        }
    }

    [Test]
    public void StartGame_SetsGameStartedToTrueAndResetsState()
    {
        oyunYoneticisi.gameEnded = true;
        oyunYoneticisi.gamePaused = true;
        oyunYoneticisi.StartGame();

        Assert.IsTrue(oyunYoneticisi.gameStarted);
        Assert.IsFalse(oyunYoneticisi.gameEnded);
        Assert.IsFalse(oyunYoneticisi.gamePaused);
        Assert.IsFalse(oyunYoneticisi.roundEnded);
    }

    [Test]
    public void PauseGame_TogglesGamePausedAndTimeScale()
    {
        oyunYoneticisi.StartGame();

        // Initial state is not paused
        Assert.IsFalse(oyunYoneticisi.gamePaused);

        // Pause
        oyunYoneticisi.PauseGame();
        Assert.IsTrue(oyunYoneticisi.gamePaused);
        Assert.AreEqual(0f, Time.timeScale);

        // Unpause via PauseGame toggle
        oyunYoneticisi.PauseGame();
        Assert.IsFalse(oyunYoneticisi.gamePaused);
        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void ResumeGame_UnpausesGameAndResetsTimeScale()
    {
        oyunYoneticisi.StartGame();
        oyunYoneticisi.PauseGame();

        oyunYoneticisi.ResumeGame();

        Assert.IsFalse(oyunYoneticisi.gamePaused);
        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void StopGame_ResetsAllGameFlags()
    {
        oyunYoneticisi.StartGame();
        oyunYoneticisi.StopGame();

        Assert.IsFalse(oyunYoneticisi.gameStarted);
        Assert.IsFalse(oyunYoneticisi.gameEnded);
        Assert.IsFalse(oyunYoneticisi.gamePaused);
        Assert.IsFalse(oyunYoneticisi.roundEnded);
        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void GetWinner_ReturnsCorrectWinnerString()
    {
        oyunYoneticisi.scoreToWin = 5;

        oyunYoneticisi.playerScore = 5;
        oyunYoneticisi.aiScore = 3;
        Assert.AreEqual("Player", oyunYoneticisi.GetWinner());

        oyunYoneticisi.playerScore = 2;
        oyunYoneticisi.aiScore = 5;
        Assert.AreEqual("AI", oyunYoneticisi.GetWinner());

        oyunYoneticisi.playerScore = 2;
        oyunYoneticisi.aiScore = 3;
        Assert.AreEqual("", oyunYoneticisi.GetWinner());
    }

    [Test]
    public void IsGameActive_ReturnsTrueOnlyWhenPlaying()
    {
        // Not started
        oyunYoneticisi.gameStarted = false;
        oyunYoneticisi.gameEnded = false;
        oyunYoneticisi.gamePaused = false;
        Assert.IsFalse(oyunYoneticisi.IsGameActive());

        // Started
        oyunYoneticisi.gameStarted = true;
        Assert.IsTrue(oyunYoneticisi.IsGameActive());

        // Paused
        oyunYoneticisi.gamePaused = true;
        Assert.IsFalse(oyunYoneticisi.IsGameActive());

        // Ended
        oyunYoneticisi.gamePaused = false;
        oyunYoneticisi.gameEnded = true;
        Assert.IsFalse(oyunYoneticisi.IsGameActive());
    }

    [Test]
    public void RestartGame_ResetsScoresAndRounds()
    {
        // Modify some state
        oyunYoneticisi.playerScore = 3;
        oyunYoneticisi.aiScore = 2;
        oyunYoneticisi.currentRound = 5;

        // Restarting calls InitializeGame & StartGame
        oyunYoneticisi.RestartGame();

        Assert.AreEqual(0, oyunYoneticisi.playerScore);
        Assert.AreEqual(0, oyunYoneticisi.aiScore);
        Assert.AreEqual(1, oyunYoneticisi.currentRound);
        Assert.IsTrue(oyunYoneticisi.gameStarted);
    }
}
