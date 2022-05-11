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
                    if (MessageBox.Show(" Последовательное завершение всех\nпроцессов программы и перезапуск?", "Внимание!",
                        MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.Cancel,
                        System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.Yes)
                            { return; }
                    try
                    {
                        musicUpdate[2] = true;
                        mediaPlayer.Close();
                        KillAllprocMusicFilesTXT();
                        File.Delete(Environment.CurrentDirectory + @"\Settings.xml");
                        System.Windows.Forms.Application.Restart();
                        System.Windows.Application.Current.Shutdown();
                    }
                    catch { MessageBox.Show("Для восстановления работоспособности побробуйте: " +
                        "\n1) Удалите файл 'Settings.xml' в папке с программой" +
                        "\n2) Завершите все процессы с текстовыми файлами из папки 'ways'" +
                        "\n☼ или перезагрузи комп ♥" +
                        "\n♥ или переустанови прогу☼",
                        "Программа не смогла устранить неполадки",
                        MessageBoxButton.OK, MessageBoxImage.Warning); }
                    break;
                case "Настройки":
                    MessageBox.Show("В разработке");
                    break;
            }
        }
        private void MenuFile_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            try
            {
                switch (menuItem.Header)
                {
                    case "Закрыть":
                        if (MessageBox.Show("Завершение работы?", "Внимание!",
                       MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel,
                       System.Windows.MessageBoxOptions.ServiceNotification) != MessageBoxResult.OK)
                            return;
                        musicUpdate[2] = true;
                        System.Windows.Application.Current.Shutdown();
                        break;
                    case "Расположение":
                        Process.Start(Environment.CurrentDirectory);
                        break;
                    case "О разработчике":
                        MessageBox.Show("Written By GHOST?", "Да, я написал эту хрень в 2022!",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
            catch (Exception ex)
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }



        ////////общее
        public void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += SliderMusic_ValueChanged;
            timer.Start();

            sliderVolume.ValueChanged += SliderVolume_ValueChanged;
            sliderBalance.ValueChanged += SliderBalance_ValueChanged;
            sliderSpeed.ValueChanged += SliderSpeed_ValueChanged;

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




        ///////настройка плеера (громкость/скорость/баланс)
        private void TextBoxVolume_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = true;

            if (!Char.IsDigit(e.Text, 0))
                return;

            ushort volume = Convert.ToUInt16(textBoxVolume.Text + e.Text);
            if (volume > 100) volume = Convert.ToUInt16(e.Text);

            sliderVolume.Value = volume;
        }
        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderVolume.SelectionEnd = sliderVolume.Value;
            textBoxVolume.Text = sliderVolume.Value.ToString();

            mediaPlayer.Volume = sliderVolume.Value / sliderVolume.Maximum;
        }

        private void TextBoxSpeed_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = true;

            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",") || (e.Text == ".") && (!textBoxSpeed.Text.Contains(","))
                && (!textBoxSpeed.Text.Contains(".")) && (textBoxSpeed.Text.Length != 0)))
            {  return; }


            textBoxSpeed.Text += e.Text;
            textBoxSpeed.Text = textBoxSpeed.Text.Replace(".", ",");
            if (textBoxSpeed.Text.Length > 3)
            {
                if (textBoxSpeed.Text[textBoxSpeed.Text.Length-2] == '0' && e.Text != "0" && textBoxSpeed.Text[0] != '5')
                {
                    textBoxSpeed.Text = textBoxSpeed.Text.Remove(textBoxSpeed.Text.Length - 2, 2);
                    textBoxSpeed.Text += e.Text;
                }
                else textBoxSpeed.Text = e.Text;
            }

            double speed = -1;

            try { speed = Convert.ToDouble(textBoxSpeed.Text); }
            catch { }

            if (speed != -1)
            {
                if (speed > 5)
                {
                    speed = 5;
                    textBoxSpeed.Text = speed.ToString("0.0");
                }

                if (textBoxSpeed.Text.Length == 2)
                    return;

                if (sliderSpeed.Value == speed)
                    SliderSpeed_ValueChanged(null, null);
                else
                    sliderSpeed.Value = speed;
            }
        }
        private void SliderSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.SpeedRatio = sliderSpeed.SelectionEnd = sliderSpeed.Value;
            textBoxSpeed.Text = sliderSpeed.Value.ToString("0.0");
            textBoxSpeed.SelectionStart = textBoxSpeed.Text.Length;
        }

        private void SliderBalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Balance = sliderBalance.Value / (sliderBalance.Maximum);
            if (sliderBalance.Value > 0)
            {
                sliderBalance.IsSelectionRangeEnabled = true;
                sliderBalance.SelectionStart = sliderBalance.Minimum;
                sliderBalance.SelectionEnd = sliderBalance.Value;
            }
            else if(sliderBalance.Value < 0)
            {
                sliderBalance.IsSelectionRangeEnabled = true;
                sliderBalance.SelectionStart = sliderBalance.Value;
                sliderBalance.SelectionEnd = sliderBalance.Maximum;
            }
            else sliderBalance.IsSelectionRangeEnabled = false;
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
                {
                    mediaPlayer.Play();
                    musicPlay = true;
                }
                else
                {
                    mediaPlayer.Pause();
                    musicPlay = false;
                    sliderSpeed.Value = 10;
                }
            }
            catch (Exception ex)
            { if (Settings.messagesActive) MessageBox.Show(ex.Message); }
        }


        private void ButtonMusicBack_Click(object sender, RoutedEventArgs e)
        {
            if (musicUpdate[0]) return;

            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                if(mediaPlayer.Position.TotalSeconds / mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 0.1)
                {
                    mediaPlayer.Position = new TimeSpan(0, 0, 0);
                    return;
                }
            }

            switch (musicNum)
            {
                case 0:
                    musicNum = music.Count - 2;
                    break;
                case 1:
                    musicNum = music.Count - 1;
                    break;
                default:
                    musicNum -= 2;
                    break;
            }
            MediaPlayer_MediaEnded(null, null);
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


        private void SliderMusic_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (sliderClass.retValueChanged) return;
                if (!mediaPlayer.NaturalDuration.HasTimeSpan) return;

                if (Math.Abs(sliderClass.oldValue - sliderMusic.Value) < (2 * timer.Interval.TotalSeconds * mediaPlayer.SpeedRatio))
                {
                    sliderClass.retValueChanged = true;
                    sliderMusic.Value = sliderClass.sliderValueCalculate(mediaPlayer.Position.TotalSeconds,
                        mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds, sliderMusic.Maximum);
                    sliderClass.retValueChanged = false;
                }

                else
                {
                    mediaPlayer.Position = sliderClass.mediaPosCalculate(mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds,
                        sliderMusic.Value, sliderMusic.Maximum);
                }

                sliderMusic.SelectionEnd = sliderMusic.Value;
                LabelMusicPosition.Content = sliderClass.timeForLabel(mediaPlayer.Position);
            }
            catch (Exception ex) 
            { if (Settings.messagesActive) MessageBox.Show(ex.Message);sliderClass.refresh(); }
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
            int delInd;
            if (musicNum == 0)
                delInd = music.Count - 1;
            else delInd = musicNum - 1;
            ComboBoxTracks.Items.RemoveAt(delInd);
            music.RemoveAt(delInd);
            musicNames.RemoveAt(delInd);

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
