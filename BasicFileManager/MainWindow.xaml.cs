using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BasicFileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                treeView.Items.Clear();

                //Główny katalog
                DirectoryInfo dir = new DirectoryInfo(dlg.SelectedPath);
                var root = CreateTreeViewDirectory(dir);
                treeView.Items.Add(root);
            }
        }

        private TreeViewItem CreateTreeViewFile(FileInfo file)
        {
            var item = new TreeViewItem
            {
                Header = file.Name,
                Tag = file.FullName
            };

            //Przyciski Open i Delete
            item.ContextMenu = new ContextMenu();

            var menuItemOpen = new MenuItem { Header = "Open" };
            menuItemOpen.Click += new RoutedEventHandler(TreeViewOpenClick);

            var menuItemDelete = new MenuItem { Header = "Delete" };
            menuItemDelete.Click += new RoutedEventHandler(TreeViewDeleteClick);

            item.ContextMenu.Items.Add(menuItemOpen);
            item.ContextMenu.Items.Add(menuItemDelete);

            //RASH
            item.Selected += new RoutedEventHandler(selectedItemRAHS);

            return item;
        }

        private TreeViewItem CreateTreeViewDirectory(DirectoryInfo dir)
        {
            var root = new TreeViewItem
            {
                Header = dir.Name,
                Tag = dir.FullName
            };

            //Foldery
            foreach (DirectoryInfo subdir in dir.GetDirectories())
                root.Items.Add(CreateTreeViewDirectory(subdir));

            //Pliki
            foreach (FileInfo file in dir.GetFiles())
                root.Items.Add(CreateTreeViewFile(file));

            //Przyciski Create i Delete
            root.ContextMenu = new ContextMenu();

            var menuItemCreate = new MenuItem { Header = "Create" };
            menuItemCreate.Click += new RoutedEventHandler(TreeViewCreateClick);

            var menuItemDelete = new MenuItem { Header = "Delete" };
            menuItemDelete.Click += new RoutedEventHandler(TreeViewDeleteClick);

            root.ContextMenu.Items.Add(menuItemCreate);
            root.ContextMenu.Items.Add(menuItemDelete);

            //RASH
            root.Selected += new RoutedEventHandler(selectedItemRAHS);

            return root;
        }

        private void TreeViewOpenClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;

            //Pobierz zawartość pliku
            string fileText = File.ReadAllText((string)item.Tag);
            scrollViewer.Content = new TextBlock()
            {
                Text = fileText
            };
        }

        private void TreeViewDeleteClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            string path = (string)item.Tag;

            //Usunięcie z dysku
            FileAttributes attributes = File.GetAttributes(path);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                directoryDelete(path);
            else
                fileDelete(path);

            //Usunięcie z TreeView
            if ((TreeViewItem)treeView.Items[0] != item)
            {
                //Jeśli element nie jest korzeniem, pobierz rodzica
                TreeViewItem parent = (TreeViewItem)item.Parent;
                parent.Items.Remove(item);
            }
            else
                treeView.Items.Clear();
        }

        private void fileDelete(string path)
        {
            //Usunięcie ReadOnly
            FileAttributes attributes = File.GetAttributes(path);
            attributes &= ~FileAttributes.ReadOnly;
            File.SetAttributes(path, attributes);

            File.Delete(path);
        }

        private void directoryDelete(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            //Foldery
            foreach (var subdir in dir.GetDirectories())
                directoryDelete(subdir.FullName);

            //Pliki
            foreach (var file in dir.GetFiles())
                fileDelete(file.FullName);

            Directory.Delete(path);
        }

        private void TreeViewCreateClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem dir = (TreeViewItem)treeView.SelectedItem;
            string path = (string)dir.Tag;

            //Otwórz nowe okno
            CreateDialog dialog = new CreateDialog(path);
            dialog.ShowDialog();

            if (dialog.Success())
            {
                string newPath = dialog.Path();

                //Dodanie do TreeView
                if (File.Exists(newPath))
                {
                    FileInfo file = new FileInfo(newPath);
                    dir.Items.Add(CreateTreeViewFile(file));
                }
                else if (Directory.Exists(newPath))
                {
                    DirectoryInfo subdir = new DirectoryInfo(newPath);
                    dir.Items.Add(CreateTreeViewDirectory(subdir));
                }
            }


        }

        private void selectedItemRAHS(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            FileAttributes attributes = File.GetAttributes((string)item.Tag);
            statusBarText.Text = "";

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                statusBarText.Text += 'r';
            else
                statusBarText.Text += '-';

            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                statusBarText.Text += 'a';
            else
                statusBarText.Text += '-';

            if ((attributes & FileAttributes.System) == FileAttributes.System)
                statusBarText.Text += 's';
            else
                statusBarText.Text += '-';

            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                statusBarText.Text += 'h';
            else
                statusBarText.Text += '-';
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}