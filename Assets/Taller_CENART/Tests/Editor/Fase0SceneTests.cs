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

    static void AssertCalls(UnityEventBase evt, Object target, string method)
    {
        var found = Enumerable.Range(0, evt.GetPersistentEventCount())
            .Any(i => evt.GetPersistentTarget(i) == target && evt.GetPersistentMethodName(i) == method);
        Assert.IsTrue(found, $"{method} on {target.name} is not wired");
    }
}
