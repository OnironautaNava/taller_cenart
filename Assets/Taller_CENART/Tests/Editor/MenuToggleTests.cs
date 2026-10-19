using NUnit.Framework;
using TallerCenart;
using UnityEngine;

public class MenuToggleTests
{
    GameObject root, menu, head;
    MenuToggle toggle;

    [SetUp]
    public void Create()
    {
        root = new GameObject("Test");
        menu = new GameObject("Menu");
        menu.SetActive(false);
        head = new GameObject("Head");
        head.transform.SetPositionAndRotation(new Vector3(1, 1.6f, 2), Quaternion.Euler(0, 90, 0));
        toggle = root.AddComponent<MenuToggle>();
        toggle.menu = menu;
        toggle.head = head.transform;
        toggle.distance = 0.8f;
    }

    [TearDown]
    public void Destroy()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(menu);
        Object.DestroyImmediate(head);
    }

    [Test]
    public void ToggleAlternatesMenuState()
    {
        toggle.Toggle();
        Assert.IsTrue(menu.activeSelf);
        toggle.Toggle();
        Assert.IsFalse(menu.activeSelf);
    }

    [Test]
    public void OpeningPlacesMenuInFrontOfHeadFacingIt()
    {
        toggle.Toggle();
        Assert.That(Vector3.Distance(new Vector3(1.8f, 1.6f, 2), menu.transform.position), Is.LessThan(0.001f));
        Assert.That(Vector3.Dot(menu.transform.forward, Vector3.right), Is.GreaterThan(0.999f), "UI faces away from the head so its front is readable");
    }

    [Test]
    public void OpeningIgnoresHeadPitch()
    {
        head.transform.rotation = Quaternion.Euler(60, 90, 0);
        toggle.Toggle();
        Assert.That(Mathf.Abs(menu.transform.position.y - 1.6f), Is.LessThan(0.001f), "menu stays at eye height");
        Assert.That(Mathf.Abs(menu.transform.forward.y), Is.LessThan(0.001f), "menu stays upright");
    }

    [Test]
    public void DefaultActionBindsMenuButtonOfBothControllers()
    {
        var bindings = toggle.toggleAction.action.bindings;
        Assert.AreEqual(2, bindings.Count);
        StringAssert.Contains("{LeftHand}/{MenuButton}", bindings[0].path);
        StringAssert.Contains("{RightHand}/{MenuButton}", bindings[1].path, "simulator manipulates the right controller by default");
    }
}
