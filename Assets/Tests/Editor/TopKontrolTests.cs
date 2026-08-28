using NUnit.Framework;
using UnityEngine;

public class TopKontrolTests
{
    private GameObject gameManagerObject;
    private GameObject ballObject;
    private TopKontrol topKontrol;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        // TopKontrol depends on OyunYoneticisi in Start(), though ResetBall doesn't need it.
        // It's safer to provide it just in case Unity calls Start() before we test.
        gameManagerObject = new GameObject("OyunYoneticisi");
        gameManagerObject.AddComponent<OyunYoneticisi>();

        ballObject = new GameObject("Ball");
        rb = ballObject.AddComponent<Rigidbody2D>();
        topKontrol = ballObject.AddComponent<TopKontrol>();
    }

    [TearDown]
    public void Teardown()
    {
        if (ballObject != null)
        {
            Object.DestroyImmediate(ballObject);
        }
        if (gameManagerObject != null)
        {
            Object.DestroyImmediate(gameManagerObject);
        }
    }

    [Test]
    public void ResetBall_ResetsPositionVelocityAndLastHitter()
    {
        // Arrange
        // We set some non-default states
        ballObject.transform.position = new Vector3(5f, 5f, 0f);
        rb.linearVelocity = new Vector2(10f, 10f);
        topKontrol.lastHitter = "Player";

        // Act
        topKontrol.ResetBall();

        // Assert
        Assert.AreEqual(Vector3.zero, ballObject.transform.position, "Ball position should be reset to zero.");
        Assert.AreEqual(Vector2.zero, rb.linearVelocity, "Ball velocity should be reset to zero.");
        Assert.AreEqual("", topKontrol.lastHitter, "lastHitter should be empty.");
    }
}
