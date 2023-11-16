using PortalDefendersAR.GameStates;
using Zenject;


namespace PortalDefendersAR.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();

            Container.Bind<IGameState>()
                .WithId(GameState.PlacingObject)
                .To<PlacingObjectState>()
                .AsSingle();
        }
    }
}
