using System.Windows.Controls;

namespace Tetris.UI
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
