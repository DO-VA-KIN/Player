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
using System.Diagnostics;

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

        private void ComboBoxTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTracks.SelectedIndex > -1)
            {
                musicNum = ComboBoxTracks.SelectedIndex;
                mediaPlayer.Open(new Uri(music[musicNum]));
                mediaPlayer.Play();
            }
        }

        private void ComboBoxPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxPlaylists.SelectedIndex == -1)
                return;

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
                    musicNames.Add(StaticFunc.parseWay(item));

                foreach (string item in musicNames)
                    ComboBoxTracks.Items.Add(item);

                if (ComboBoxTracks.Items.Count > 0)
                    ComboBoxTracks.SelectedIndex = 0;

                streamReader.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }           
        }



///////добавление и удаление треков/плейлистов
        private void ButtonNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string fileWay;//путь к создаваемому файлу
            string fileName;//название нового плейлиста

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            //sfd.Filter = "cnt64 files (*.cnt64)|*.cnt64|All files (*.*)|*.*";
            sfd.CheckPathExists = true;
            sfd.RestoreDirectory = false;
            sfd.Title = "выберите директрорию и введите имя нового плейлиста";

            if (playlists.Count > 0)
            {
                string str = playlists[playlists.Count - 1];
                int end = str.LastIndexOf("\\");
                str = str.Remove(end, str.Length - end);
                sfd.InitialDirectory = str;
            }

            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {

                DirectoryInfo directoryInfo = new DirectoryInfo(sfd.InitialDirectory);
                List<string> allFiles = new List<string>();
                fileName = StaticFunc.parseWay(sfd.FileName);

                if (!replace)
                {
                    //fileWay = Environment.CurrentDirectory + @"\ways" + newPlaylistName + ".txt";
                    fileWay = @"C:\Users\pressF\Documents\Visual Studio 2017\Projects\Player\Player\ways\" + fileName + ".txt";

                    bool flag = File.Exists(fileWay);
                    if (flag)
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

                    playlistNames.Add(fileName);
                    playlists.Add(fileWay);

                    if (!flag)
                        ComboBoxPlaylists.Items.Add(fileName);
                    ComboBoxPlaylists.SelectedItem = fileName;
                    //ComboBoxPlaylists.SelectedIndex = ComboBoxPlaylists.Items.Count - 1;
                }
                else { }//
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnDeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить плейлист?", "Внимание!",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel,
                System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.OK)
            { return; }

            int numDelete = ComboBoxPlaylists.SelectedIndex;
            Process[] procFile = Process.GetProcessesByName("Notepad");
            foreach(Process process in  procFile)
            {
                if (StaticFunc.parseWay(process.MainWindowTitle) == playlistNames[numDelete])
                    process.Kill();
            }

            //ComboBoxPlaylists.SelectedIndex = -1;
            try
            {
                File.Delete(playlists[numDelete]);
                ComboBoxTracks.Items.Clear();
                ComboBoxPlaylists.Items.RemoveAt(numDelete);
                playlists.RemoveAt(numDelete);

                if (playlists.Count > numDelete)
                    ComboBoxPlaylists.SelectedIndex = numDelete;
                else if (playlists.Count != 0 & numDelete != 0)
                    ComboBoxPlaylists.SelectedIndex = numDelete - 1;
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnDeleteTrack_Click(object sender, RoutedEventArgs e)
        {

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
