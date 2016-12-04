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

        private void TextBoxInitials_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonOk.IsEnabled = TextBoxInitials.Text.Length != 0;
        }
    }
}
