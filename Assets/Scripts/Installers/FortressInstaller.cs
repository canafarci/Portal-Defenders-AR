using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.Creation;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.Installers
{
    public class FortressInstaller : Installer<FortressInstaller>
    {
        private Pose _pose;

        private FortressInstaller(Pose pose)
        {
            _pose = pose;
        }

        public override void InstallBindings()
        {
            Container.Bind<Fortress>().AsSingle();
            Container.Bind<Transform>().FromComponentOnRoot().AsSingle();
            Container.Bind<Pose>().FromInstance(_pose).AsSingle();
        }
    }
}
