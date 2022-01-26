using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Controls;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using System.Media;
using System.Windows.Media;

using MessageBox = System.Windows.MessageBox;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();//сам плеер
        private List<string> playlistNames = new List<string>();//названия плейлистов
        private List<string> playlists = new List<string>();//пути плейлистов
        private List<string> musicNames = new List<string>();//названия песен
        private List<string> music = new List<string>();//пути к музыке
        private List<string> musicExtension = new List<string>() { ".wav", ".mp3" };//допустимые форматы музыки

        private int musicNum = 0;//номер текущего трека

        private string newPlaylistName = "1";
        private bool replace = false;//перенос музыки в папку программы (если нет, то сохраняются пути)
        //private uint sumMusic = 1000;//максимальное кол-во путей треков подгружаемых



        ////////общее
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            try
            {
                //DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + @"ways");//перед сборкой удалить нижнюю строку и раскомментить эту
                DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\Users\pressF\Documents\Visual Studio 2017\Projects\Player\Player\ways");
                foreach (var file in directoryInfo.GetFiles())
                {
                    string name = file.Name.Replace(file.Extension, "");

                    playlists.Add(file.FullName);
                    playlistNames.Add(name);
                    ComboBoxPlaylists.Items.Add(name);
                }

                if (playlists.Count > 0)
                {
                    LabelMusic.Content = "";
                    ComboBoxPlaylists.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }


        public string parseWay(string way)
        {
            int end = way.LastIndexOf(".");
            int beg = way.LastIndexOf("\\");
            way = way.Remove(end, way.Length - end);
            way = way.Remove(0, beg + 1);
            return way;
        }
        //////




        ///////проигрывание музыки
        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            try
            {
                if (musicNum == music.Count - 1)
                    musicNum = 0;
                else musicNum++;

                ComboBoxTracks.SelectedIndex = musicNum;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ButtonMusicPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mediaPlayer.IsMuted)
                {
                    mediaPlayer.Play(); mediaPlayer.IsMuted = false;
                }
                else
                {
                    mediaPlayer.Pause(); mediaPlayer.IsMuted = true;
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        ///////





////////работа с директориями и плейлистами
        private void ButtonNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string fileWay;//путь к создаваемому файлу
            newPlaylistName = TextBoxPlaylistName.Text;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Выберите директорию для поиска музыки";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            if (playlists.Count > 0)
            {
                string str = playlists[playlists.Count - 1];
                int end = str.LastIndexOf("\\");
                str = str.Remove(end, str.Length - end);
                fbd.SelectedPath = str;
            }

            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(fbd.SelectedPath);
                List<string> allFiles = new List<string>();

                if (!replace)
                {
                    //fileWay = Environment.CurrentDirectory + @"\ways" + newPlaylistName + ".txt";
                    fileWay = @"C:\Users\pressF\Documents\Visual Studio 2017\Projects\Player\Player\ways\" + newPlaylistName + ".txt";

                    if (File.Exists(fileWay))
                        {
                        if (MessageBox.Show("Переписать?", "Плейлист уже существует",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.Cancel,
                        System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.OK)
                        {
                            return;
                        }
                    }

                    StreamWriter streamWriter = new StreamWriter(fileWay);

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        if (musicExtension.Contains(file.Extension))
                            streamWriter.WriteLine(file.FullName);
                    }
                    streamWriter.Close();

                    playlistNames.Add(newPlaylistName);
                    playlists.Add(fileWay);

                    ComboBoxPlaylists.Items.Add(newPlaylistName);
                    ComboBoxPlaylists.SelectedItem = newPlaylistName;
                    //ComboBoxPlaylists.SelectedIndex = ComboBoxPlaylists.Items.Count - 1;
                }
                else { }//
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }


        private void ComboBoxTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTracks.SelectedIndex > -1)
            {
                mediaPlayer.Open(new Uri(music[ComboBoxTracks.SelectedIndex]));
                mediaPlayer.Play();
            }
        }

        private void ComboBoxPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaPlayer.Close();
            LabelMusic.Content = "";
            ComboBoxTracks.Items.Clear();

            music.Clear();
            musicNames.Clear();
            musicNum = 0;

            string fileWay = playlists[ComboBoxPlaylists.SelectedIndex];

            try
            {
                StreamReader streamReader = new StreamReader(fileWay);

                while (!streamReader.EndOfStream)
                    music.Add(streamReader.ReadLine());

                foreach (string item in music)
                    musicNames.Add(parseWay(item));

                foreach (string item in musicNames)
                    ComboBoxTracks.Items.Add(item);

                ComboBoxTracks.SelectedIndex = 0;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }           
        }


        /////









        ////////завершение работы и пасхалки
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Завершение работы?", "Внимание!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel,
                    System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                try { mediaPlayer.Close(); }
                catch (Exception ex)
                { MessageBox.Show(ex.Message); }
            }
        }
    }
}
