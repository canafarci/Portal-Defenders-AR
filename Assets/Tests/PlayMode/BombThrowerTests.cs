using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PortalDefendersAR.Creation;
using PortalDefendersAR.GameStates;
using PortalDefendersAR.Installers;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PortalDefendersAR.Tests.PlayMode
{
    [TestFixture]
    public class BombThrowerTests : ZenjectIntegrationTestFixture
    {
        [Inject] private BombThrower _bombThrower;
        [Inject] private Bomb.Factory _bombFactory;
        void CommonInstall()
        {
            GameObject camera = new GameObject("cam", typeof(Camera));
            camera.tag = "MainCamera";

            PreInstall();

            Container.BindInterfacesAndSelfTo<BombThrower>().AsSingle();

            GameObject _bombPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bomb.prefab");

            Container.Bind<Transform>()
                    .WithId(ComponentReference.BombParentTransform)
                    .FromInstance(new GameObject("parent").transform)
                    .AsSingle();

            Container.BindFactory<Bomb, Bomb.Factory>()
                    .FromSubContainerResolve()
                    .ByNewPrefabInstaller<BombInstaller>(_bombPrefab);

            PostInstall();
        }


        [UnityTest]
        public IEnumerator SetBomb_SetsBombCorrectly()
        {

            CommonInstall();

            // Wait one frame to allow update logic 
            yield return null;

            var bomb = _bombFactory.Create();

            _bombThrower.SetBomb(bomb);

            Assert.IsTrue(_bombThrower.HasBomb());
        }

        [UnityTest]
        public IEnumerator ClearBomb_ClearsBombCorrectly()
        {
            CommonInstall();

            yield return null; // Wait one frame for initialization

            var bomb = _bombFactory.Create();
            _bombThrower.SetBomb(bomb);
            _bombThrower.ClearBomb();

            Assert.IsFalse(_bombThrower.HasBomb());
        }

        [UnityTest]
        public IEnumerator DragBomb_SetsTargetPositionCorrectly()
        {
            CommonInstall();

            yield return null;

            var bomb = _bombFactory.Create();
            _bombThrower.SetBomb(bomb);

            var pose = new Pose(Vector3.one, Quaternion.identity);
            _bombThrower.DragBomb(pose);

            // Verify the bomb's position is updated after some time (requires a few frames)
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(Vector3.one, bomb.Transform.position);
        }

        [UnityTest]
        public IEnumerator FixedTick_UpdatesBombPosition()
        {
            CommonInstall();

            yield return null;

            var bomb = _bombFactory.Create();
            _bombThrower.SetBomb(bomb);

            var initialPosition = bomb.Transform.position;
            var targetPosition = Vector3.one;
            var pose = new Pose(targetPosition, Quaternion.identity);
            _bombThrower.DragBomb(pose);

            // Simulate a few frames for fixed update
            yield return new WaitForSeconds(0.5f);

            Assert.AreNotEqual(initialPosition, bomb.Transform.position);
        }

        [UnityTest]
        public IEnumerator Throw_AppliesPhysicsToBomb()
        {
            CommonInstall();

            yield return null;

            var bomb = _bombFactory.Create();
            _bombThrower.SetBomb(bomb);

            _bombThrower.Throw();

            yield return new WaitForFixedUpdate(); // Wait for physics update

            Assert.IsFalse(bomb.Rigidbody.isKinematic);
            Assert.IsTrue(bomb.Rigidbody.useGravity);
            Assert.IsNull(bomb.Transform.parent);
            Assert.IsFalse(bomb.Rigidbody.velocity == Vector3.zero);
        }
    }
}
