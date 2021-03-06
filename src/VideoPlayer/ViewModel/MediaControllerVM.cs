﻿using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Movies.MediaService.Interfaces;
using System.Collections.Generic;
using Movies.MediaService.Models;
using System.Windows.Controls.Primitives;

namespace VideoPlayerControl.ViewModel
{
    public partial class MediaControllerViewModel : INotifyPropertyChanged, IMediaControllerViewModel
    {
        private bool HasSubcribed = false;
        private VideoFolderChild currentvideoitem;
        private bool ismousecontrolover;
        private DelegateCommand playbtn;
        private DelegateCommand repeatbtn;
        private DelegateCommand mute;
        private string playtext;
        private DelegateCommand _next;
        private DelegateCommand _prev;
        public DelegateCommand<Popup> showmenucommand;
        private bool isplaying;
        private DelegateCommand tofullscreenbtn;
        private bool cananimate;
        private bool IsDirectoryChanged;
        private RepeatMode repeatmode = RepeatMode.NoRepeat;
        private TimeSpan lastseentime;
        private bool haslastseen;
        private bool isdragging = false;
        private VolumeState volumestate = VolumeState.Active;
        
        IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }

        IMediaPlayerService MediaPlayerService
        {
            get
            {
                return FilePlayerManager.MediaPlayerService;
            }
        }

        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

        public bool IsDragging { get { return isdragging; } set { isdragging = value; } }
        public ExecuteCommand CurrentVideoItemChangedEvent { get; set; }
        public DispatcherTimer PositionSlideTimerTooltip { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SubtitleChanged;
        public VolumeState VolumeState { get{return volumestate;} set{volumestate = value;}} 
        public DelegateCommand CloseLastSeenCommand { get; private set; }
        public DelegateCommand SetLastSeenCommand { get; private set; }
        public bool IsRewindOrFastForward { get; set; }
        

        public VideoFolderChild CurrentVideoItem
        {
            get { return currentvideoitem; }
        }

        public DelegateCommand Next
        {
            get
            {
                if (_next == null)
                {
                    _next = new DelegateCommand(NextPlayAction, CanNext);
                }
                return _next;
            }
        }

        public DelegateCommand ToFullScreenBtn
        {
            get
            {
                if (tofullscreenbtn == null)
                {
                    tofullscreenbtn = new DelegateCommand(() =>
                    {
                        (((SubtitleMediaController)IVideoPlayer).DataContext as VideoPlayerVM)
                        .FullScreenAction();
                    });
                }
                return tofullscreenbtn;
            }
        }

        public DelegateCommand RepeatBtn
        {
            get
            {
                if (repeatbtn == null)
                {
                    repeatbtn = new DelegateCommand(RepeatBtnAction);
                }
                return repeatbtn;
            }
        }

        public DelegateCommand Mute
        {
            get
            {
                if (mute == null)
                {
                    mute = new DelegateCommand(MuteAction);
                }
                return mute;
            }
            set
            {
                mute = value;
            }
        }
        
        public bool HaslastSeen
        {
            get { return haslastseen; }
            set { haslastseen = value; OnPropertyChanged("HaslastSeen"); }
        }

        public TimeSpan LastSeenTime
        {
            get { return lastseentime; }
            set { lastseentime = value; OnPropertyChanged("LastSeenTime"); }
        }

        public RepeatMode RepeatMode
        {
            get { return repeatmode; }
            set { repeatmode = value; OnPropertyChanged("RepeatMode"); }
        }

        public IMovieTitle_Tab IMovieTitle_Tab
        {
            get { return (IVideoPlayer.MediaController as IControllerView).MovieTitle_Tab; }
        }

        public Slider DragPositionSlider
        {
            get { return DragProgressSliderPart.ProgressSlider; }
        }

        private Border Border
        {
            get
            {
                return BorderPart.Border;
            }
        }

        public Slider VolumeSlider
        {
            get { return VolumeControl.CurrentVolumeSlider; }
        }

        public DelegateCommand PlayBtn
        {
            get
            {
                if (playbtn == null)
                {
                    playbtn = new DelegateCommand(() =>
                    {
                        PlayAction();
                    }, CanPlay);
                }

                return playbtn;
            }
            set
            {
                playbtn = value;
            }
        }

        public DelegateCommand Previous
        {
            get
            {
                if (_prev == null)
                {
                    _prev = new DelegateCommand(PrevPlayAction, CanPrev);
                }
                return _prev;
            }
        }

        public DelegateCommand<Popup> ShowMenuCommand
        {
            get
            {
                if (showmenucommand == null)
                    showmenucommand = new DelegateCommand<Popup>((popup) => {
                        if (popup.IsOpen)
                            popup.IsOpen = false;
                        else
                            popup.IsOpen = true;
                    });
                return showmenucommand;
            }
        }


        public string PlayText
        {
            get { return playtext; }
            set
            {
                playtext = value; OnPropertyChanged("PlayText");
            }
        }

        public bool IsPlaying
        {
            get { return isplaying; }
            set { isplaying = value; OnPropertyChanged("IsPlaying"); }
        }

        public bool IsMouseControlOver
        {
            get
            {
                return ismousecontrolover;
            }
            set
            {
                ismousecontrolover = value;
                OnPropertyChanged("IsMouseControlOver");
            }
        }

        public bool CanAnimate
        {
            get { return cananimate; }
            set
            {
                cananimate = value; OnPropertyChanged("CanAnimate");
                //PositioSliderInit(HasSubcribed); 
            }
        }

        public IMediaController IVideoPlayer
        {
            get
            {
                return IVideoElement.IVideoPlayerController;
            }
        }

        public IVideoElement IVideoElement
        {
            get
            {
                return FilePlayerManager.VideoElement;
            }
        }

        public IDispatcherService DispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        public MovieMediaState MediaState { get { return MediaPlayerService.State; } }

        
    }
}
