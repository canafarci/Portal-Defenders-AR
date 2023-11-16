using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using PortalDefendersAR.CoreStructures;
using PortalDefendersAR.GameStates;
using PortalDefendersAR.Installers;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace PortalDefendersAR.Tests.EditMode
{
    [TestFixture]
    public class PlacingObjectStateTests : ZenjectUnitTestFixture
    {
        [Inject] private PlacingObjectState _placingObjectState;

        [SetUp]
        public void SetUp()
        {
            var ar_objects = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AR_System_Objects.prefab");

            GameObject go = new GameObject("asd");

            Container.Bind<ARPlaneManager>()
                .FromComponentOn(ar_objects)
                .AsSingle();

            Container.Bind<ARRaycastManager>()
                .FromComponentOn(ar_objects)
                .AsSingle();

            Container.BindFactory<Pose, Portal, Portal.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<PortalInstaller>(go);

            Container.BindFactory<Pose, Fortress, Fortress.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<FortressInstaller>(go);

            Container.Bind<PlacingObjectState>().AsSingle();
        }

        [Test]
        public void Enter_SetsCurrentStateToPlacingPortal()
        {
            // Act
            _placingObjectState.Enter();

            // Assert
            var currentStateField = typeof(PlacingObjectState)
                .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            var currentStateValue = currentStateField.GetValue(_placingObjectState);

            Assert.AreEqual(currentStateValue, PlacingObjectStates.PlacingPortal,
                "Enter method did not set the current state to PlacingPortal as expected.");
        }

        // Other tests for object placement, state transitions, raycast handling, etc.
    }
}
