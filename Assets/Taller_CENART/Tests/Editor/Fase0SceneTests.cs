using System.Linq;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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
}
