using System.Text.RegularExpressions;
using NUnit.Framework;
using TallerCenart;
using UnityEngine;
using UnityEngine.TestTools;

public class PuzzleComponentTests
{
    GameObject root;

    [SetUp]
    public void Create() => root = new GameObject("Test");

    [TearDown]
    public void Destroy() => Object.DestroyImmediate(root);

    PuzzleCondition Condition(int required, out int[] solvedCount)
    {
        var condition = root.AddComponent<PuzzleCondition>();
        condition.requiredCount = required;
        var count = solvedCount = new int[1];
        condition.OnSolved.AddListener(() => count[0]++);
        return condition;
    }

    [Test]
    public void FewerThanRequiredSignalsDoNotSolve()
    {
        var condition = Condition(2, out var solved);
        condition.Signal();
        Assert.AreEqual(1, condition.Count);
        Assert.IsFalse(condition.IsSolved);
        Assert.AreEqual(0, solved[0]);
    }

    [Test]
    public void ReachingRequiredSignalsSolvesExactlyOnce()
    {
        var condition = Condition(2, out var solved);
        condition.Signal();
        condition.Signal();
        condition.Signal();
        condition.Unsignal();
        condition.Signal();
        Assert.IsTrue(condition.IsSolved);
        Assert.AreEqual(1, solved[0]);
    }

    [Test]
    public void UnsignalBeforeSolvedUndoesTheSignal()
    {
        var condition = Condition(2, out var solved);
        condition.Signal();
        condition.Unsignal();
        condition.Unsignal();
        condition.Signal();
        Assert.AreEqual(1, condition.Count);
        Assert.AreEqual(0, solved[0]);
    }

    [Test]
    public void InvalidRequiredCountNeverSolvesAndLogsError()
    {
        var condition = Condition(0, out var solved);
        LogAssert.Expect(LogType.Error, new Regex("Required Count must be at least 1"));
        condition.Signal();
        Assert.IsFalse(condition.IsSolved);
        Assert.AreEqual(0, solved[0]);
    }

    [Test]
    public void DoorOpensAndClosesTargetPose()
    {
        var lid = new GameObject("Lid").transform;
        lid.SetParent(root.transform);
        lid.localPosition = new Vector3(0, 1, 0);
        var door = root.AddComponent<Door>();
        door.target = lid;
        door.openOffset = new Vector3(0, 0.5f, 0);
        door.openRotation = new Vector3(-90, 0, 0);
        door.duration = 0;

        door.Open();
        Assert.IsTrue(door.IsOpen);
        Assert.That(Vector3.Distance(lid.localPosition, new Vector3(0, 1.5f, 0)), Is.LessThan(1e-4f));
        Assert.That(Quaternion.Angle(lid.localRotation, Quaternion.Euler(-90, 0, 0)), Is.LessThan(0.01f));

        door.Close();
        Assert.IsFalse(door.IsOpen);
        Assert.That(Vector3.Distance(lid.localPosition, new Vector3(0, 1, 0)), Is.LessThan(1e-4f));
        Assert.That(Quaternion.Angle(lid.localRotation, Quaternion.identity), Is.LessThan(0.01f));
    }

    [Test]
    public void DoorWithoutTargetMovesItself()
    {
        var door = root.AddComponent<Door>();
        door.openOffset = Vector3.up;
        door.duration = 0;
        door.Open();
        Assert.AreEqual(root.transform, door.target);
        Assert.That(Vector3.Distance(root.transform.localPosition, Vector3.up), Is.LessThan(1e-4f));
    }

    [Test]
    public void RevealShowsAndHidesTargetsAndReportsEmptySlots()
    {
        var hidden = new GameObject("Hidden");
        hidden.transform.SetParent(root.transform);
        hidden.SetActive(false);
        var reveal = root.AddComponent<Reveal>();
        reveal.targets = new[] { null, hidden };

        LogAssert.Expect(LogType.Warning, new Regex("empty target slot"));
        reveal.Show();
        Assert.IsTrue(hidden.activeSelf);

        LogAssert.Expect(LogType.Warning, new Regex("empty target slot"));
        reveal.Hide();
        Assert.IsFalse(hidden.activeSelf);
    }

    [Test]
    public void RevealWithNoTargetsWarns()
    {
        var reveal = root.AddComponent<Reveal>();
        LogAssert.Expect(LogType.Warning, new Regex("no targets"));
        reveal.Show();
    }
}
