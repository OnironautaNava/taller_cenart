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

    static GameObject Find(string path) => GameObject.Find("Environment/" + path);

    [Test]
    public void LadderClimbsToTheWestPlatformWithBalls()
    {
        var ladder = Find("Zone_B_Gallery/Ladder");
        Assert.IsNotNull(ladder.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbInteractable>(), "ladder is climbable");
        Assert.IsNull(ladder.GetComponent<TallerCenart.PuzzleCondition>(), "ladder is static, not an activator");
        var top = Find("Zone_B_Gallery/Platform_West/Top").GetComponent<TeleportationArea>();
        var topY = top.GetComponentInChildren<Collider>().bounds.max.y;
        Assert.That(topY, Is.GreaterThanOrEqualTo(1.9f), "platform is high");
        Assert.That(ladder.GetComponentsInChildren<Collider>().Where(c => c.enabled).Max(c => c.bounds.max.y), Is.GreaterThan(topY + 0.5f), "rungs reach above the platform");
        foreach (var body in ladder.GetComponentsInChildren<Rigidbody>())
            Assert.IsTrue(body.isKinematic && !body.useGravity, body.name + " must not be pushed by physics");
        Assert.IsFalse(ladder.GetComponent<Collider>().enabled, "only rungs are grabbable, not the whole ladder");
        var climbs = ladder.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbInteractable>();
        var rungs = climbs.SelectMany(c => c.colliders).ToArray();
        Assert.That(rungs.Length, Is.GreaterThanOrEqualTo(8), "every rung is a hand hold");
        Assert.AreEqual(rungs.Length, rungs.Distinct().Count(), "a rung belongs to one climb interactable");
        Assert.IsTrue(rungs.All(r => r.enabled && r.bounds.size.y < 0.15f), "hand holds are single rungs");
        var climbTop = climbs.Single(c => c.climbAssistanceTeleportVolume != null);
        Assert.IsTrue(climbTop.colliders.All(r => r.bounds.center.y > topY + 0.1f), "only rungs above the platform hand off to it");
        Assert.IsTrue(climbs.Where(c => c != climbTop).SelectMany(c => c.colliders).All(r => r.bounds.center.y < topY + 0.1f), "releasing lower rungs drops the player");
        var anchors = climbTop.climbAssistanceTeleportVolume.anchorTransforms;
        Assert.IsNotEmpty(anchors);
        var platform = Find("Zone_B_Gallery/Platform_West/Top").GetComponentInChildren<Collider>().bounds;
        foreach (var anchor in anchors)
            Assert.IsTrue(platform.Contains(anchor.position + Vector3.down * 0.05f), "climbing off the top lands on the platform");

        var balls = Find("Zone_B_Gallery").GetComponentsInChildren<Respawn>(true).Where(r => r.name.StartsWith("Ball")).ToArray();
        Assert.That(balls.Length, Is.GreaterThanOrEqualTo(1));
        var doorPuzzle = Find("Doorway_AB/Puzzle_Door").GetComponent<PuzzleCondition>();
        foreach (var ball in balls)
        {
            Assert.IsFalse(ball.gameObject.activeSelf, ball.name + " cannot start inside ZoneB_Lock");
            AssertCalls(doorPuzzle.OnSolved, ball.gameObject, "SetActive");
            Assert.IsNotNull(ball.GetComponent<SphereCollider>(), ball.name + " collides");
            Assert.IsTrue(ball.GetComponent<XRGrabInteractable>().throwOnDetach, ball.name);
            Assert.That(ball.transform.position.y, Is.GreaterThan(topY), ball.name + " waits on the platform");
            Assert.That(ball.minHeight, Is.InRange(0.2f, topY - 0.2f), ball.name + " returns to the platform after a miss lands on the floor");
        }
    }

    [Test]
    public void HighSurfacesAreTeleportTargetsOnlyFromAbove()
    {
        var origin = Object.FindAnyObjectByType<XROrigin>().transform;
        foreach (var path in new[] { "Zone_B_Gallery/Platform_West/Top", "Zone_B_Gallery/Platform_East/Top", "Zone_B_Gallery/Bridge" })
        {
            var area = Find(path).GetComponent<TeleportationArea>();
            var hover = area.startingHoverFilters.OfType<PlayerHeightFilter>().SingleOrDefault();
            Assert.IsNotNull(hover, path + " filters hover by player height");
            Assert.Contains(hover, area.startingSelectFilters, path + " filters select by player height");
            Assert.AreEqual(origin, hover.player, path);
            Assert.That(hover.minHeight, Is.InRange(0.5f, 1.9f), path + ": above the floor, reachable when standing on the platform");
        }
        foreach (var floor in Find("Zone_B_Gallery").GetComponentsInChildren<TeleportationArea>().Where(a => a.name.StartsWith("Floor_")))
            Assert.IsEmpty(floor.startingHoverFilters, floor.name + " stays reachable from anywhere");
    }

    [Test]
    public void TargetHitDropsTheBridgeAndUnlocksTheLever()
    {
        var target = Find("Zone_B_Gallery/Bridge").GetComponentInChildren<TargetHit>();
        var puzzle = Find("Zone_B_Gallery/Puzzle_Target").GetComponent<PuzzleCondition>();
        var bridge = Find("Zone_B_Gallery/Bridge");
        var bridgeArea = bridge.GetComponent<TeleportationArea>();
        var eastLock = Find("Zone_B_Gallery/EastLock");
        var lever = Find("Zone_B_Gallery/Challenge_03").GetComponentInChildren<XRSimpleInteractable>(true);

        Assert.IsNotNull(target.GetComponent<Collider>(), "target receives collisions");
        Assert.AreEqual(1, puzzle.requiredCount);
        AssertCalls(target.OnHit, puzzle, nameof(PuzzleCondition.Signal));
        AssertCalls(puzzle.OnSolved, bridge.GetComponent<Door>(), nameof(Door.Open));
        Assert.IsFalse(bridgeArea.enabled, "upright bridge is not a teleport destination");
        AssertCalls(puzzle.OnSolved, bridgeArea, "set_enabled");
        Assert.IsFalse(lever.enabled, "lever cannot be pulled before the bridge falls");
        AssertCalls(puzzle.OnSolved, lever, "set_enabled");
        AssertCalls(puzzle.OnSolved, eastLock, "SetActive");

        var box = eastLock.GetComponent<BoxCollider>();
        Assert.IsFalse(box.isTrigger);
        var eastTop = Find("Zone_B_Gallery/Platform_East/Top").GetComponentInChildren<Collider>().bounds;
        Assert.IsTrue(box.bounds.Contains(eastTop.center + Vector3.up * 0.1f), "east platform blocked before the bridge");
        Assert.IsTrue(box.bounds.Contains(lever.transform.position + Vector3.up * 0.1f), "lever enclosed before the bridge");
        Assert.That(box.bounds.min.x, Is.GreaterThan(target.GetComponent<Collider>().bounds.max.x), "blocker does not shield the target");
        Assert.That(target.GetComponent<Collider>().bounds.max.y, Is.LessThan(4f), "target stays under ZoneB_Lock so it cannot be hit from Zone A");
    }

    [Test]
    public void LeverOpensTheFinalDoorToTheExit()
    {
        var lever = Find("Zone_B_Gallery/Challenge_03").GetComponentInChildren<XRSimpleInteractable>(true);
        var puzzle = Find("Zone_B_Gallery/Puzzle_Lever").GetComponent<PuzzleCondition>();
        var door = Find("Exit/FinalDoor").GetComponent<Door>();
        var exitLock = Find("Exit/ExitLock");
        var panel = GameObject.Find("UI").transform.Find("ExitPanel").gameObject;

        AssertCalls(lever.selectEntered, puzzle, nameof(PuzzleCondition.Signal));
        AssertCalls(puzzle.OnSolved, door, nameof(Door.Open));
        AssertCalls(puzzle.OnSolved, exitLock, "SetActive");
        Assert.IsFalse(panel.activeSelf, "completion message hidden until the lever");
        AssertCalls(puzzle.OnSolved, panel, "SetActive");
        var box = exitLock.GetComponent<BoxCollider>();
        Assert.IsFalse(box.isTrigger);
        Assert.That(box.bounds.max.y, Is.GreaterThanOrEqualTo(3f));
        foreach (var area in Find("Exit").GetComponentsInChildren<TeleportationArea>())
        {
            var c = area.GetComponentInChildren<Collider>().bounds.center;
            Assert.IsTrue(box.bounds.Contains(new Vector3(c.x, 0.05f, c.z)), area.name + " blocked until the lever");
        }
    }

    [TestCase("Puzzle_Target", "TargetFeedback")]
    [TestCase("Puzzle_Lever", "LeverFeedback")]
    public void ThirdChallengeFeedbackIsSpatialAndWired(string puzzleName, string feedbackName)
    {
        var puzzle = Find("Zone_B_Gallery/" + puzzleName).GetComponent<PuzzleCondition>();
        var sources = Find("Zone_B_Gallery/" + feedbackName).GetComponentsInChildren<AudioSource>();
        Assert.IsNotEmpty(sources);
        foreach (var audio in sources)
        {
            Assert.IsNotNull(audio.clip, audio.name);
            Assert.IsFalse(audio.playOnAwake || audio.loop, audio.name);
            Assert.AreEqual(1f, audio.spatialBlend, audio.name + " is spatial");
            AssertCalls(puzzle.OnSolved, audio, nameof(AudioSource.Play));
        }
        foreach (var fx in Find("Zone_B_Gallery/" + feedbackName).GetComponentsInChildren<ParticleSystem>())
            Assert.IsFalse(fx.main.playOnAwake || fx.main.loop, fx.name);
    }

    [Test]
    public void HintsCoverTheThirdChallengeAndExit()
    {
        var menu = GameObject.Find("UI").GetComponent<MenuToggle>().menu.transform;
        var gallery = menu.Find("Hint_Gallery").gameObject;
        var lever = menu.Find("Hint_Lever").gameObject;
        var exit = menu.Find("Hint_Exit").gameObject;
        Assert.IsFalse(lever.activeSelf || exit.activeSelf);
        StringAssert.DoesNotContain("Pendiente", gallery.GetComponent<TMPro.TMP_Text>().text);
        var target = Find("Zone_B_Gallery/Puzzle_Target").GetComponent<PuzzleCondition>();
        var leverPuzzle = Find("Zone_B_Gallery/Puzzle_Lever").GetComponent<PuzzleCondition>();
        AssertCalls(target.OnSolved, gallery, "SetActive");
        AssertCalls(target.OnSolved, lever, "SetActive");
        AssertCalls(leverPuzzle.OnSolved, lever, "SetActive");
        AssertCalls(leverPuzzle.OnSolved, exit, "SetActive");
    }

    [Test]
    public void RoomDoesNotRequireTheTimer()
    {
        Assert.IsEmpty(Object.FindObjectsByType<Timer>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        var example = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Taller_CENART/Prefabs/TimerExample.prefab");
        Assert.IsNotNull(example, "reusable Timer example prefab");
        var timer = example.GetComponent<Timer>();
        Assert.AreEqual(1, timer.OnTextChanged.GetPersistentEventCount());
        Assert.IsInstanceOf<TMPro.TMP_Text>(timer.OnTextChanged.GetPersistentTarget(0));
        Assert.That(timer.OnTimeUp.GetPersistentEventCount(), Is.GreaterThanOrEqualTo(1));
    }

    static void AssertCalls(UnityEventBase evt, Object target, string method)
    {
        var found = Enumerable.Range(0, evt.GetPersistentEventCount())
            .Any(i => evt.GetPersistentTarget(i) == target && evt.GetPersistentMethodName(i) == method);
        Assert.IsTrue(found, $"{method} on {target.name} is not wired");
    }
}
