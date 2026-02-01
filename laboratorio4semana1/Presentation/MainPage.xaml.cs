using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using LibVLCSharp.Shared; // O namespace do VLC

namespace laboratorio4semana1.Presentation;

public sealed partial class MainPage : Page
{
    private double _yPos = 0;
    private const double Step = 20;
    
    private LibVLC _libVlc;
    private LibVLCSharp.Shared.MediaPlayer _vlcPlayer;
    
    
    private Media? _moveSoundMedia; 

    public MainPage()
    {
        this.InitializeComponent();

       
        Core.Initialize(); 

        _libVlc = new LibVLC();
        _vlcPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVlc);

       
        _ = InitializeAudioAsync();

  
        this.KeyDown += MainPage_KeyDown;
        this.Loaded += (s, e) =>
        {
            this.Focus(FocusState.Programmatic);
            Canvas.SetTop(PlayerSprite, _yPos);
        };
    }

    private async Task InitializeAudioAsync()
    {
        try
        {
        
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/move_sound.mp3"));

           
            _moveSoundMedia = new Media(_libVlc, file.Path, FromType.FromPath);
            
           
            _moveSoundMedia.Parse(MediaParseOptions.ParseLocal);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar áudio VLC: {ex.Message}");
        }
    }

    private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        bool moved = false;

        if (e.Key == Windows.System.VirtualKey.Up)
        {
            _yPos -= Step;
            moved = true;
        }
        else if (e.Key == Windows.System.VirtualKey.Down)
        {
            _yPos += Step;
            moved = true;
        }

        if (moved)
        {
            Canvas.SetTop(PlayerSprite, _yPos);
            PlaySound();
        }
    }

    private void PlaySound()
    {
        
        if (_vlcPlayer != null && _moveSoundMedia != null)
        {
            if (_vlcPlayer.IsPlaying)
            {
                _vlcPlayer.Stop();
            }

            _vlcPlayer.Media = _moveSoundMedia;
            _vlcPlayer.Play();
        }
    }
}
