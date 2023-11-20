using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using PortalDefendersAR.ARModules;
using PortalDefendersAR.Creation;
using PortalDefendersAR.GameInput;
using PortalDefendersAR.GameStates;
using PortalDefendersAR.Installers;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PortalDefendersAR.Tests.PlayMode
{
    [TestFixture]
    public class PlacingObjectStateTests : ZenjectIntegrationTestFixture
    {
        private Mock<IPoseRaycaster> _mockPoseRaycaster;
        private Mock<ITouchInputChecker> _mockInputChecker;
        private Mock<IGameState> _mockPlayingState;
        [Inject] private PlacingObjectState _placingObjectState;

        void CommonInstall()
        {
            // Mock dependencies
            _mockPoseRaycaster = new Mock<IPoseRaycaster>();
            _mockInputChecker = new Mock<ITouchInputChecker>();
            _mockPlayingState = new Mock<IGameState>();

            GameObject _portalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Portal.prefab");
            GameObject _fortressPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Fortress.prefab");

            var fac = new Mock<Portal.Factory>();
            fac.Setup(x => x.Create(It.IsAny<Pose>()));

            PreInstall();

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

            PostInstall();
        }


        [UnityTest]
        public IEnumerator Tick_TransitionsStateCorrectly()
        {
            CommonInstall();

            // Wait one frame to allow update logic for SpaceShip to run
            yield return null;

            // Arrange
            _placingObjectState.Enter();

            var samplePose = new Pose();
            _mockPoseRaycaster.Setup(x => x.TryRaycastValidPose(It.IsAny<Vector2>(), out samplePose))
                              .Returns(true).Callback<Vector2, Pose>((pos, p) => p = samplePose);

            var sampleTouch = new Touch();
            _mockInputChecker.Setup(x => x.CheckScreenTouch(out sampleTouch))
                              .Returns(true).Callback<Touch>((t) => t = sampleTouch);

            // Act and Assert for PlacingPortal to PlacingFortress
            _placingObjectState.Tick();
            var currentStateAfterPortal = GetPrivateState(_placingObjectState);
            Assert.AreEqual(PlacingObjectStates.PlacingFortress, currentStateAfterPortal);
            yield return null;

            // Act and Assert for PlacingFortress to Finished
            _placingObjectState.Tick();
            var currentStateAfterFortress = GetPrivateState(_placingObjectState);
            yield return null;
            Assert.AreEqual(PlacingObjectStates.Finished, currentStateAfterFortress);
        }

        private PlacingObjectStates GetPrivateState(PlacingObjectState placingObjectState)
        {
            var currentStateField = typeof(PlacingObjectState)
                .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            return (PlacingObjectStates)currentStateField.GetValue(placingObjectState);
        }

        [UnityTest]
        public IEnumerator TransitionToPlayingState_CallsExit()
        {
            CommonInstall();

            yield return null;
            // Arrange
            _placingObjectState.Enter();
            // Set the state to the final state before Playing
            SetPrivateState(_placingObjectState, PlacingObjectStates.Finished);

            // Act
            var nextState = _placingObjectState.Tick();

            // Assert
            Assert.AreEqual(GameState.Playing, nextState, "State did not transition to Playing as expected.");

        }

        // Helper method to set the private state, similar to GetPrivateState
        private void SetPrivateState(PlacingObjectState placingObjectState, PlacingObjectStates newState)
        {
            var currentStateField = typeof(PlacingObjectState)
                .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            currentStateField.SetValue(placingObjectState, newState);
        }
    }
}
