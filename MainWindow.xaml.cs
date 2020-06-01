using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Linq;

namespace Wereldvlaggen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly DirectoryInfo FLAGS_DIR = new DirectoryInfo("flags");

        private List<Flag> flags;

        private List<Flag> practiceFlags;
        private Flag practiceFlag;
        private Random random;
        private int correct, incorrect;

        public MainWindow()
        {
            InitializeComponent();
            flags = new List<Flag>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Maak een List van alle vlaggen en toon een Image met die vlag
            // en een Label met de naam van dat land.
            FileInfo[] flagFiles = FLAGS_DIR.GetFiles();
            foreach (FileInfo flagFile in flagFiles)
            {
                Flag flag = new Flag(flagFile);
                flags.Add(flag);

                StackPanel stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(10);

                Image image = new Image();
                image.Source = flag.Image;
                image.Width = 100;
                image.Height = 75;
                stackPanel.Children.Add(image);

                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Content = flag.CountryName;
                stackPanel.Children.Add(label);

                wrapPanel.Children.Add(stackPanel);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (scrollViewer.IsVisible)
            {
                scrollViewer.Visibility = Visibility.Collapsed;
                stackPanel.Visibility = Visibility.Visible;
                label.Content = "";
                button.Content = "Stop";

                // Maak een copy van de flags list zodat we tijdens het oefenen elementen uit die
                // kopie kunnen verwijderen en dat de oorspronkelijke lijst hetzelfde blijft,
                // mocht de gebruiker gaan stoppen met oefenen en daarna weer opnieuw beginnen.
                practiceFlags = flags.ToList();
                random = new Random();
                correct = incorrect = 0;
                Practice();
            }
            else
            {
                StopPractice();
            }
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bNext_Click(sender, e);
            }
        }

        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.Length == 0)
            {
                // Niks invullen betekent "Ik weet het niet".
                label.Content = "Dat was: " + practiceFlag.CountryName;
                label.Foreground = Brushes.Black;
                incorrect++;
            }
            else if (textBox.Text.ToLower() == practiceFlag.CountryName.ToLower())
            {
                correct++;
                label.Content = "Goed!";
                label.Foreground = Brushes.Green;
            }
            else
            {
                incorrect++;
                label.Content = "Fout! Het juiste antwoord was: " + practiceFlag.CountryName;
                label.Foreground = Brushes.Red;
            }

            string result =
                "Aantal goed: " + correct + Environment.NewLine +
                "Aantal fout: " + incorrect;
            label.Content += Environment.NewLine + result;

            if (practiceFlags.Count > 0)
            {
                Practice();
            }
            else
            {
                StopPractice();
                MessageBox.Show(result, "Resultaat",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Practice()
        {
            practiceFlag = practiceFlags[random.Next(0, practiceFlags.Count)];
            practiceFlags.Remove(practiceFlag);

            image.Source = practiceFlag.Image;
            textBox.Text = "";
            textBox.Focus();
        }

        private void StopPractice()
        {
            scrollViewer.Visibility = Visibility.Visible;
            stackPanel.Visibility = Visibility.Collapsed;
            button.Content = "Oefenen";
        }

        private class Flag
        {
            private ImageSource image;
            private String countryName;

            public Flag(FileInfo fileInfo)
            {
                image = new BitmapImage(new Uri(fileInfo.FullName));
                countryName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            }

            public ImageSource Image
            {
                get { return image; }
            }

            public String CountryName
            {
                get { return countryName; }
            }
        }
    }
}
