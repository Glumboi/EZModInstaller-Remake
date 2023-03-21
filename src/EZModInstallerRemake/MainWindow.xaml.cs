using EZModInstallerRemake.ViewModels;
using Wpf.Ui.Controls;

namespace EZModInstallerRemake
{
    public partial class MainWindow : UiWindow
    {
        private readonly MainViewModel viewModel = new MainViewModel();

        public MainWindow()
        {
            DataContext = viewModel;
            InitializeComponent();

            viewModel.SnackBar = Notification_SnackBar;
        }

        private void UiWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.CheckForRequirements();
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void SelectPCBS_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.OpenPCBS();
        }

        private void InstallUnitPatcher_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.InstallUnityPatcher();
        }

        private void InstallMods_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.MergeAndExecuteModInstallers();
        }

        private void FixSaves_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.ResetSaves();
        }

        private void RestoreSaves_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.RestoreSaves();
        }

        private void PCBSPath_TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            viewModel.PCBSPath = PCBSPath_TextBox.Text;
        }
    }
}