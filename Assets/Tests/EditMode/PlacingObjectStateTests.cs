using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using PortalDefendersAR.ARModules;
using PortalDefendersAR.CoreStructures;
using PortalDefendersAR.GameInput;
using PortalDefendersAR.GameStates;
using PortalDefendersAR.Installers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace PortalDefendersAR.Tests.EditMode
{
    [TestFixture]
    public class PlacingObjectStateTests : ZenjectUnitTestFixture
    {
        private Mock<IPoseRaycaster> _mockPoseRaycaster;
        private Mock<ITouchInputChecker> _mockInputChecker;
        [Inject] private PlacingObjectState _placingObjectState;

        [SetUp]
        public void SetUp()
        {
            // Mock dependencies
            _mockPoseRaycaster = new Mock<IPoseRaycaster>();
            _mockInputChecker = new Mock<ITouchInputChecker>();

            GameObject _portalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Portal.prefab");
            GameObject _fortressPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Fortress.prefab");

            var fac = new Mock<Portal.Factory>();
            fac.Setup(x => x.Create(It.IsAny<Pose>()));

            // Bind the mock to the container
            Container.BindFactory<Pose, Portal, Portal.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<PortalInstaller>(new GameObject("go"));


            Container.BindFactory<Pose, Fortress, Fortress.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<FortressInstaller>(new GameObject("go"));

            Container.Bind<IPoseRaycaster>().FromInstance(_mockPoseRaycaster.Object).AsSingle();
            Container.Bind<ITouchInputChecker>().FromInstance(_mockInputChecker.Object).AsSingle();

            // Inject dependencies
            Container.Bind<PlacingObjectState>().AsSingle();

            Container.Inject(this);
        }

        [Test]
        public void Tick_StaysInSameStateWhenNoTouchIsDetected()
        {
            // Arrange
            _placingObjectState.Enter();
            var initialState = GetPrivateState(_placingObjectState);

            var sampleTouch = new Touch();
            _mockInputChecker.Setup(x => x.CheckTouchedScreen(out sampleTouch))
                              .Returns(false).Callback<Touch>((t) => t = sampleTouch);

            // Act
            _placingObjectState.Tick();

            //Assert
            var currentStateValue = GetPrivateState(_placingObjectState);
            Assert.AreEqual(initialState, currentStateValue);
        }

        [Test]
        public void Tick_StaysInSSameStateWhenRaycastFails()
        {
            // Arrange
            _placingObjectState.Enter();
            var initialState = GetPrivateState(_placingObjectState);

            var sampleTouch = new Touch();
            _mockInputChecker.Setup(x => x.CheckTouchedScreen(out sampleTouch))
                              .Returns(true).Callback<Touch>((t) => t = sampleTouch);

            var samplePose = new Pose();
            _mockPoseRaycaster.Setup(x => x.TryRaycastValidPose(It.IsAny<Vector2>(), out samplePose))
                              .Returns(false).Callback<Vector2, Pose>((pos, p) => p = samplePose);

            // Act
            _placingObjectState.Tick();

            //Assert
            var currentStateValue = GetPrivateState(_placingObjectState);

            Assert.AreEqual(initialState, currentStateValue);
        }

        [Test]
        public void Tick_ReturnsStaysInStateWhenNoTouchIsDetected()
        {
            // Arrange
            _placingObjectState.Enter();

            var sampleTouch = new Touch();
            _mockInputChecker.Setup(x => x.CheckTouchedScreen(out sampleTouch))
                              .Returns(false).Callback<Touch>((t) => t = sampleTouch);

            // Act
            var state = _placingObjectState.Tick();

            //Assert
            var currentStateValue = GetPrivateState(_placingObjectState);

            Assert.AreEqual(state, GameState.StayInState);
        }

        [Test]
        public void Enter_SetsCurrentStateToPlacingPortal()
        {
            // Act
            _placingObjectState.Enter();

            // Assert: Use reflection to access the private field _currentState
            var currentStateValue = GetPrivateState(_placingObjectState);

            Assert.AreEqual(PlacingObjectStates.PlacingPortal, currentStateValue,
                "Enter method did not set the current state to PlacingPortal as expected.");
        }

        private PlacingObjectStates GetPrivateState(PlacingObjectState placingObjectState)
        {
            var currentStateField = typeof(PlacingObjectState)
                .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            return (PlacingObjectStates)currentStateField.GetValue(placingObjectState);
        }
    }
}
