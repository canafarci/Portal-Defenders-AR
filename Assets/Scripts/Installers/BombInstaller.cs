using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.Creation;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.Installers
{
    public class BombInstaller : Installer<BombInstaller>
    {
        // private Vector3 _position;

        // private BombInstaller(Vector3 position)
        // {
        //     _position = position;
        // }

        public override void InstallBindings()
        {
            Container.Bind<Bomb>().AsSingle();

            Container.Bind<Transform>()
                     .WithId(ComponentReference.ObjectTransform)
                     .FromComponentOnRoot()
                     .AsSingle();

            Container.Bind<Rigidbody>()
                     .FromComponentOnRoot()
                     .AsSingle();
        }
    }
}
