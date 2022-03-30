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
using Newtonsoft.Json;

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
                DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + @"\ways");
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

                LabelMusic.Content = musicNames[musicNum];
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

            if (ComboBoxPlaylists.SelectedIndex == -1)
                return;

            bool errFileExist = false;//существует ли файл, записанный в плейлист

            string fileWay = playlists[ComboBoxPlaylists.SelectedIndex];
            StreamReader streamReader = new StreamReader(fileWay);

            try
            {
                while (!streamReader.EndOfStream)
                {
                    string str = streamReader.ReadLine();
                    if (File.Exists(str))
                        music.Add(str);
                    else errFileExist = true;
                }

                foreach (string item in music)
                    musicNames.Add(StaticFunc.parseWay(item));

                foreach (string item in musicNames)
                    ComboBoxTracks.Items.Add(item);


                if (errFileExist)
                {
                    StreamWriter streamWriter = new StreamWriter(fileWay, false);
                    foreach (string musicWay in music)
                        streamWriter.WriteLine(musicWay);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            finally
            {
                streamReader.Close();

                if (ComboBoxTracks.Items.Count > 0)
                    ComboBoxTracks.SelectedIndex = 0;
            }
        }



///////добавление и удаление треков/плейлистов
        private void ButtonNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string directory;//директория с музыкой(выбранная пользователем)
            string fileWay;//путь к создаваемому файлу (запись треков в текстовый файл в папке с программой)
            string fileName;//название нового плейлиста

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            //sfd.Filter = "cnt64 files (*.cnt64)|*.cnt64|All files (*.*)|*.*";
            sfd.CheckPathExists = true;
            sfd.RestoreDirectory = false;
            sfd.Title = "выберите директрорию с музыкой и введите имя нового плейлиста";

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
                directory = sfd.FileName;
                int end = directory.LastIndexOf("\\");
                directory = directory.Remove(end, directory.Length - end);

                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                List<string> allFiles = new List<string>();
                fileName = StaticFunc.parseWay(sfd.FileName);

                if (!replace)
                {
                    fileWay = Environment.CurrentDirectory + @"\ways\" + fileName + ".txt";
                    StaticFunc.checkAndCreateDirectory(Environment.CurrentDirectory + @"\ways");

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
                    foreach (var file in directoryInfo.GetFiles())//
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

        private void ButtonAddTrack_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = StaticFunc.createFilterStringSFD(musicExtension);
            sfd.CheckPathExists = true;
            sfd.RestoreDirectory = false;
            sfd.Title = "выберите треки";

            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
        }

        private void BtnDeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxPlaylists.SelectedIndex == -1)
                return;

            if (MessageBox.Show("Удалить плейлист?", "Внимание!",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel,
                System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.OK)
            { return; }

            int numDelete = ComboBoxPlaylists.SelectedIndex;
            StaticFunc.killProcessByName(playlistNames[numDelete], "Notepad");

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
            //int numDelFile = ComboBoxTracks.SelectedIndex;
            //ComboBoxTracks.Items.Remove(numDelFile);
        }
        /////









        ////////завершение работы и пасхалки
        
        private void killAllprocMusicFilesTXT()
        {
            foreach (string item in playlistNames)
                StaticFunc.killProcessByName(item, "Notepad");
        }

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
