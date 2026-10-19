using NUnit.Framework;
using TallerCenart;
using UnityEngine;

public class TargetHitAndTimerTests
{
    GameObject root;

    [SetUp]
    public void Create() => root = new GameObject("Test");

    [TearDown]
    public void Destroy() => Object.DestroyImmediate(root);

    [Test]
    public void TargetFiresOnEveryRigidbodyHitAndIgnoresStaticColliders()
    {
        var target = root.AddComponent<TargetHit>();
        var hits = 0;
        target.OnHit.AddListener(() => hits++);
        target.Hit(null);
        Assert.AreEqual(0, hits, "a collider without Rigidbody (wall, floor) is not a throw");
        var ball = new GameObject("Ball").AddComponent<Rigidbody>();
        target.Hit(ball);
        target.Hit(ball);
        Object.DestroyImmediate(ball.gameObject);
        Assert.AreEqual(2, hits, "every hit is reported; PuzzleCondition keeps the solve single");
    }

    Timer Timer(float duration, out int[] timeUp, out string[] text)
    {
        var timer = root.AddComponent<Timer>();
        timer.duration = duration;
        var up = timeUp = new int[1];
        var shown = text = new string[1];
        timer.OnTimeUp.AddListener(() => up[0]++);
        timer.OnTextChanged.AddListener(s => shown[0] = s);
        return timer;
    }

    [Test]
    public void TimerDoesNothingUntilStarted()
    {
        var timer = Timer(5, out var up, out _);
        timer.Tick(10);
        Assert.IsFalse(timer.IsRunning);
        Assert.AreEqual(0, up[0]);
    }

    [Test]
    public void TimerCountsDownAndFiresTimeUpOnceAtZero()
    {
        var timer = Timer(90, out var up, out var text);
        timer.StartTimer();
        Assert.AreEqual("01:30", text[0]);
        timer.Tick(29.5f);
        Assert.AreEqual("01:01", text[0], "rounds partial seconds up");
        Assert.AreEqual(0, up[0]);
        timer.Tick(61);
        Assert.AreEqual("00:00", text[0]);
        Assert.AreEqual(0f, timer.Remaining);
        Assert.IsFalse(timer.IsRunning);
        timer.Tick(5);
        Assert.AreEqual(1, up[0]);
    }

    [Test]
    public void TimerRestartsFromFullDurationAndStopPauses()
    {
        var timer = Timer(10, out var up, out _);
        timer.StartTimer();
        timer.Tick(4);
        timer.Stop();
        timer.Tick(100);
        Assert.AreEqual(6f, timer.Remaining, 0.001f);
        Assert.AreEqual(0, up[0]);
        timer.StartTimer();
        Assert.AreEqual(10f, timer.Remaining);
        timer.Tick(10);
        Assert.AreEqual(1, up[0]);
        timer.StartTimer();
        timer.Tick(10);
        Assert.AreEqual(2, up[0], "a restarted timer can expire again");
    }
}
