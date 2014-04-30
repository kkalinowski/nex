using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for VideoPreview.xaml
    /// </summary>
    public partial class AudioVideoPreview : UserControl
    {
        #region PlayingStatus
        public enum PlayingStatus
        {
            Stop,
            Play,
            Pause
        } 
        #endregion

        #region Fields
        private string mediaPath;
        private bool changingTimelinePos = false;
        private Storyboard storyboard;
        private PlayingStatus status; 
        #endregion

        #region DProps
        /// <summary>
        /// Determines if playing video or audio
        /// </summary>
        public bool PlayingVideo
        {
            get
            {
                return (bool)GetValue(PlayingVideoProperty);
            }
            set
            {
                SetValue(PlayingVideoProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PlayingVideo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayingVideoProperty =
            DependencyProperty.Register("PlayingVideo", typeof(bool), typeof(AudioVideoPreview));
        #endregion

        public AudioVideoPreview()
        {
            InitializeComponent();
            meContent.Volume = sVolume.Value;
        }

        /// <summary>
        /// Support the event bPlayPause click - play/pause media
        /// </summary>
        private void bPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (status == PlayingStatus.Stop)
            {
                bPlayPause.Content = "Pause";
                Play();
            }
            else if (status == PlayingStatus.Play)
            {
                bPlayPause.Content = "Play";
                Pause();
            }
            else
            {
                bPlayPause.Content = "Pause";
                Resume();
            }
        }

        private void Play()
        {
            status = PlayingStatus.Play;
            BeginStoryboard(storyboard, HandoffBehavior.SnapshotAndReplace, true);
        }

        private void Pause()
        {
            status = PlayingStatus.Pause;
            storyboard.Pause(this);
        }

        private void Resume()
        {
            status = PlayingStatus.Play;
            storyboard.Resume(this);
        }

        /// <summary>
        /// Support the event bStop click - stop media
        /// </summary>
        private void bStop_Click(object sender, RoutedEventArgs e)
        {
            status = PlayingStatus.Stop;
            bPlayPause.Content = "Play";
            storyboard.Stop(this);
        }

        /// <summary>
        /// Support the event sVolume ValueChanged - change volume of media
        /// </summary>
        private void sVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            meContent.Volume = sVolume.Value;
        }

        /// <summary>
        /// Support the event sSeek ValueChanged - change position of media
        /// </summary>
        private void sSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (changingTimelinePos)
            {
                int sliderVal = (int)sSeek.Value;
                TimeSpan time = new TimeSpan(0, 0, 0, 0, sliderVal);

                //FIX: Click to set position not working
                storyboard.SeekAlignedToLastTick(this, time, TimeSeekOrigin.BeginTime);
            }
        }

        /// <summary>
        /// Support the event meContent MediaOpened - initialize sSeek
        /// </summary>
        private void meContent_MediaOpened(object sender, RoutedEventArgs e)
        {
            sSeek.Maximum = meContent.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        /// <summary>
        /// Support the event meContent MediaEnded - stop the media
        /// </summary>
        private void meContent_MediaEnded(object sender, RoutedEventArgs e)
        {
            //QSTN: Why this is not working?
            bStop_Click(null, null);
        }

        /// <summary>
        /// Load a movie file
        /// </summary>
        /// <param name="path">Path to file</param>
        public void LoadVideo(string path)
        {
            mediaPath = path;
            PlayingVideo = true;
            meContent.ScrubbingEnabled = true;
            meContent.Source = new Uri(path);
        }

        /// <summary>
        /// Load a audio file
        /// </summary>
        /// <param name="path">Path to file</param>
        public void LoadAudio(string path)
        {
            mediaPath = path;
            PlayingVideo = false;
            meContent.ScrubbingEnabled = false;
            meContent.Source = new Uri(path);
        }

        private void meContent_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard = (Storyboard)_AudioVideoPreview.Resources["sbStory"];
            MediaTimeline timeline = (MediaTimeline)storyboard.Children[0];
            timeline.Source = new Uri(mediaPath);

            if (PlayingVideo)
            {
                Play();
                Pause();
            }
        }

        private void MediaTimeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (!changingTimelinePos)
                sSeek.Value = meContent.Position.TotalMilliseconds;
        }

        private void sSeek_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            changingTimelinePos = true;
            Pause();
        }

        private void sSeek_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            changingTimelinePos = false;
            Resume();
        }
    }
}