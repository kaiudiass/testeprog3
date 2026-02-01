using Microsoft.UI.Xaml.Input;
using Windows.Media.Core;

namespace laboratorio4semana1.Presentation;

public sealed partial class MainPage : Page
{
    private double _yPos = 100; // Posição vertical inicial
    private const double Step = 15; // Velocidade do movimento

    public MainPage()
    {
        this.InitializeComponent();
        
        // Garante que o Canvas receba o foco para detectar o teclado
        this.Loaded += (s, e) => GameCanvas.Focus(FocusState.Programmatic);
        
        // Assina o evento de tecla pressionada
        this.KeyDown += MainPage_KeyDown;

        // Carrega o som de movimento
        SFXPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/move_sound.mp3"));
    }

    private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        bool moved = false;

        // Verifica as setas para cima e para baixo
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
            // Aplica a nova posição ao sprite no Canvas
            Canvas.SetTop(PlayerSprite, _yPos);
            
            // Toca o som (reseta se já estiver tocando para repetir rápido)
            SFXPlayer.MediaPlayer.Stop();
            SFXPlayer.MediaPlayer.Play();
        }
    }
}
