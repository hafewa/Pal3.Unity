﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Game.Scene.SceneObjects
{
    using System;
    using System.Collections;
    using System.Threading;
    using Common;
    using Core.Contract.Enums;
    using Core.DataReader.Scn;
    using Core.Utilities;
    using Data;
    using Engine.Abstraction;
    using Engine.Extensions;
    using Rendering.Renderer;
    using UnityEngine;
    using Color = Core.Primitives.Color;

    [ScnSceneObject(SceneObjectType.StaticOrAnimated)]
    public sealed class StaticOrAnimatedObject : SceneObject
    {
        public StaticOrAnimatedObject(ScnObjectInfo objectInfo, ScnSceneInfo sceneInfo)
            : base(objectInfo, sceneInfo)
        {
        }

        public override IGameEntity Activate(GameResourceProvider resourceProvider, Color tintColor)
        {
            if (IsActivated) return GetGameEntity();
            IGameEntity sceneObjectGameEntity = base.Activate(resourceProvider, tintColor);

            // Should block the player if Parameters[0] is 0
            if (ObjectInfo.Parameters[0] == 0)
            {
                sceneObjectGameEntity.AddComponent<SceneObjectMeshCollider>();
            }

            sceneObjectGameEntity.AddComponent<StaticOrAnimatedObjectController>().Init(ObjectInfo.Parameters);
            return sceneObjectGameEntity;
        }

        public override bool IsDirectlyInteractable(float distance) => false;

        public override bool ShouldGoToCutsceneWhenInteractionStarted() => true;

        public override IEnumerator InteractAsync(InteractionContext ctx)
        {
            if (IsActivated && ModelType == SceneObjectModelType.CvdModel)
            {
                GetCvdModelRenderer().StartOneTimeAnimation(true);
            }

            yield break;
        }
    }

    internal class StaticOrAnimatedObjectController : TickableGameEntityScript
    {
        private int[] _parameters;
        private Component _effectComponent;
        private float _initYPosition;
        private CancellationTokenSource _animationCts;

        protected override void OnDisableGameEntity()
        {
            if (_animationCts is {IsCancellationRequested: false})
            {
                _animationCts.Cancel();
            }

            _effectComponent.Destroy();
            _effectComponent = null;
        }

        public void Init(int[] parameters)
        {
            _parameters = parameters;

            _initYPosition = Transform.LocalPosition.y;

            // Randomly play animation if Parameters[1] == 0 for Cvd modeled objects.
            if (_parameters[1] == 0)
            {
                if (GameEntity.GetComponent<CvdModelRenderer>() is {} cvdModelRenderer)
                {
                    _animationCts = new CancellationTokenSource();
                    StartCoroutine(PlayAnimationRandomlyAsync(cvdModelRenderer, _animationCts.Token));
                }
            }
        }

        // Play animation with random wait time.
        private IEnumerator PlayAnimationRandomlyAsync(CvdModelRenderer cvdModelRenderer,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                yield return new WaitForSeconds(RandomGenerator.Range(0.5f, 3.5f));
                if (cancellationToken.IsCancellationRequested) yield break;
                yield return cvdModelRenderer.PlayOneTimeAnimationAsync(true);
            }
        }

        protected override void OnLateUpdateGameEntity(float deltaTime)
        {
            switch (_parameters[2])
            {
                // Parameters[2] describes animated object's default animation.
                // 0 means no animation.
                // 1 means the object is animated up and down (sine curve).
                // 2 means the object is animated with constant rotation.
                case 1:
                {
                    Vector3 currentPosition = Transform.LocalPosition;
                    Transform.LocalPosition = new Vector3(currentPosition.x,
                        _initYPosition + MathF.Sin(Time.time) / 6f,
                        currentPosition.z);
                    break;
                }
                case 2:
                {
                    Transform.RotateAround(Transform.Position,
                        Transform.Up,
                        deltaTime * 80f);
                    break;
                }
            }
        }
    }
}