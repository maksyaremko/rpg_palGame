// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2022, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Player
{
    using Command;
    using Command.InternalCommands;
    using Command.SceCommands;
    using MetaData;

    public class PlayerManager :
        ICommandExecutor<DialogueRenderActorAvatarCommand>,
        ICommandExecutor<ActorActivateCommand>,
        ICommandExecutor<ActorAddSkillCommand>,
        ICommandExecutor<ActorAutoStandCommand>,
        ICommandExecutor<ActorPerformActionCommand>,
        ICommandExecutor<ActorChangeScaleCommand>,
        ICommandExecutor<ActorChangeTextureCommand>,
        ICommandExecutor<ActorSetTilePositionCommand>,
        ICommandExecutor<ActorChangeColliderSettingCommand>,
        ICommandExecutor<ActorSetFacingDirectionCommand>,
        ICommandExecutor<ActorRotateFacingCommand>,
        ICommandExecutor<ActorRotateFacingDirectionCommand>,
        ICommandExecutor<ActorPathToCommand>,
        #if PAL3A
        ICommandExecutor<ActorWalkToUsingActionCommand>,
        #endif
        ICommandExecutor<ActorMoveToCommand>,
        ICommandExecutor<ActorMoveBackwardsCommand>,
        ICommandExecutor<ActorMoveOutOfScreenCommand>,
        ICommandExecutor<ActorLookAtActorCommand>,
        ICommandExecutor<ActorShowEmojiCommand>,
        #if PAL3A
        ICommandExecutor<ActorShowEmoji2Command>,
        #endif
        ICommandExecutor<ActorStopActionCommand>,
        ICommandExecutor<ActorStopActionAndStandCommand>,
        ICommandExecutor<ActorEnablePlayerControlCommand>,
        ICommandExecutor<ActorSetNavLayerCommand>,
        ICommandExecutor<ActorFadeInCommand>,
        ICommandExecutor<ActorFadeOutCommand>,
        #if PAL3A
        ICommandExecutor<ActorSetYPositionCommand>,
        #endif
        ICommandExecutor<PlayerEnableInputCommand>,
        ICommandExecutor<CameraFocusOnActorCommand>,
        ICommandExecutor<ScenePostLoadingNotification>,
        ICommandExecutor<LongKuiSwitchModeCommand>
    {
        private PlayerActorId _playerActor = 0;
        private bool _playerActorControlEnabled;
        private bool _playerInputEnabled;
        private int _longKuiLastKnownMode = 0;

        public PlayerManager()
        {
            CommandExecutorRegistry<ICommand>.Instance.Register(this);
        }

        public void Dispose()
        {
            CommandExecutorRegistry<ICommand>.Instance.UnRegister(this);
        }

        public PlayerActorId GetPlayerActor()
        {
            return _playerActor;
        }

        public bool IsPlayerActorControlEnabled()
        {
            return _playerActorControlEnabled;
        }

        public bool IsPlayerInputEnabled()
        {
            return _playerInputEnabled;
        }

        public void Execute(PlayerEnableInputCommand command)
        {
            _playerInputEnabled = (command.Enable == 1);
        }

        public void Execute(DialogueRenderActorAvatarCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new DialogueRenderActorAvatarCommand((int)_playerActor, command.AvatarTextureName, command.RightAligned));
            }
        }

        public void Execute(ActorActivateCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorActivateCommand((int)_playerActor, command.IsActive));
            }
        }

        public void Execute(ActorAddSkillCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorAddSkillCommand((int)_playerActor, command.SkillId));
            }
        }
        
        public void Execute(ActorAutoStandCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorAutoStandCommand((int)_playerActor, command.AutoStand));
            }
        }

        public void Execute(ActorPerformActionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorPerformActionCommand((int)_playerActor, command.ActionName, command.LoopCount));
            }
        }

        public void Execute(ActorChangeScaleCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorChangeScaleCommand((int)_playerActor, command.Scale));
            }
        }
        
        public void Execute(ActorSetTilePositionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorSetTilePositionCommand((int)_playerActor, command.TileXPosition, command.TileZPosition));
                // TODO: fix PAL3A actor activation issue
                #if PAL3A
                CommandDispatcher<ICommand>.Instance.Dispatch(new ActorActivateCommand((int)_playerActor, 1));
                #endif
            }
        }

        public void Execute(ActorChangeColliderSettingCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorChangeColliderSettingCommand((int)_playerActor, command.DisableCollider));
            }
        }

        public void Execute(ActorSetFacingDirectionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorSetFacingDirectionCommand((int)_playerActor, command.Direction));
            }
        }

        public void Execute(ActorRotateFacingCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorRotateFacingCommand((int)_playerActor, command.Degrees));
            }
        }

        public void Execute(ActorRotateFacingDirectionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorRotateFacingDirectionCommand((int)_playerActor, command.Direction));
            }
        }

        public void Execute(ActorPathToCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorPathToCommand((int)_playerActor, command.TileX, command.TileZ, command.Mode));
            }
        }
        
        #if PAL3A
        public void Execute(ActorWalkToUsingActionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorWalkToUsingActionCommand((int)_playerActor, command.TileX, command.TileZ, command.Action));
            }
        }
        #endif
        
        public void Execute(ActorMoveToCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorMoveToCommand((int)_playerActor, command.TileX, command.TileZ, command.Mode));
            }
        }
        
        public void Execute(ActorMoveOutOfScreenCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorMoveOutOfScreenCommand((int)_playerActor, command.TileX, command.TileZ, command.Mode));
            }
        }

        public void Execute(ActorMoveBackwardsCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorMoveBackwardsCommand((int)_playerActor, command.Distance));
            }
        }

        public void Execute(ActorLookAtActorCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorLookAtActorCommand((int)_playerActor, command.LookAtActorId));
            }
            else if (command.LookAtActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorLookAtActorCommand(command.ActorId, (int)_playerActor));
            }
        }

        public void Execute(ActorShowEmojiCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorShowEmojiCommand((int)_playerActor, command.EmojiId));
            }
        }
        
        #if PAL3A
        public void Execute(ActorShowEmoji2Command command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorShowEmoji2Command((int)_playerActor, command.EmojiId));
            }
        }
        #endif

        public void Execute(ActorFadeInCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorFadeInCommand((int)_playerActor));
            }
        }

        public void Execute(ActorFadeOutCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorFadeOutCommand((int)_playerActor));
            }
        }

        public void Execute(ActorChangeTextureCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorChangeTextureCommand((int)_playerActor, command.TextureName));
            }
        }

        public void Execute(ActorStopActionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorStopActionCommand((int)_playerActor));
            }
        }

        public void Execute(ActorStopActionAndStandCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorStopActionAndStandCommand((int)_playerActor));
            }
        }

        public void Execute(ActorEnablePlayerControlCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorEnablePlayerControlCommand((int)_playerActor));
            }
            else
            {
                _playerActor = (PlayerActorId)command.ActorId;
                _playerActorControlEnabled = true;
            }
        }

        public void Execute(ActorSetNavLayerCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorSetNavLayerCommand((int)_playerActor, command.LayerIndex));
            }
        }
        
        #if PAL3A
        public void Execute(ActorSetYPositionCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorSetYPositionCommand((int)_playerActor, command.YPosition));
            }
        }
        #endif

        public void Execute(CameraFocusOnActorCommand command)
        {
            if (command.ActorId == ActorConstants.PlayerActorVirtualID)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new CameraFocusOnActorCommand((int)_playerActor));
            }
        }

        public void Execute(LongKuiSwitchModeCommand command)
        {
            _longKuiLastKnownMode = command.Mode;
        }

        public void Execute(ScenePostLoadingNotification notification)
        {
            CommandDispatcher<ICommand>.Instance.Dispatch(new ActorActivateCommand((int)_playerActor, 1));
            
            if (_playerActorControlEnabled)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(new ActorEnablePlayerControlCommand((int)_playerActor));
            }

            if (_playerInputEnabled)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(new PlayerEnableInputCommand(1));
            }
            
            #if PAL3
            CommandDispatcher<ICommand>.Instance.Dispatch(new LongKuiSwitchModeCommand(_longKuiLastKnownMode));
            #endif
        }
    }
}