using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RROMediaPlayer
{
    /// <summary>
    /// Assignment1- Ricardo Oliveira - W048722 - 02/2021
    /// Interaction logic for MainWindow.xaml
    /// References:
    /// https://wpf-tutorial.com/audio-video/how-to-creating-a-complete-audio-video-player/
    /// https://youtu.be/k9Dm13xhrdY
    /// MusicIcon = https://images.app.goo.gl/J3uFhzXuFmDxdJ3r5 (Free sourcing)
    /// </summary>
    public partial class MainWindow : Window    
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        TagLib.File file;

        public MainWindow()
        {
            InitializeComponent();

            ////UI
            //string ImgPath = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString()}\\Images\\MusicIcon.png";
            //Musicimg.Source = new BitmapImage(new Uri(ImgPath));

            //Player
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                timerSlider.Minimum = 0;
                timerSlider.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                timerSlider.Value = mePlayer.Position.TotalSeconds;
            }
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.Filter = "Media Files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    mePlayer.Source = new Uri(openFileDialog.FileName);

                    file = TagLib.File.Create(openFileDialog.FileName);

                    if (file.Tag.Title is null)
                    {
                        labelSongName.Text = openFileDialog.FileName;
                    } else
                    {
                        labelSongName.Text = (file.Tag.Title + "; " + file.Tag.FirstPerformer);
                        labelSongAtt.Text = (file.Tag.Album + "; " + file.Tag.Year);
                    }

                    
                }
            }
            catch (Exception ex) {
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            };
            
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        }   

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Stop();
            mediaPlayerIsPlaying = false;
        }

        private void sliProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mePlayer.Position = TimeSpan.FromSeconds(timerSlider.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelCurrentTime.Text = TimeSpan.FromSeconds(timerSlider.Value).ToString(@"hh\:mm\:ss");
        }
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void btnTagEdit_Click(object sender, RoutedEventArgs e)
        {
            editorPanel.Visibility = Visibility.Visible;
            if (file.Tag.Title == "")
            {
                eTitleTag.Text = labelSongName.Text;
                eArtistTag.Text = "Please update Tag";
                eAlbumTag.Text = "Please update Tag";
                eYearTag.Text = "Please update Tag";
            }
            eTitleTag.Text = (file.Tag.Title);
            eArtistTag.Text = (file.Tag.FirstPerformer);
            eAlbumTag.Text = (file.Tag.Album);
            eYearTag.Text = (file.Tag.Year).ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                file.Tag.Title = eTitleTag.Text;
                file.Tag.Performers = new String[1] { eArtistTag.Text };
                file.Tag.Album = eAlbumTag.Text;
                file.Tag.Year = Convert.ToUInt32(eYearTag.Text);
                file.Save();
                labelSongName.Text = (file.Tag.Title + "; " + file.Tag.FirstPerformer);
                labelSongAtt.Text = (file.Tag.Album + "; " + file.Tag.Year);
                editorPanel.Visibility = Visibility.Hidden;
            } catch ( Exception ex )
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
