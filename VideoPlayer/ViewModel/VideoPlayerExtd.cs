﻿using Common.ApplicationCommands;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using VideoComponent.BaseClass;
using VirtualizingListView.ViewModel;

namespace VideoPlayerControl.ViewModel
{
    public partial class VideoPlayerVM
    {
        private void RegisterCommands()
        {
           // ICommandBindings.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FileExplorer, FileExplorer_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.PlayList,
                PlayList_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Mute, 
                Mute_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Next, 
                Next_executed,Next_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.PausePlay,
                PausePlay_executed));//,PausePlay_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Previous,
                Previous_executed, Previous_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Stop, 
                Stop_executed,CommandEnabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.VolDown,
                VolDown_executed, Vol_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.VolUp, 
                VolUp_executed,Vol_enabled));
           // IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.AddtoPlayList, AddtoPlayList_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.RemovefromPlayList,
                RemovefromPlayList_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Play ,Play_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.TopMost,
                TopMost_executed));


            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FullScreen,
                FullScreen_executed));//, FullScreen_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Rewind, 
                Rewind_executed, Rewind_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ShiftRewind,
                ShiftRewind_executed, Rewind_enabled));
            
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.SelectedSub, 
                SelectedSub_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FastForward, 
                FastForward_executed, Rewind_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ShiftFastForward,
               ShiftFastForward_executed, Rewind_enabled));

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.AddSubFile, BrowerSubFile));

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ResizeMediaAlways,
              ResizeMediaAlways_executed));
            // IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.MinimizeMediaCtrl, MinimizeMediaCtrl_executed));
        }
        
        private void ResizeMediaAlways_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.AllowAutoResize)
            {
                this.AllowAutoResize = false;
            }
            else
            {
                this.AllowAutoResize = true;
            }
        }

        private void BrowerSubFile(object sender, ExecutedRoutedEventArgs e)
        {
            var openfiles = new Microsoft.Win32.OpenFileDialog();
            openfiles.Filter = "All files(*.*)|*.srt;";
            if (openfiles.ShowDialog() == true)
            {
                AddSubtitleFileAction(new string[] { openfiles.FileName });
            }
        }

        private void ShiftFastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Time += TimeSpan.FromMilliseconds(1500);
        }

        private void ShiftRewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Time -= TimeSpan.FromMilliseconds(1500);
        }

        private void DisableSubtitle_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IVideoElement.MediaPlayer.VlcMediaPlayer.SubtitleCount != 0;
        }

        public void RestoreMediaState()
        {
            //IVideoElement.MediaPlayer.ScrubbingEnabled = false;
            if (MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Playing)
                IVideoElement.MediaPlayer.Play();

            MediaControllerVM.MediaControllerInstance.IsRewindOrFastForward = false;
            if (MediaControllerVM.MediaControllerInstance.VolumeState
                == VolumeState.Active)
            {
                IVideoElement.MediaPlayer.IsMute = false;
            }
        }

        private void SelectedSub_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentSubtitle == e.Parameter as SubtitleFilesModel)
            {
                CurrentSubtitle.IsSelected = true;
                return;
            }
            CurrentSubtitle = e.Parameter as SubtitleFilesModel;
            var subtitlecollection = MediaControllerVM.CurrentVideoItem.SubPath;
            int vlcsubitemId = IVideoElement.MediaPlayer.VlcMediaPlayer.Subtitle;
            foreach (var item in subtitlecollection)
            {
                item.IsSelected =  false;
            }
            if(CurrentSubtitle.SubtitleType == SubtitleType.HardCoded &&
                IVideoElement.MediaPlayer.VlcMediaPlayer.SubtitleDescription.Count > 0)
            {
                IVideoElement.MediaPlayer.VlcMediaPlayer.Subtitle = CurrentSubtitle.Id;
                CurrentSubtitle.IsSelected = true;
            }
        }
        
        private void TopMost_executed(object sender, ExecutedRoutedEventArgs e)
        {
            IVideoElement.SetTopMost();
        }

        private void FastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Time += TimeSpan.FromMilliseconds(10000);
            if (e.OriginalSource is Button)
            {
                RestoreMediaState();
            }
        }

        private void Rewind_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (IVideoElement.MediaPlayer.VlcMediaPlayer.Media != null || !IVideoElement.MediaPlayer.VlcMediaPlayer.HasVideo)
            //{
            //    e.CanExecute = false;
            //    return;
            //}
            e.CanExecute =  IVideoElement.MediaPlayer.VlcMediaPlayer.IsSeekable;
        }

        private void Rewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Time -= TimeSpan.FromMilliseconds(10000);
            if (e.OriginalSource is Button)
            {
                RestoreMediaState();
            }
            //IVideoPlayer.MediaPlayer.Play();
        }

        private void ReWindFastForward()
        {
            if (!MediaControllerVM.IsRewindOrFastForward)
            {
                MediaControllerVM.IsRewindOrFastForward = true;
                if (MediaControllerVM.MediaState == MediaState.Playing)
                    IVideoElement.MediaPlayer.Pause();
                
                if (MediaControllerVM.VolumeState == VolumeState.Active){
                    IVideoElement.MediaPlayer.IsMute = true;
                }

                ResetVisibilityAnimation();
            }
        }

        public void Loaded()
        {
            Isloaded = true;
            //IVideoElement.WindowsTab.MouseEnter += WindowsTab_MouseEnter;
            //IVideoElement.WindowsTab.MouseLeave += WindowsTab_MouseLeave;
        }

        private void FullScreen_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MinimizeMediaCtrl_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (screensetting == SCREENSETTINGS.Normal)
            {
                FullScreenSettings();
               IsFullScreenMode = true;
                (IMediaController as SubtitleMediaController).
                    OnScreenSettingsChanged(new object[] { null });
            }
            else
            {
                IsFullScreenMode = false;
                NormalScreenSettings();
                (IMediaController as SubtitleMediaController).
                    OnScreenSettingsChanged(new object[] { null });
            }
            
        }

        public void FullScreenAction()
        {
            if (!IsFullScreenMode)
            {
                ((SubtitleMediaController)IMediaController).OnScreenSettingsChanged(
                                new object[] { SCREENSETTINGS.Fullscreen, SCREENSETTINGS.Fullscreen });
                IsFullScreenMode = true;
                (((SubtitleMediaController)IMediaController).DataContext as VideoPlayerVM).FullScreenSettings();
            }
            else
                RestoreScreen();
        }

        private void FullScreen_executed(object sender, ExecutedRoutedEventArgs e)
        {
            FullScreenAction();
        }

        private void PausePlay_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IVideoElement.MediaPlayer.Length != TimeSpan.Zero;
        }

        private void RemovefromPlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                MediaControllerVM.MediaControllerInstance.Playlist.Remove(vfc);
            }
        }
        
        private void Vol_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
           e.CanExecute = !IVideoElement.MediaPlayer.IsMute;
        }

        private void Play_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                MediaControllerVM.MediaControllerInstance.GetVideoItem(vfc, true);
            }
        }

        private void Next_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaControllerVM.CanNext();
        }

        private void Previous_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaControllerVM.CanPrev();
        }

        private void CommandEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaControllerVM.IVideoElement.MediaPlayer.Length != TimeSpan.Zero;
        }

        private void VolUp_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            VolumeControl.CurrentVolumeSlider.Value += 10;
        }

        private void VolDown_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            VolumeControl.CurrentVolumeSlider.Value -= 10;
        }

        private void Stop_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaControllerVM.MediaState = MediaState.Stopped;
        }

        private void Previous_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            if (MediaControllerVM.MediaControllerInstance.DragPositionSlider.Value > 50)
                IVideoElement.MediaPlayer.Time = TimeSpan.FromMilliseconds(0);
            else
                MediaControllerVM.MediaControllerInstance.PrevPlayAction();
        }

        private void PausePlay_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            MediaControllerVM.MediaControllerInstance.PlayAction();
        }

        private void Next_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            MediaControllerVM.MediaControllerInstance.NextPlayAction();
        }

        private void Mute_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            MediaControllerVM.MediaControllerInstance.MuteAction();
        }

        private void PlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistView plv = IVideoElement.PlayListView as PlaylistView;
            plv.OnPlaylistCloseExecute(this);
        }

        private void FileExplorer_executed(object sender, ExecutedRoutedEventArgs e)
        {
            CollectionViewModel.Instance.CloseFileExplorerAction(this);
        }
        
    }
}
