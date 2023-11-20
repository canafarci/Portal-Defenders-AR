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
            _mockTimeTracker.Setup(t => t.CheckTimeOver(It.IsAny<float>())).Returns(true);

            yield return null; // Wait one frame

            //Act
            var nextState = _playingState.Tick();

            // Assert that the state transitioned to GameOverState
            Assert.IsTrue(nextState == GameState.GameWon);
        }

        private PlayingStates GetPrivateState(PlayingState placingObjectState)
        {
            var currentStateField = typeof(PlayingState)
                .GetField("_currentPlayingState", BindingFlags.NonPublic | BindingFlags.Instance);
            return (PlayingStates)currentStateField.GetValue(placingObjectState);
        }
    }
}
