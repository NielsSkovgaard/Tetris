using System.Windows.Controls;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for HighScoreInputUserControl.xaml
    /// </summary>
    public partial class HighScoreInputUserControl : UserControl
    {
        public int Score { get; set; }

        public HighScoreInputUserControl()
        {
            InitializeComponent();
        }
    }
}
