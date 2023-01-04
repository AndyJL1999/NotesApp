using Firebase.Storage;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NotesApp.MVVM.ViewModel;
using NotesApp.MVVM.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Net;
using System.Net.Http;
using System.Windows.Media.Animation;
using System.IO.Pipes;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NotesApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        int animPlayed = 0;
        NotesVM viewModel;

        public NotesWindow()
        {
            InitializeComponent();

            viewModel = Resources["vm"] as NotesVM;

            viewModel.SelectedNotebookChanged += ViewModel_SelectedNotebookChanged;

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontFamilyComboBox.ItemsSource = fontFamilies;

            List<double> fontSizes = new List<double>
            {
                8, 9, 10, 11, 12, 14, 16, 28, 32, 48
            };
            fontSizeComboBox.ItemsSource = fontSizes;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if(string.IsNullOrEmpty(App.UserId))
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                viewModel.GetNotebooks();
            }
        }

        private void ViewModel_SelectedNotebookChanged(object? sender, EventArgs e)
        {
            if (viewModel.SelectedNotebook != null)
            {
                Storyboard sb = this.FindResource("NotePanelSlide") as Storyboard;
                if(animPlayed == 0)
                {
                    sb.Begin();
                    animPlayed++;
                }  
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void contentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountOfCharacters = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd).Text.Length;

            statusTextBlock.Text = $"Document length: {amountOfCharacters} characters";
        }

        //Bolden or unbolden selected text when clicked
        private void boldButton_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as ToggleButton).IsChecked ?? false;

            if(isChecked)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            else
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
        }

        //Italicize or remove italicized selected text when clicked
        private void italicButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonEnabled)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            else
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
        }

        //Underline or remove underline selected text when clicked
        private void underlineButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonEnabled)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            else
            {
                TextDecorationCollection textDecorations;
                (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void contentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = contentRichTextBox.Selection.GetPropertyValue(FontWeightProperty);
            boldButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && selectedWeight.Equals(FontWeights.Bold);

            var selectedStyle = contentRichTextBox.Selection.GetPropertyValue(FontStyleProperty);
            italicButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && selectedStyle.Equals(FontStyles.Italic);

            var selectedDecoration = contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && selectedDecoration.Equals(TextDecorations.Underline);

            fontFamilyComboBox.SelectedItem = contentRichTextBox.Selection.GetPropertyValue(FontFamilyProperty);
            fontSizeComboBox.Text = (contentRichTextBox.Selection.GetPropertyValue(FontSizeProperty)).ToString();
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(fontFamilyComboBox.SelectedItem != null)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
        }

        private void fontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (fontSizeComboBox.Text != string.Empty)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);
        }

        private async void speechButton_Click(object sender, RoutedEventArgs e)
        {
            //NO SUBSCRIPTION YET! WILL NOT WORK!
            string region = "eastus";
            string key = "";

            try
            {
                var speechConfig = SpeechConfig.FromSubscription(key, region);
                using (var audioConfig = AudioConfig.FromDefaultMicrophoneInput())
                {
                    using (var recongnizer = new SpeechRecognizer(speechConfig, audioConfig))
                    {
                        var result = await recongnizer.RecognizeOnceAsync();
                        contentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(result.Text)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Subscription Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
