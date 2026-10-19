using System.Linq;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using TallerCenart;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class Fase0SceneTests
{
    const string ScenePath = "Assets/Taller_CENART/Scenes/Fase0.unity";

    [OneTimeSetUp]
    public void Open() => EditorSceneManager.OpenScene(ScenePath);

    [Test]
    public void SimulatorIsAutoInstantiatedInEditorOnly()
    {
        // XRDeviceSimulatorSettings is internal to XRI, so read its serialized fields.
        var settings = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>("Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset"));
        Assert.IsTrue(settings.FindProperty("m_AutomaticallyInstantiateSimulatorPrefab").boolValue);
        Assert.IsTrue(settings.FindProperty("m_AutomaticallyInstantiateInEditorOnly").boolValue, "simulator must stay out of player builds");
        Assert.IsNotNull(settings.FindProperty("m_SimulatorPrefab").objectReferenceValue);
    }

    [Test]
    public void SceneHasSingleRigAndManager()
    {
        Assert.AreEqual(1, Object.FindObjectsByType<XROrigin>(FindObjectsSortMode.None).Length);
        Assert.AreEqual(1, Object.FindObjectsByType<XRInteractionManager>(FindObjectsSortMode.None).Length);
    }

    [TestCase("Zone_A_Entrance")]
    [TestCase("Zone_B_Gallery")]
    public void ZoneHasTeleportableFloor(string zone)
    {
        var go = GameObject.Find("Environment/" + zone);
        Assert.IsNotNull(go, zone);
        var areas = go.GetComponentsInChildren<TeleportationArea>();
        Assert.IsNotEmpty(areas);
        Assert.IsTrue(areas.All(a => a.GetComponentInChildren<Collider>() != null));
    }

    [Test]
    public void ThreeChallengeSlotsArePedestalPrefabInstances()
    {
        var pedestals = new[] { "Zone_A_Entrance/Challenge_01", "Zone_B_Gallery/Challenge_02", "Zone_B_Gallery/Challenge_03" }
            .Select(p => GameObject.Find("Environment/" + p)).ToArray();
        Assert.IsTrue(pedestals.All(p => p != null));
        Assert.IsTrue(pedestals.All(p => AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(p)).EndsWith("ChallengePedestal.prefab")));
    }

    [Test]
    public void NoMissingScripts()
    {
        var missing = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(go => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0).Select(go => go.name);
        CollectionAssert.IsEmpty(missing);
    }

    [Test]
    public void SocketPuzzleIsWiredThroughInspectorEvents()
    {
        var socket = GameObject.Find("Environment/Zone_A_Entrance/Challenge_01/Slot/Socket").GetComponent<XRSocketInteractor>();
        var condition = GameObject.Find("Environment/Zone_A_Entrance/Puzzle_01").GetComponent<PuzzleCondition>();
        var reveal = condition.GetComponent<Reveal>();
        var door = GameObject.Find("Environment/Zone_A_Entrance/Chest_01").GetComponent<Door>();
        var piece = GameObject.Find("Environment/Zone_A_Entrance/Piece_Gem").GetComponent<XRGrabInteractable>();
        var decoy = GameObject.Find("Environment/Zone_A_Entrance/Decoy_Ball").GetComponent<XRGrabInteractable>();

        var pieceLayer = InteractionLayerMask.GetMask("PuzzlePiece");
        Assert.AreNotEqual(0, pieceLayer, "PuzzlePiece interaction layer must exist");
        Assert.AreEqual(pieceLayer, socket.interactionLayers.value, "socket accepts only puzzle pieces");
        Assert.AreNotEqual(0, piece.interactionLayers.value & pieceLayer);
        Assert.AreEqual(0, decoy.interactionLayers.value & pieceLayer, "decoy must not fit the socket");
        Assert.IsTrue(piece.throwOnDetach && decoy.throwOnDetach);
        Assert.IsNotNull(piece.GetComponent<Respawn>());
        Assert.IsNotNull(decoy.GetComponent<Respawn>());

        Assert.AreEqual(1, condition.requiredCount);
        AssertCalls(socket.selectEntered, condition, nameof(PuzzleCondition.Signal));
        AssertCalls(socket.selectExited, condition, nameof(PuzzleCondition.Unsignal));
        AssertCalls(condition.OnSolved, door, nameof(Door.Open));
        AssertCalls(condition.OnSolved, reveal, nameof(Reveal.Show));
        Assert.IsTrue(reveal.targets.Length > 0 && reveal.targets.All(t => t != null && !t.activeSelf), "revealed objects start hidden");
        Assert.IsTrue(door.target != null && door.target != door.transform, "door moves the chest lid");
    }

    [Test]
    public void SocketPuzzleSuccessFeedbackIsWiredToOnSolved()
    {
        var condition = GameObject.Find("Environment/Zone_A_Entrance/Puzzle_01").GetComponent<PuzzleCondition>();
        var feedback = GameObject.Find("Environment/Zone_A_Entrance/Challenge_01/SuccessFeedback");
        Assert.IsNotNull(feedback, "success feedback sits at the socket");
        var audio = feedback.GetComponent<AudioSource>();
        var fx = feedback.GetComponentInChildren<ParticleSystem>();

        Assert.IsNotNull(audio.clip);
        Assert.IsFalse(audio.playOnAwake || audio.loop, "sound plays only on solve, once");
        Assert.AreEqual(1f, audio.spatialBlend, "sound is spatial");
        Assert.IsTrue(fx.GetComponentsInChildren<ParticleSystem>().All(p => !p.main.playOnAwake && !p.main.loop), "particles play only on solve, once");
        AssertCalls(condition.OnSolved, audio, nameof(AudioSource.Play));
        AssertCalls(condition.OnSolved, fx, nameof(ParticleSystem.Play));
    }

    [Test]
    public void LocomotionIsTeleportAndSnapTurnOnly()
    {
        // ControllerInputActionManager is a sample type, so read its serialized fields.
        var managers = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .Where(m => m.GetType().Name == "ControllerInputActionManager").ToArray();
        Assert.AreEqual(2, managers.Length);
        foreach (var m in managers)
        {
            var so = new SerializedObject(m);
            Assert.IsFalse(so.FindProperty("m_SmoothMotionEnabled").boolValue, m.name + " must not use continuous move");
            Assert.IsFalse(so.FindProperty("m_SmoothTurnEnabled").boolValue, m.name + " must use snap turn");
        }
    }

    [Test]
    public void FloorIsTeleportTargetNotGrabbable()
    {
        var teleport = InteractionLayerMask.GetMask("Teleport");
        var rays = Object.FindObjectsByType<XRRayInteractor>(FindObjectsSortMode.None).Where(r => r.name == "Teleport Interactor").ToArray();
        var hands = Object.FindObjectsByType<NearFarInteractor>(FindObjectsSortMode.None);
        Assert.AreEqual(2, rays.Length);
        foreach (var area in Object.FindObjectsByType<TeleportationArea>(FindObjectsSortMode.None))
        {
            Assert.AreEqual(teleport, area.interactionLayers.value, area.name + " must be on the Teleport interaction layer only");
            Assert.IsTrue(rays.All(r => (r.interactionLayers & area.interactionLayers) != 0), area.name + " must be reachable by the teleport ray");
            Assert.IsTrue(hands.All(h => (h.interactionLayers & area.interactionLayers) == 0), area.name + " must not be grabbable");
        }
    }

    [Test]
    public void DoorNeedsKeyInLockAndCode()
    {
        var condition = GameObject.Find("Environment/Doorway_AB/Puzzle_Door").GetComponent<PuzzleCondition>();
        var socket = GameObject.Find("Environment/Doorway_AB/KeyLock/Socket").GetComponent<XRSocketInteractor>();
        var codeLock = GameObject.Find("Environment/Doorway_AB/Keypad").GetComponent<CodeLock>();
        var door = GameObject.Find("Environment/Doorway_AB/Door_AB").GetComponent<Door>();
        var key = GameObject.Find("Environment/Zone_A_Entrance").transform.Find("Reward_Key").GetComponent<XRGrabInteractable>();
        var gem = GameObject.Find("Environment/Zone_A_Entrance/Piece_Gem").GetComponent<XRGrabInteractable>();

        Assert.AreEqual(2, condition.requiredCount, "key and code are two different conditions");
        var keyLayer = InteractionLayerMask.GetMask("Key");
        Assert.AreNotEqual(0, keyLayer, "Key interaction layer must exist");
        Assert.AreEqual(keyLayer, socket.interactionLayers.value, "lock accepts only the key");
        Assert.AreNotEqual(0, key.interactionLayers.value & keyLayer);
        Assert.AreEqual(0, gem.interactionLayers.value & keyLayer, "gem must not open the lock");
        Assert.IsNotNull(key.GetComponent<Respawn>());
        Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(codeLock.code, "^[0-9]+$"));

        AssertCalls(socket.selectEntered, condition, nameof(PuzzleCondition.Signal));
        AssertCalls(socket.selectExited, condition, nameof(PuzzleCondition.Unsignal));
        AssertCalls(codeLock.OnSolved, condition, nameof(PuzzleCondition.Signal));
        AssertCalls(condition.OnSolved, door, nameof(Door.Open));
    }

    [Test]
    public void KeypadButtonsEnterEveryDigitOnce()
    {
        var codeLock = GameObject.Find("Environment/Doorway_AB/Keypad").GetComponent<CodeLock>();
        var digits = new System.Collections.Generic.List<int>();
        foreach (var button in codeLock.GetComponentsInChildren<XRSimpleInteractable>())
        {
            var evt = button.selectEntered;
            Assert.AreEqual(1, evt.GetPersistentEventCount(), button.name);
            Assert.AreEqual(codeLock, evt.GetPersistentTarget(0), button.name);
            if (evt.GetPersistentMethodName(0) == nameof(CodeLock.Clear))
                continue;
            Assert.AreEqual(nameof(CodeLock.Press), evt.GetPersistentMethodName(0), button.name);
            var digit = new SerializedObject(button).FindProperty("m_SelectEntered.m_PersistentCalls.m_Calls.Array.data[0].m_Arguments.m_IntArgument").intValue;
            Assert.AreEqual(button.name, "Key_" + digit);
            digits.Add(digit);
            Assert.IsNotNull(button.GetComponent<UnityEngine.XR.Interaction.Toolkit.Filtering.XRPokeFilter>(), button.name + " supports poke");
        }
        CollectionAssert.AreEquivalent(Enumerable.Range(0, 10), digits);
        var display = codeLock.GetComponentInChildren<TMPro.TMP_Text>();
        Assert.AreEqual(1, codeLock.OnInputChanged.GetPersistentEventCount());
        Assert.AreEqual(display, codeLock.OnInputChanged.GetPersistentTarget(0), "display shows the entry");
    }

    [Test]
    public void ZoneBIsBlockedUntilTheDoorOpens()
    {
        var condition = GameObject.Find("Environment/Doorway_AB/Puzzle_Door").GetComponent<PuzzleCondition>();
        var blocker = GameObject.Find("Environment/Zone_B_Gallery/ZoneB_Lock");
        Assert.IsNotNull(blocker, "blocker starts active");
        var box = blocker.GetComponent<BoxCollider>();
        Assert.IsFalse(box.isTrigger, "teleport ray must hit it");
        Assert.IsNull(blocker.GetComponent<TeleportationArea>(), "blocker is not a teleport destination");
        foreach (var area in GameObject.Find("Environment/Zone_B_Gallery").GetComponentsInChildren<TeleportationArea>())
        {
            var top = area.GetComponentInChildren<Collider>().bounds.center + Vector3.up * 0.05f;
            Assert.IsTrue(box.bounds.Contains(new Vector3(top.x, 0.05f, top.z)), area.name + " is covered by the blocker");
        }
        Assert.That(box.bounds.max.y, Is.GreaterThanOrEqualTo(3f), "covers the wall height so arcs over the wall are blocked");
        Assert.That(box.bounds.min.z, Is.GreaterThan(8.14f), "does not cover Zone A or the wall");
        AssertCalls(condition.OnSolved, blocker, "SetActive");
    }

    [Test]
    public void WelcomeVisibleAndHintMenuToggledByController()
    {
        var welcome = GameObject.Find("UI/WelcomePanel");
        Assert.IsNotNull(welcome, "welcome visible at entry");
        var toggle = GameObject.Find("UI").GetComponent<MenuToggle>();
        Assert.IsNotNull(toggle.menu);
        Assert.IsFalse(toggle.menu.activeSelf, "hint menu starts closed");
        var bindings = toggle.toggleAction.action.bindings;
        Assert.AreEqual(2, bindings.Count, "menu button of either controller");
        Assert.IsTrue(bindings.All(b => b.path.Contains("{MenuButton}")));
        foreach (var canvas in new[] { welcome, toggle.menu })
        {
            Assert.AreEqual(RenderMode.WorldSpace, canvas.GetComponent<Canvas>().renderMode);
            Assert.IsNotNull(canvas.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>(), canvas.name + " reacts to XR rays");
        }
        AssertCalls(toggle.menu.transform.Find("CloseButton").GetComponent<UnityEngine.UI.Button>().onClick, toggle, nameof(MenuToggle.Toggle));
        AssertCalls(welcome.transform.Find("StartButton").GetComponent<UnityEngine.UI.Button>().onClick, welcome, "SetActive");
    }

    [Test]
    public void HintsFollowPuzzleProgression()
    {
        var menu = GameObject.Find("UI").GetComponent<MenuToggle>().menu.transform;
        var gem = menu.Find("Hint_Gem").gameObject;
        var door = menu.Find("Hint_Door").gameObject;
        var gallery = menu.Find("Hint_Gallery").gameObject;
        Assert.IsTrue(gem.activeSelf && !door.activeSelf && !gallery.activeSelf, "only the first hint at start");
        var puzzle1 = GameObject.Find("Environment/Zone_A_Entrance/Puzzle_01").GetComponent<PuzzleCondition>();
        var doorPuzzle = GameObject.Find("Environment/Doorway_AB/Puzzle_Door").GetComponent<PuzzleCondition>();
        AssertCalls(puzzle1.OnSolved, gem, "SetActive");
        AssertCalls(puzzle1.OnSolved, door, "SetActive");
        AssertCalls(doorPuzzle.OnSolved, door, "SetActive");
        AssertCalls(doorPuzzle.OnSolved, gallery, "SetActive");
    }

    static void AssertCalls(UnityEventBase evt, Object target, string method)
    {
        var found = Enumerable.Range(0, evt.GetPersistentEventCount())
            .Any(i => evt.GetPersistentTarget(i) == target && evt.GetPersistentMethodName(i) == method);
        Assert.IsTrue(found, $"{method} on {target.name} is not wired");
    }
}
