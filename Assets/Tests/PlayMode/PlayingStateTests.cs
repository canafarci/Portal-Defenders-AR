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
    public class PlayingStateTests : ZenjectIntegrationTestFixture
    {
        private Mock<ITouchInputChecker> _mockTouchInputChecker;
        private Mock<ITouchInputChecker> _mockDraggingInputChecker;
        private Mock<TimeTracker> _mockTimeTracker;
        private Mock<IPoseRaycaster> _mockBombRaycaster;
        private Mock<IPoseRaycaster> _mockDragPlaneRaycaster;
        private Mock<BombThrower> _mockBombThrower;
        [Inject] private PlayingState _playingState;

        public void CommonInstall()
        {
            // Create mocks for all dependencies
            _mockTouchInputChecker = new Mock<ITouchInputChecker>();
            _mockDraggingInputChecker = new Mock<ITouchInputChecker>();
            _mockTimeTracker = new Mock<TimeTracker>();
            _mockBombRaycaster = new Mock<IPoseRaycaster>();
            _mockDragPlaneRaycaster = new Mock<IPoseRaycaster>();
            _mockBombThrower = new Mock<BombThrower>();

            PreInstall();

            // Set up the container
            Container.Bind<ITouchInputChecker>()
                    .WithId(InputCheckers.TouchChecker)
                    .FromInstance(_mockTouchInputChecker.Object)
                    .AsTransient();

            Container.Bind<ITouchInputChecker>()
                     .WithId(InputCheckers.DragChecker)
                     .FromInstance(_mockDraggingInputChecker.Object)
                     .AsTransient();

            Container.Bind<TimeTracker>().FromInstance(_mockTimeTracker.Object).WhenInjectedInto<PlayingState>();

            Container.Bind<IPoseRaycaster>().WithId(Raycasters.BombRaycaster).FromInstance(_mockBombRaycaster.Object).AsTransient();
            Container.Bind<IPoseRaycaster>().WithId(Raycasters.DragPlaneRaycaster).FromInstance(_mockDragPlaneRaycaster.Object).AsTransient();

            Container.Bind<BombThrower>().FromInstance(_mockBombThrower.Object).WhenInjectedInto<PlayingState>();

            GameObject _bombPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bomb.prefab");
            Container.Bind<Transform>()
                    .WithId(ComponentReference.BombParentTransform)
                    .FromInstance(new GameObject("parent").transform)
                    .AsSingle();

            Container.BindFactory<Bomb, Bomb.Factory>()
                    .FromSubContainerResolve()
                    .ByNewPrefabInstaller<BombInstaller>(_bombPrefab);

            // Create PlayingState instance
            Container.Bind<PlayingState>().AsSingle();

            PostInstall();
        }

        [UnityTest]
        public IEnumerator EnterState_InitializesCorrectly()
        {
            CommonInstall();

            // Wait one frame to allow update logic 
            yield return null;

            // Act
            _playingState.Enter();

            // Assert

            var bombField = typeof(PlayingState)
                .GetField("_currentBomb", BindingFlags.NonPublic | BindingFlags.Instance);

            var bomb = (Bomb)bombField.GetValue(_playingState);

            Assert.IsNotNull(bomb);
        }

        [UnityTest]
        public IEnumerator PlayingState_TransitionsToGameOver_OnGameOverCondition()
        {
            CommonInstall();

            // Simulate game over condition
            yield return null; // Wait one frame

            SetTimeTrackerLeftTime(_mockTimeTracker.Object);
            yield return null;
            //Act
            var nextState = _playingState.Tick();

            // Assert that the state transitioned to GameOverState
            Assert.AreEqual(nextState, GameState.GameWon);
        }

        [UnityTest]
        public IEnumerator IdleState_TransitionsTo_DraggingState_OnSuccessfulRaycast()
        {
            CommonInstall();

            // Simulate game over condition
            yield return null; // Wait one frame

            // Arrange
            var samplePose = new Pose();
            _mockBombRaycaster.Setup(x => x.TryRaycastValidPose(It.IsAny<Vector2>(), out samplePose))
                              .Returns(true).Callback<Vector2, Pose>((pos, p) => p = samplePose);

            var sampleTouch = new Touch();
            _mockTouchInputChecker.Setup(x => x.CheckScreenTouch(out sampleTouch))
                              .Returns(true).Callback<Touch>((t) => t = sampleTouch);

            // Simulate state transition to DraggingBomb
            _playingState.Enter();
            _playingState.Tick();

            var playingState = GetPrivateState(_playingState);

            Assert.AreEqual(playingState, PlayingStates.DraggingBomb);
        }

        [UnityTest]
        public IEnumerator DraggingBombState_SetsBombOnBombThrower()
        {
            CommonInstall();

            yield return null; // Wait one frame

            // Arrange
            var samplePose = new Pose();
            _mockDragPlaneRaycaster.Setup(x => x.TryRaycastValidPose(It.IsAny<Vector2>(), out samplePose))
                              .Returns(true).Callback<Vector2, Pose>((pos, p) => p = samplePose);

            var sampleTouch = new Touch();
            _mockDraggingInputChecker.Setup(x => x.CheckScreenTouch(out sampleTouch))
                              .Returns(true).Callback<Touch>((t) => t = sampleTouch);

            SetPrivateState(_playingState, PlayingStates.DraggingBomb);

            yield return null;

            // Simulate state transition to DraggingBomb
            _playingState.Enter();
            _playingState.Tick();

            yield return null;
            //assert
            Bomb stateBomb = GetBombFromCurrentState(_playingState);
            Bomb throwerBomb = GetBombFromThrower(_mockBombThrower.Object);

            Assert.AreEqual(stateBomb, throwerBomb);
        }

        [UnityTest]
        public IEnumerator DraggingBombState_ThrowsBombOnReleaseTouch()
        {
            // Arrange
            CommonInstall();

            GameObject camera = new GameObject("cam", typeof(Camera));
            camera.tag = "MainCamera";
            yield return null; // Wait one frame

            var samplePose = new Pose();
            _mockDragPlaneRaycaster.Setup(x => x.TryRaycastValidPose(It.IsAny<Vector2>(), out samplePose))
                              .Returns(true).Callback<Vector2, Pose>((pos, p) => p = samplePose);

            var sampleTouch = new Touch();
            _mockDraggingInputChecker.Setup(x => x.CheckScreenTouch(out sampleTouch))
                              .Returns(true).Callback<Touch>((t) => t = sampleTouch);

            SetPrivateState(_playingState, PlayingStates.DraggingBomb);

            //act
            yield return null;

            // Simulate state transition to DraggingBomb
            _playingState.Enter();
            _playingState.Tick();
            Bomb firstBomb = GetBombFromCurrentState(_playingState);

            yield return null;

            //simulate release touch
            _mockDraggingInputChecker.Setup(x => x.CheckScreenTouch(out sampleTouch))
                  .Returns(false).Callback<Touch>((t) => t = sampleTouch);

            _playingState.Tick();
            yield return null;
            //assert
            Bomb stateBomb = GetBombFromCurrentState(_playingState);
            Bomb throwerBomb = GetBombFromThrower(_mockBombThrower.Object);

            Assert.IsNull(throwerBomb);
            Assert.AreNotEqual(stateBomb, firstBomb);
        }

        private PlayingStates GetPrivateState(PlayingState playingState)
        {
            var currentStateField = typeof(PlayingState)
                .GetField("_currentPlayingState", BindingFlags.NonPublic | BindingFlags.Instance);
            return (PlayingStates)currentStateField.GetValue(playingState);
        }

        private void SetPrivateState(PlayingState playingState, PlayingStates currentPlayingState)
        {
            var currentStateField = typeof(PlayingState)
                .GetField("_currentPlayingState", BindingFlags.NonPublic | BindingFlags.Instance);

            currentStateField.SetValue(playingState, currentPlayingState);
        }

        private void SetTimeTrackerLeftTime(TimeTracker timeTracker)
        {
            var floatField = typeof(TimeTracker)
                .GetField("_remainingTime", BindingFlags.NonPublic | BindingFlags.Instance);

            floatField.SetValue(timeTracker, -1f);
        }

        private Bomb GetBombFromCurrentState(PlayingState playingState)
        {
            var field = typeof(PlayingState)
                .GetField("_currentBomb", BindingFlags.NonPublic | BindingFlags.Instance);

            return (Bomb)field.GetValue(playingState);
        }

        private Bomb GetBombFromThrower(BombThrower bombThrower)
        {
            var field = typeof(BombThrower)
                .GetField("_currentBomb", BindingFlags.NonPublic | BindingFlags.Instance);

            return (Bomb)field.GetValue(bombThrower);
        }
    }
}
