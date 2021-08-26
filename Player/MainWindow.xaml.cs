﻿using System;
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
            try
            {
                //DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + @"\music\basePlaylist");//перед сборкой удалить нижнюю строку и раскомментить эту
                DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\Users\pressF\Documents\Visual Studio 2017\Projects\Player\Player\music\basePlaylist");
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (musicExtension.Contains(file.Extension))
                    {
                        music.Add(file.FullName);
                        musicNames.Add(file.Name.Replace(file.Extension, ""));
                    }
                }

                if (music.Count > 0)
                {
                    musicNum = 0;

                    mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                    mediaPlayer.Open(new Uri(music[musicNum]));
                    mediaPlayer.Play();

                    ButtonMusicPlay.Visibility = Visibility.Visible;
                    ButtonMusicNext.Visibility = Visibility.Visible;
                    LabelMusic.Content = musicNames[musicNum];
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

                mediaPlayer.Open(new Uri(music[musicNum]));
                mediaPlayer.Play();

                LabelMusic.Content = musicNames[musicNum];
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message);
            }
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

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(fbd.SelectedPath);
                List<string> allFiles = new List<string>();

                if (!replace)
                {
                    //progWay = Environment.CurrentDirectory + @"\ways" + newPlaylistName + ".txt";
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

                    ComboBoxPlaylists.Items.Add(newPlaylistName);
                    ComboBoxPlaylists.SelectedIndex = ComboBoxPlaylists.Items.Count - 1;
                    playlistNames.Add(newPlaylistName);
                    playlists.Add(fileWay);
                }
                else { }//
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }



        private void ComboBoxPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
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