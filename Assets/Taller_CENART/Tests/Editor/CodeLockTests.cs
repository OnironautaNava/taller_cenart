using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TallerCenart;
using UnityEngine;
using UnityEngine.TestTools;

public class CodeLockTests
{
    GameObject root;
    CodeLock codeLock;
    int solved, wrong;
    List<string> shown;

    [SetUp]
    public void Create()
    {
        root = new GameObject("Test");
        codeLock = root.AddComponent<CodeLock>();
        codeLock.code = "1994";
        solved = wrong = 0;
        shown = new List<string>();
        codeLock.OnSolved.AddListener(() => solved++);
        codeLock.OnWrong.AddListener(() => wrong++);
        codeLock.OnInputChanged.AddListener(shown.Add);
    }

    [TearDown]
    public void Destroy() => Object.DestroyImmediate(root);

    void Press(string digits)
    {
        foreach (var c in digits)
            codeLock.Press(c - '0');
    }

    [Test]
    public void PartialSequenceDoesNotSolve()
    {
        Press("199");
        Assert.IsFalse(codeLock.IsSolved);
        Assert.AreEqual(0, solved);
        Assert.AreEqual("199", codeLock.Input);
        Assert.AreEqual("199_", shown[shown.Count - 1], "display pads missing digits");
    }

    [Test]
    public void CorrectOrderedSequenceSolvesOnce()
    {
        Press("1994");
        Assert.IsTrue(codeLock.IsSolved);
        Assert.AreEqual(1, solved);
        Assert.AreEqual(0, wrong);
    }

    [Test]
    public void WrongOrderClearsInputAndFiresWrong()
    {
        Press("9194");
        Assert.IsFalse(codeLock.IsSolved);
        Assert.AreEqual(1, wrong);
        Assert.AreEqual("", codeLock.Input);
        Assert.AreEqual("____", shown[shown.Count - 1]);
    }

    [Test]
    public void RetryAfterWrongEntrySolves()
    {
        Press("1234");
        Press("1994");
        Assert.IsTrue(codeLock.IsSolved);
        Assert.AreEqual(1, wrong);
        Assert.AreEqual(1, solved);
    }

    [Test]
    public void ClearDiscardsPartialInput()
    {
        Press("19");
        codeLock.Clear();
        Press("1994");
        Assert.AreEqual(1, solved);
        Assert.AreEqual(0, wrong);
    }

    [Test]
    public void InputAfterSolveIsIgnored()
    {
        Press("1994");
        var shownBefore = shown.Count;
        Press("1234");
        codeLock.Clear();
        Assert.AreEqual(1, solved);
        Assert.AreEqual(0, wrong);
        Assert.AreEqual("1994", codeLock.Input);
        Assert.AreEqual(shownBefore, shown.Count, "display keeps the solved code");
    }

    [Test]
    public void InvalidCodeNeverSolvesAndLogsError()
    {
        codeLock.code = "19a4";
        LogAssert.Expect(LogType.Error, new Regex("digits"));
        Press("1");
        Assert.IsFalse(codeLock.IsSolved);
        Assert.AreEqual("", codeLock.Input);
    }

    [Test]
    public void OutOfRangeDigitIsRejected()
    {
        LogAssert.Expect(LogType.Error, new Regex("0-9"));
        codeLock.Press(12);
        Assert.AreEqual("", codeLock.Input);
    }
}
