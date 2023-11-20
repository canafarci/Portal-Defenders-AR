using PortalDefendersAR.Creation;
using PortalDefendersAR.GameStates;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using Zenject;
using PortalDefendersAR.ARModules;
using PortalDefendersAR.GameInput;


namespace PortalDefendersAR.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private GameObject _portalPrefab;
        [SerializeField] private GameObject _fortressPrefab;
        [SerializeField] private GameObject _bombPrefab;
        [SerializeField] private Transform _bombParent;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();

            Container.Bind<IGameState>()
                .WithId(GameState.PlacingObject)
                .To<PlacingObjectState>()
                .AsSingle();

            Container.Bind<IGameState>()
                .WithId(GameState.Playing)
                .To<PlayingState>()
                .AsSingle();

            Container.Bind<Transform>()
                     .WithId(ComponentReference.BombParentTransform)
                     .FromInstance(_bombParent)
                     .AsSingle();

            Container.BindFactory<Pose, Portal, Portal.Factory>()
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<PortalInstaller>(_portalPrefab);

            Container.BindFactory<Pose, Fortress, Fortress.Factory>()
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<FortressInstaller>(_fortressPrefab);

            Container.BindFactory<Bomb, Bomb.Factory>()
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<BombInstaller>(_bombPrefab);

            Container.BindInterfacesAndSelfTo<BombThrower>().AsSingle();
            Container.Bind<TimeTracker>().AsSingle();

            BindInputs();

            BindRaycasters();
        }

        private void BindRaycasters()
        {

            Container.Bind<ARPlaneManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<ARRaycastManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<IPoseRaycaster>()
                .To<WorldPoseRaycaster>()
                .WhenInjectedInto<PlacingObjectState>();

            Container.Bind<IPoseRaycaster>()
                .WithId(Raycasters.DragPlaneRaycaster)
                .To<DragPlaneRaycaster>()
                .AsSingle();

            Container.Bind<IPoseRaycaster>()
                .WithId(Raycasters.BombRaycaster)
                .To<BombRaycaster>()
                .AsSingle();
        }

        private void BindInputs()
        {
            Container.Bind<ITouchInputChecker>()
                     .To<TouchInputChecker>()
                     .WhenInjectedInto<PlacingObjectState>();

            Container.Bind<ITouchInputChecker>()
                    .WithId(InputCheckers.TouchChecker)
                     .To<TouchInputChecker>()
                     .AsCached();

            Container.Bind<ITouchInputChecker>()
                    .WithId(InputCheckers.DragChecker)
                     .To<DragInputChecker>()
                     .AsSingle();
        }
    }
}
