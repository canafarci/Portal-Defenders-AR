using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.CoreStructures;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.Installers
{
    public class PortalInstaller : Installer<PortalInstaller>
    {
        private Pose _pose;

        private PortalInstaller(Pose pose)
        {
            _pose = pose;
        }

        public override void InstallBindings()
        {
            Container.Bind<Portal>().AsSingle();
            Container.Bind<Transform>().FromComponentOnRoot().AsSingle();
            Container.Bind<Pose>().FromInstance(_pose).AsSingle();
        }
    }
}
