﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2022, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Scene.SceneObjects
{
    using System;
    using System.Collections;
    using System.Linq;
    using Command;
    using Command.InternalCommands;
    using Command.SceCommands;
    using Common;
    using Core.DataLoader;
    using Core.DataReader.Cpk;
    using Core.DataReader.Cvd;
    using Core.DataReader.Scn;
    using Core.Services;
    using Data;
    using MetaData;
    using Renderer;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [ScnSceneObject(ScnSceneObjectType.Switch)]
    public sealed class SwitchObject : SceneObject
    {
        private const float MAX_INTERACTION_DISTANCE = 5f;

        private SceneObjectMeshCollider _meshCollider;
        private readonly string _switchInteractionIndicatorModelPath = FileConstants.ObjectFolderVirtualPath +
                                                                       CpkConstants.DirectorySeparator + "g03.cvd";
        private CvdModelRenderer _switchInteractionIndicatorModelRenderer;
        private GameObject _switchInteractionIndicatorGameObject;
        private readonly Scene _currentScene;

        public SwitchObject(ScnObjectInfo objectInfo, ScnSceneInfo sceneInfo)
            : base(objectInfo, sceneInfo)
        {
            _currentScene = ServiceLocator.Instance.Get<SceneManager>().GetCurrentScene();
        }

        public override GameObject Activate(GameResourceProvider resourceProvider, Color tintColor)
        {
            if (Activated) return GetGameObject();
            GameObject sceneGameObject = base.Activate(resourceProvider, tintColor);

            if (ObjectInfo.IsNonBlocking == 0)
            {
                if (!(ObjectInfo.SwitchState == 1 && ObjectInfo.Parameters[0] == 1) &&
                    !IsDivineTreeMasterFlower())
                {
                    // Add collider to block player
                    _meshCollider = sceneGameObject.AddComponent<SceneObjectMeshCollider>();
                }
            }

            // Add interaction indicator when switch times is greater than 0
            // and Parameter[1] is 0 (1 means the switch is not directly interactable)
            if (ObjectInfo.Times > 0 && ObjectInfo.Parameters[1] == 0)
            {
                Vector3 switchPosition = sceneGameObject.transform.position;
                _switchInteractionIndicatorGameObject = new GameObject("Switch_Interaction_Indicator");
                _switchInteractionIndicatorGameObject.transform.SetParent(sceneGameObject.transform, false);
                _switchInteractionIndicatorGameObject.transform.position =
                    new Vector3(switchPosition.x, GetRendererBounds().max.y + 1f, switchPosition.z);
                (CvdFile cvdFile, ITextureResourceProvider textureProvider) =
                    resourceProvider.GetCvd(_switchInteractionIndicatorModelPath);
                _switchInteractionIndicatorModelRenderer = _switchInteractionIndicatorGameObject.AddComponent<CvdModelRenderer>();
                _switchInteractionIndicatorModelRenderer.Init(cvdFile,
                    resourceProvider.GetMaterialFactory(),
                    textureProvider,
                    tintColor);
                _switchInteractionIndicatorModelRenderer.LoopAnimation();
            }

            return sceneGameObject;
        }

        public override bool IsDirectlyInteractable(float distance)
        {
            return Activated && distance < MAX_INTERACTION_DISTANCE && ObjectInfo.Times > 0;
        }

        public override IEnumerator Interact(InteractionContext ctx)
        {
            if (!IsInteractableBasedOnTimesCount()) yield break;

            var shouldResetCamera = false;
            if (ctx.InitObjectId != ObjectInfo.Id && !IsFullyVisibleToCamera())
            {
                shouldResetCamera = true;
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new CameraFocusOnSceneObjectCommand(ObjectInfo.Id));
            }

            var currentSwitchState = ObjectInfo.SwitchState;

            ToggleAndSaveSwitchState();

            if (ObjectInfo.Times == 0 && _switchInteractionIndicatorGameObject != null)
            {
                _switchInteractionIndicatorModelRenderer.Dispose();
                Object.Destroy(_switchInteractionIndicatorModelRenderer);
                Object.Destroy(_switchInteractionIndicatorGameObject);
            }

            if (ctx.InitObjectId == ObjectInfo.Id)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorStopActionAndStandCommand(ActorConstants.PlayerActorVirtualID));
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new PlayerActorLookAtSceneObjectCommand(ObjectInfo.Id));
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorPerformActionCommand(ActorConstants.PlayerActorVirtualID,
                        ActorConstants.ActionNames[ActorActionType.Check], 1));
            }

            PlaySfxIfAny();

            if (ModelType == SceneObjectModelType.CvdModel)
            {
                yield return GetCvdModelRenderer().PlayOneTimeAnimation(true,
                    currentSwitchState == 0 ? 1f : -1f);

                // Remove collider to allow player to pass through
                if (ObjectInfo.Parameters[0] == 1 && _meshCollider != null)
                {
                    Object.Destroy(_meshCollider);
                }
            }

            ExecuteScriptIfAny();

            yield return ActivateOrInteractWithLinkedObjectIfAny(ctx);

            // Special handling for master flower switch located in
            // the scene m16 4
            if (IsDivineTreeMasterFlower())
            {
                // Save master flower switch state
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new SceneSaveGlobalObjectSwitchStateCommand(SceneInfo.CityName,
                        SceneInfo.SceneName,
                        ObjectInfo.Id,
                        ObjectInfo.SwitchState));

                var allActivatedSceneObjects = _currentScene.GetAllActivatedSceneObjects();
                var allFlowerObjects = _currentScene.GetAllSceneObjects().Where(
                    _ => allActivatedSceneObjects.Contains(_.Key) &&
                         _.Value.ObjectInfo.Type == ScnSceneObjectType.DivineTreeFlower);
                foreach (var flowerObject in allFlowerObjects)
                {
                    // Re-activate all flowers in current scene to refresh their state
                    _currentScene.DeactivateSceneObject(flowerObject.Key);
                    _currentScene.ActivateSceneObject(flowerObject.Key);
                }
            }

            if (shouldResetCamera)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(new CameraFreeCommand(1));
            }
        }

        private bool IsDivineTreeMasterFlower()
        {
            return ObjectInfo.Parameters[2] == 1 &&
                   ObjectInfo.Id == 22 &&
                   SceneInfo.CityName.Equals("m16", StringComparison.OrdinalIgnoreCase) &&
                   SceneInfo.SceneName.Equals("4", StringComparison.OrdinalIgnoreCase);
        }

        public override void Deactivate()
        {
            if (_meshCollider != null)
            {
                Object.Destroy(_meshCollider);
            }

            if (_switchInteractionIndicatorModelRenderer != null)
            {
                _switchInteractionIndicatorModelRenderer.Dispose();
                Object.Destroy(_switchInteractionIndicatorModelRenderer);
            }

            if (_switchInteractionIndicatorGameObject != null)
            {
                Object.Destroy(_switchInteractionIndicatorGameObject);
            }

            base.Deactivate();
        }
    }
}