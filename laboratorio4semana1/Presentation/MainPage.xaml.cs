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

    // Variáveis do LibVLC
    private LibVLC _libVlc;
    private LibVLCSharp.Shared.MediaPlayer _vlcPlayer;
    
    // CORREÇÃO 1: Adicionado '?' para permitir nulo e evitar Warning CS8618
    private Media? _moveSoundMedia; 

    public MainPage()
    {
        this.InitializeComponent();

        // CORREÇÃO 2: Removido 'if (!Core.IsInitialized)' que causava o erro.
        // O Initialize já trata isso internamente na maioria dos casos.
        Core.Initialize(); 

        _libVlc = new LibVLC();
        _vlcPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVlc);

        // CORREÇÃO 3: Adicionado '_ =' para silenciar o Warning CS4014 (Fire-and-forget)
        _ = InitializeAudioAsync();

        // Configurações de Teclado e Foco
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
            // O LibVLC precisa do caminho físico do arquivo.
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/move_sound.mp3"));

            // Cria a mídia a partir do caminho do arquivo
            _moveSoundMedia = new Media(_libVlc, file.Path, FromType.FromPath);
            
            // Opcional: Pré-analisa a mídia para agilizar o primeiro play
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
        // Verifica se a mídia foi carregada antes de tentar tocar
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
