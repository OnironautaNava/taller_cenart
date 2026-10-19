using NUnit.Framework;
using TallerCenart;
using UnityEngine;

public class PlayerHeightFilterTests
{
    GameObject root;
    Transform player;

    [SetUp]
    public void Create()
    {
        root = new GameObject("Test");
        player = new GameObject("Player").transform;
        player.SetParent(root.transform);
    }

    [TearDown]
    public void Destroy() => Object.DestroyImmediate(root);

    [Test]
    public void AllowsOnlyWhenThePlayerIsAtOrAboveMinHeight()
    {
        var filter = root.AddComponent<PlayerHeightFilter>();
        filter.player = player;
        filter.minHeight = 1.5f;
        player.position = Vector3.zero;
        Assert.IsFalse(filter.Allows(), "from the floor the high surface is not a destination");
        player.position = new Vector3(0, 1.5f, 0);
        Assert.IsTrue(filter.Allows());
        player.position = new Vector3(0, 2f, 0);
        Assert.IsTrue(filter.Allows(), "on the platform the player moves freely");
    }

    [Test]
    public void WithoutPlayerItDoesNotBlockMovement()
    {
        var filter = root.AddComponent<PlayerHeightFilter>();
        Assert.IsTrue(filter.Allows());
    }
}
