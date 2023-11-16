using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PortalDefendersAR.GameStates;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.Tests.EditMode
{
    [TestFixture]
    public class GameStateMachineTests : ZenjectUnitTestFixture
    {
        private Mock<IGameState> _mockPlacingObjectState;
        private Mock<IGameState> _mockNewState;
        [Inject] private GameStateMachine _gameStateMachine;

        [SetUp]
        public void SetUp()
        {
            // Mock dependencies
            _mockPlacingObjectState = new Mock<IGameState>();
            _mockPlacingObjectState.Setup(m => m.Tick()).Returns(GameState.StayInState);
            _mockNewState = new Mock<IGameState>();

            // Bind the mock to the container
            Container.Bind<IGameState>()
                     .WithId(GameState.PlacingObject)
                     .FromInstance(_mockPlacingObjectState.Object)
                     .AsCached();

            Container.Bind<IGameState>()
                     .WithId(GameState.Playing)
                     .FromInstance(_mockNewState.Object)
                     .AsCached();

            Container.Bind<GameStateMachine>().AsSingle();

            Container.Inject(this);
        }

        [Test]
        public void Initialize_EntersInitialState()
        {
            _gameStateMachine.Initialize();
            _mockPlacingObjectState.Verify(m => m.Enter(), Times.Once);
        }

        [Test]
        public void Tick_RemainsInStateWhenStateReturnsStayInState()
        {
            // Arrange: Initialize the GameStateMachine to set the initial state.
            _gameStateMachine.Initialize();

            // Act: Call the Tick method on the GameStateMachine.
            _gameStateMachine.Tick();

            // Assert: Verify that the Tick method of the current state is called.
            _mockPlacingObjectState.Verify(m => m.Tick(), Times.Once);

            // Assert: Verify that the Exit and Enter methods of the current state are not called again, 
            // since the state should remain the same.
            _mockPlacingObjectState.Verify(m => m.Exit(), Times.Never);
            _mockPlacingObjectState.Verify(m => m.Enter(), Times.Once); // Once from the Initialize call
        }

        [Test]
        public void Exit_IsCalledWhenStateChanges()
        {
            // Setup the initial state to transition to the new state upon Tick.
            _mockPlacingObjectState.Setup(m => m.Tick()).Returns(GameState.Playing);

            // Initialize and perform the first Tick to set up the state.
            _gameStateMachine.Initialize();
            _gameStateMachine.Tick();

            // Act: Trigger another Tick to cause the state transition.
            _gameStateMachine.Tick();

            // Assert: Verify that Exit is called on the initial state.
            _mockPlacingObjectState.Verify(m => m.Exit(), Times.Once);

            // Assert: Optionally, verify that Enter is called on the new state.
            _mockNewState.Verify(m => m.Enter(), Times.Once);
        }
    }
}
