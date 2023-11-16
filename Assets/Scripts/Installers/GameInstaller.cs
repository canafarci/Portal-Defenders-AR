using PortalDefendersAR.CoreStructures;
using PortalDefendersAR.GameStates;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using Zenject;


namespace PortalDefendersAR.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private GameObject _portalPrefab;
        [SerializeField] private GameObject _fortressPrefab;
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

            Container.BindFactory<Pose, Portal, Portal.Factory>()
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<PortalInstaller>(_portalPrefab);

            Container.BindFactory<Pose, Fortress, Fortress.Factory>()
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<FortressInstaller>(_fortressPrefab);

            Container.Bind<ARPlaneManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<ARRaycastManager>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}
