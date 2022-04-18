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
using System.Threading;

using MessageBox = System.Windows.MessageBox;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();//сам плеер
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        //таймер для следящий за позицией текущего трека

        private List<string> playlistNames = new List<string>();//названия плейлистов
        private List<string> playlists = new List<string>();//пути плейлистов
        private List<string> musicNames = new List<string>();//названия песен
        private List<string> music = new List<string>();//пути к музыке
        private List<string> musicExtension = new List<string>() { ".wav", ".mp3" };//допустимые форматы музыки

        private int musicNum = 0;//номер текущего трека
        private bool musicPlay = true;//играет ли музыка
        private bool[] musicUpdate = new bool[3] { false, false, false };
        //происходит ли обновление плейлистов в данный момент/вызывались ли события во время обновления/вызвано устранение неполадок                                                          



        ///////меню
        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            switch (menuItem.Header)
            {
                case "Уведомления":
                    menuItem.IsChecked = !menuItem.IsChecked;
                    Settings.messagesActive = menuItem.IsChecked;
                    Settings.GetInstance.SaveSettings();
                    break;
                case "Предупреждения":
                    menuItem.IsChecked = !menuItem.IsChecked;
                    Settings.warningDelPlaylist = menuItem.IsChecked;
                    Settings.warningDelTrack = menuItem.IsChecked;
                    Settings.warningExistPlaylist = menuItem.IsChecked;
                    Settings.GetInstance.SaveSettings();
                    break;
                case "Музыка сразу":
                    menuItem.IsChecked = !menuItem.IsChecked;
                    Settings.musicAtTheStart = menuItem.IsChecked;
                    Settings.GetInstance.SaveSettings();
                    break;
                case "Устр. неполадок":
                    if (MessageBox.Show("Последовательное завершение всех процессов программы и перезапуск?", "Внимание!",
                        MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.Cancel,
                        System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.Yes)
                            { return; }
                    musicUpdate[2] = true;
                    mediaPlayer.Close();
                    KillAllprocMusicFilesTXT();
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                    break;
                case "Настройки":
                    MessageBox.Show("В разработке");
                    break;
            }
        }







        ////////общее
        public void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += Timer_Tick;
            timer.Start();

            Settings.GetInstance.InitializeSettings();
            foreach (MenuItem menuItem in Menu_Settings.Items)
            {
                switch (menuItem.Header)
                {
                    case "Уведомления":
                        menuItem.IsChecked = Settings.messagesActive;
                        break;
                    case "Предупреждения":
                        menuItem.IsChecked = Settings.warningDelPlaylist;
                        break;
                    case "Музыка сразу":
                        menuItem.IsChecked = Settings.musicAtTheStart;
                        break;
                }
            }
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
            { if(Settings.messagesActive) MessageBox.Show(ex.Message); }
        }





        ///////проигрывание музыки
        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (musicUpdate[0])
            { musicUpdate[1] = true; return; }

            try
            {
                if (musicNum == music.Count - 1)
                    musicNum = 0;
                else musicNum++;

                ComboBoxTracks.SelectedIndex = musicNum;
            }
            catch (Exception ex) 
            { if(Settings.messagesActive) MessageBox.Show(ex.Message); }
        }

        private void ButtonMusicPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!musicPlay)
                { mediaPlayer.Play(); musicPlay = true; }
                else
                { mediaPlayer.Pause(); musicPlay = false; }
            }
            catch (Exception ex)
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }

        private void ComboBoxTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTracks.SelectedIndex > -1)
            {
                musicNum = ComboBoxTracks.SelectedIndex;
                mediaPlayer.Open(new Uri(music[musicNum]));
                if (musicPlay)
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

            try
            {
                StreamReader streamReader = new StreamReader(fileWay);
                while (!streamReader.EndOfStream)
                {
                    string str = streamReader.ReadLine();
                    if (File.Exists(str))
                        music.Add(str);
                    else errFileExist = true;
                }
                streamReader.Close();

                foreach (string item in music)
                    musicNames.Add(StaticFunc.parseWay(item));

                foreach (string item in musicNames)
                    ComboBoxTracks.Items.Add(item);


                if (errFileExist)
                {
                    StreamWriter streamWriter = new StreamWriter(fileWay, false);
                    foreach (string musicWay in music)
                        streamWriter.WriteLine(musicWay);
                    streamWriter.Close();
                }
            }
            catch (Exception ex) 
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }

            finally
            {
                if (Settings.musicAtTheStart)
                    musicPlay = true;
                else musicPlay = false;

                if (ComboBoxTracks.Items.Count > 0)
                    ComboBoxTracks.SelectedIndex = 0;
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!mediaPlayer.NaturalDuration.HasTimeSpan)
                return;
            
            sliderMusic.Value = mediaPlayer.Position.TotalSeconds / mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds * 100;
        }

        private void SliderMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if ((e.NewValue - e.OldValue) < (10 * timer.Interval.TotalSeconds * mediaPlayer.SpeedRatio))
                {
                    sliderMusic.SelectionEnd = sliderMusic.Value;
                    string strPosition = mediaPlayer.Position.ToString();
                    int lInd = strPosition.LastIndexOf(".");
                    if (lInd != -1)
                        strPosition = strPosition.Remove(lInd, strPosition.Length - lInd);
                    LabelMusicPosition.Content = strPosition;
                }

                else
                {
                    double totalSeconds = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    double factor = (sliderMusic.Value / sliderMusic.Maximum);
                    int DurationHours = (int)((totalSeconds / 3600) * factor);
                    totalSeconds -= (DurationHours * 3600);
                    int DurationMin = (int)((totalSeconds / 60) * factor);
                    totalSeconds -= (DurationMin * 60);
                    int DurationSec = (int)(totalSeconds * factor);

                    mediaPlayer.Position = new TimeSpan(DurationHours, DurationMin, DurationSec);
                    LabelMusicPosition.Content = mediaPlayer.Position.ToString();
                    sliderMusic.SelectionEnd = sliderMusic.Value;
                }
            }
            catch (Exception ex) { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }
        private void SliderMusic_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }



        ///////добавление и удаление треков/плейлистов
        private void ButtonNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string directory;//директория с музыкой(выбранная пользователем)
            string fileWay;//путь к создаваемому файлу (запись треков в текстовый файл в папке с программой)
            string fileName;//название нового плейлиста

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.CheckFileExists = false;
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

                fileWay = Environment.CurrentDirectory + @"\ways\" + fileName + ".txt";
                StaticFunc.checkAndCreateDirectory(Environment.CurrentDirectory + @"\ways");

                bool flag = File.Exists(fileWay);
                if (flag && Settings.warningExistPlaylist)
                {
                    if (MessageBox.Show("Переписать?", "Плейлист уже существует",
                    MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.Cancel,
                    System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.Yes)
                    { return; }
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

                if (ComboBoxPlaylists.SelectedIndex == -1)
                    ComboBoxPlaylists.SelectedIndex = 0;
                else
                {
                    if (ComboBoxPlaylists.SelectedItem.ToString() == fileName)
                        ComboBoxPlaylists_SelectionChanged(null, null);
                    else ComboBoxPlaylists.SelectedItem = fileName;
                }
            }
            catch (Exception ex) 
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }

        private void ButtonAddTrack_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = StaticFunc.createFilterStringSFD(musicExtension);
            ofd.CheckPathExists = true;
            ofd.RestoreDirectory = false;
            ofd.Multiselect = true;
            ofd.Title = "выберите треки";

            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            musicUpdate[0] = true;
            ComboBoxPlaylists.IsEnabled = false;
            ComboBoxTracks.IsEnabled = false;

            List<string> existTracksList = new List<string>();
            List<string> localList = new List<string>();
            foreach (string fileName in ofd.FileNames)
            {
                if (!music.Contains(fileName))
                    localList.Add(fileName);
                else existTracksList.Add(fileName);
            }

            int newMusicNum = musicNum + 1;
            music.InsertRange(newMusicNum, localList);
            localList = StaticFunc.parseWays(localList);
            musicNames.InsertRange(newMusicNum, localList);
            foreach (string lName in localList)
            { ComboBoxTracks.Items.Insert(newMusicNum, lName); newMusicNum++; }

            try
            {
                StreamWriter streamWriter = new StreamWriter(playlists[ComboBoxPlaylists.SelectedIndex]);
                foreach (string track in music)//
                    streamWriter.WriteLine(track);
                streamWriter.Close();
            }
            catch(Exception ex)
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }

            musicUpdate[0] = false;
            ComboBoxPlaylists.IsEnabled = true;
            ComboBoxTracks.IsEnabled = true;
            if (musicUpdate[1])
                MediaPlayer_MediaEnded(null, null);
            musicUpdate[1] = false;

            if (existTracksList.Count != 0 && Settings.messagesActive)
                MessageBox.Show("Нельзя добавить треки, которые уже есть в плейлисте\n"+existTracksList,
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnDeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxPlaylists.SelectedIndex == -1)
                return;

            if (Settings.warningDelPlaylist)
            {
                if (MessageBox.Show("Удалить плейлист?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel,
                System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.Yes)
                { return; }
            }

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
            catch(Exception ex) 
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }

        private void BtnDeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.warningDelTrack)
            {
                if (MessageBox.Show("Удалить трек?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel,
                System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.Yes)
                { return; }
            }

            ComboBoxPlaylists.IsEnabled = false;

            MediaPlayer_MediaEnded(null, null);
            ComboBoxTracks.Items.RemoveAt(musicNum - 1);
            music.RemoveAt(musicNum - 1);
            musicNames.RemoveAt(musicNum - 1);

            try
            {
                StreamWriter streamWriter = new StreamWriter(playlists[ComboBoxPlaylists.SelectedIndex]);
                foreach (string track in music)//
                    streamWriter.WriteLine(track);
                streamWriter.Close();
            }
            catch (Exception ex) 
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }

            ComboBoxPlaylists.IsEnabled = true;
        }
        /////









        ////////завершение работы и пасхалки
        
        private void KillAllprocMusicFilesTXT()
        {
            foreach (string item in playlistNames)
                StaticFunc.killProcessByName(item, "Notepad");
        }

        private void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (musicUpdate[2] != true)
            {
                if (MessageBox.Show("Завершение работы?", "Внимание!",
                        MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel,
                        System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.OK)
                { e.Cancel = true; return; }
            }

            try { mediaPlayer.Close(); }
            catch (Exception ex)
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }

    }
}
