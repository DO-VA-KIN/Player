#pragma checksum "..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CD31576EBFF2454D9573A6D9022495EC0C866936"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Player;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Player
{


    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

#line default
#line hidden


#line 12 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Menu_Close;

#line default
#line hidden


#line 14 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Menu_Settings;

#line default
#line hidden


#line 17 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LabelMusic;

#line default
#line hidden


#line 18 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonMusicPlay;

#line default
#line hidden


#line 26 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonMusicNext;

#line default
#line hidden


#line 35 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBoxPlaylists;

#line default
#line hidden


#line 36 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBoxTracks;

#line default
#line hidden


#line 38 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDeletePlaylist;

#line default
#line hidden


#line 46 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDeleteTrack;

#line default
#line hidden


#line 54 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonNewPlaylist;

#line default
#line hidden


#line 62 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonAddTrack;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Player;component/mainwindow.xaml", System.UriKind.Relative);

#line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.Window1 = ((Player.MainWindow)(target));

#line 8 "..\..\MainWindow.xaml"
                    this.Window1.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);

#line default
#line hidden

#line 8 "..\..\MainWindow.xaml"
                    this.Window1.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);

#line default
#line hidden
                    return;
                case 2:
                    this.Menu_Close = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 3:
                    this.Menu_Settings = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 4:
                    this.LabelMusic = ((System.Windows.Controls.Label)(target));
                    return;
                case 5:
                    this.ButtonMusicPlay = ((System.Windows.Controls.Button)(target));

#line 18 "..\..\MainWindow.xaml"
                    this.ButtonMusicPlay.Click += new System.Windows.RoutedEventHandler(this.ButtonMusicPlay_Click);

#line default
#line hidden
                    return;
                case 6:
                    this.ButtonMusicNext = ((System.Windows.Controls.Button)(target));

#line 26 "..\..\MainWindow.xaml"
                    this.ButtonMusicNext.Click += new System.Windows.RoutedEventHandler(this.MediaPlayer_MediaEnded);

#line default
#line hidden
                    return;
                case 7:
                    this.ComboBoxPlaylists = ((System.Windows.Controls.ComboBox)(target));

#line 35 "..\..\MainWindow.xaml"
                    this.ComboBoxPlaylists.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ComboBoxPlaylists_SelectionChanged);

#line default
#line hidden
                    return;
                case 8:
                    this.ComboBoxTracks = ((System.Windows.Controls.ComboBox)(target));

#line 36 "..\..\MainWindow.xaml"
                    this.ComboBoxTracks.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ComboBoxTracks_SelectionChanged);

#line default
#line hidden
                    return;
                case 9:
                    this.btnDeletePlaylist = ((System.Windows.Controls.Button)(target));

#line 38 "..\..\MainWindow.xaml"
                    this.btnDeletePlaylist.Click += new System.Windows.RoutedEventHandler(this.BtnDeletePlaylist_Click);

#line default
#line hidden
                    return;
                case 10:
                    this.btnDeleteTrack = ((System.Windows.Controls.Button)(target));

#line 46 "..\..\MainWindow.xaml"
                    this.btnDeleteTrack.Click += new System.Windows.RoutedEventHandler(this.BtnDeleteTrack_Click);

#line default
#line hidden
                    return;
                case 11:
                    this.ButtonNewPlaylist = ((System.Windows.Controls.Button)(target));

#line 54 "..\..\MainWindow.xaml"
                    this.ButtonNewPlaylist.Click += new System.Windows.RoutedEventHandler(this.ButtonNewPlaylist_Click);

#line default
#line hidden
                    return;
                case 12:
                    this.ButtonAddTrack = ((System.Windows.Controls.Button)(target));

#line 62 "..\..\MainWindow.xaml"
                    this.ButtonAddTrack.Click += new System.Windows.RoutedEventHandler(this.ButtonNewPlaylist_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Window Window1;
    }
}

