using NUnit.Framework;
using UnityEngine;

public class OyunYoneticisiTests
{
    private OyunYoneticisi oyunYoneticisi;
    private GameObject testObject;

    [SetUp]
    public void Setup()
    {
        testObject = new GameObject("OyunYoneticisiTestObj");
        oyunYoneticisi = testObject.AddComponent<OyunYoneticisi>();

        // Mock game objects to avoid NullReferenceExceptions during ResetGameObjects
        oyunYoneticisi.ball = new GameObject("Ball");
        oyunYoneticisi.player = new GameObject("Player");
        oyunYoneticisi.aiPaddle = new GameObject("AI");

        // Mock UI elements to avoid NullReferenceExceptions during UpdateGameStatusUI
        var statusContainer = new GameObject("StatusContainer");
        oyunYoneticisi.gameStatusText = statusContainer.AddComponent<TMPro.TextMeshProUGUI>();

        var pScoreContainer = new GameObject("PScoreContainer");
        oyunYoneticisi.playerScoreText = pScoreContainer.AddComponent<TMPro.TextMeshProUGUI>();

        var aiScoreContainer = new GameObject("AIScoreContainer");
        oyunYoneticisi.aiScoreText = aiScoreContainer.AddComponent<TMPro.TextMeshProUGUI>();

        // Provide DilYoneticisi instance to avoid NullReferenceException in UpdateGameStatusUI
        if (DilYoneticisi.Instance == null)
        {
            var dilYoneticisiObj = new GameObject("DilYoneticisi");
            DilYoneticisi.Instance = dilYoneticisiObj.AddComponent<DilYoneticisi>();
            // Wake it up to initialize dictionaries
            dilYoneticisiObj.SendMessage("Awake");
        }

        // Directly initialize the dictionaries to prevent NullReferenceException in CeviriAl
        if (DilYoneticisi.Instance != null)
        {
            var field = typeof(DilYoneticisi).GetField("ingilizceSozluk", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(DilYoneticisi.Instance, new System.Collections.Generic.Dictionary<string, string>());

            var field2 = typeof(DilYoneticisi).GetField("turkceSozluk", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field2 != null) field2.SetValue(DilYoneticisi.Instance, new System.Collections.Generic.Dictionary<string, string>());
        }

        // Initialize basic state
        oyunYoneticisi.gameStarted = false;
        oyunYoneticisi.gameEnded = true;
        oyunYoneticisi.gamePaused = true;
        oyunYoneticisi.roundEnded = true;
    }

    [TearDown]
    public void Teardown()
    {
        if (testObject != null) Object.DestroyImmediate(testObject);
        if (oyunYoneticisi.ball != null) Object.DestroyImmediate(oyunYoneticisi.ball);
        if (oyunYoneticisi.player != null) Object.DestroyImmediate(oyunYoneticisi.player);
        if (oyunYoneticisi.aiPaddle != null) Object.DestroyImmediate(oyunYoneticisi.aiPaddle);

        if (oyunYoneticisi.gameStatusText != null && oyunYoneticisi.gameStatusText.gameObject != null)
            Object.DestroyImmediate(oyunYoneticisi.gameStatusText.gameObject);
        if (oyunYoneticisi.playerScoreText != null && oyunYoneticisi.playerScoreText.gameObject != null)
            Object.DestroyImmediate(oyunYoneticisi.playerScoreText.gameObject);
        if (oyunYoneticisi.aiScoreText != null && oyunYoneticisi.aiScoreText.gameObject != null)
            Object.DestroyImmediate(oyunYoneticisi.aiScoreText.gameObject);

        // Clean up DilYoneticisi
        if (DilYoneticisi.Instance != null)
        {
            Object.DestroyImmediate(DilYoneticisi.Instance.gameObject);
            DilYoneticisi.Instance = null;
        }
    }

    [Test]
    public void StartGame_WhenGameNotStarted_SetsCorrectState()
    {
        // Act
        oyunYoneticisi.StartGame();

        // Assert
        Assert.IsTrue(oyunYoneticisi.gameStarted, "gameStarted should be true");
        Assert.IsFalse(oyunYoneticisi.gameEnded, "gameEnded should be false");
        Assert.IsFalse(oyunYoneticisi.gamePaused, "gamePaused should be false");
        Assert.IsFalse(oyunYoneticisi.roundEnded, "roundEnded should be false");
    }

    [Test]
    public void StartGame_WhenGameAlreadyStarted_DoesNotChangeState()
    {
        // Arrange
        oyunYoneticisi.gameStarted = true;
        oyunYoneticisi.gameEnded = true; // Setup state that would be overridden if it ran
        oyunYoneticisi.gamePaused = true;
        oyunYoneticisi.roundEnded = true;

        // Act
        oyunYoneticisi.StartGame();

        // Assert - If StartGame returned early, these should still be true
        Assert.IsTrue(oyunYoneticisi.gameStarted, "gameStarted should still be true");
        Assert.IsTrue(oyunYoneticisi.gameEnded, "gameEnded should still be true because StartGame should have returned early");
        Assert.IsTrue(oyunYoneticisi.gamePaused, "gamePaused should still be true");
        Assert.IsTrue(oyunYoneticisi.roundEnded, "roundEnded should still be true");
    }
}
