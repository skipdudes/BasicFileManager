using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BasicFileManager
{
    /// <summary>
    /// Interaction logic for CreateDialog.xaml
    /// </summary>
    public partial class CreateDialog : Window
    {
        string path;
        string name;
        bool success;

        public CreateDialog(string path)
        {
            InitializeComponent();
            this.path = path;
            name = "";
            success = false;
        }

        private void createDialogButtonOk_Click(object sender, RoutedEventArgs e)
        {
            //RadioButtons
            bool selectedFile = (bool)createDialogRadioFile.IsChecked;
            bool selectedDirectory = (bool)createDialogRadioDirectory.IsChecked;

            //Regex (tylko dla plików)
            string pattern = "^[a-zA-Z0-9_~-]{1,8}\\.(txt|php|html)$";
            if (selectedFile && !(Regex.IsMatch(createDialogName.Text, pattern)))
                System.Windows.MessageBox.Show("Wrong name!", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                //Ścieżka
                name = createDialogName.Text;
                path += "/" + name;

                //Utworzenie
                if (selectedFile)
                    File.Create(path).Close(); //Zamknięcie po utworzeniu
                else if (selectedDirectory)
                    Directory.CreateDirectory(path);

                //RAHS
                FileAttributes attributes = FileAttributes.Normal;
                if ((bool)createDialogCheckBoxR.IsChecked)
                    attributes |= FileAttributes.ReadOnly;
                if ((bool)createDialogCheckBoxA.IsChecked)
                    attributes |= FileAttributes.Archive;
                if ((bool)createDialogCheckBoxH.IsChecked)
                    attributes |= FileAttributes.Hidden;
                if ((bool)createDialogCheckBoxS.IsChecked)
                    attributes |= FileAttributes.System;
                File.SetAttributes(path, attributes);

                success = true;
                Close();
            }
        }

        private void createDialogButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool Success()
        {
            return success;
        }

        public string Path()
        {
            return path;
        }
    }
}
