// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ICSharpCode.TextEditor;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for the main workbench window.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.ViewModel = new AppViewModel();
            this.InitializeComponent();

            this.GrammarEditor.SetHighlighting("Pegasus");
            this.GrammarEditor.Text = this.ViewModel.GrammarText;
            this.TestEditor.Text = this.ViewModel.TestText;

            var updatingGrammar = false;
            this.GrammarEditor.TextChanged += (s, e) =>
            {
                if (!updatingGrammar)
                {
                    updatingGrammar = true;
                    this.ViewModel.GrammarText = this.GrammarEditor.Text;
                    updatingGrammar = false;
                }
            };

            this.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "GrammarText" && !updatingGrammar)
                {
                    updatingGrammar = true;
                    Dispatcher.Invoke(() => { this.GrammarEditor.Text = this.ViewModel.GrammarText; });
                    updatingGrammar = false;
                }
            };

            var updatingTest = false;
            this.TestEditor.TextChanged += (s, e) =>
            {
                if (!updatingTest)
                {
                    updatingTest = true;
                    this.ViewModel.TestText = this.TestEditor.Text;
                    updatingTest = false;
                }
            };

            this.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "TestText" && !updatingTest)
                {
                    updatingTest = true;
                    Dispatcher.Invoke(() => { this.TestEditor.Text = this.ViewModel.TestText; });
                    updatingTest = false;
                }
            };
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public AppViewModel ViewModel { get; protected set; }

        /// <summary>
        /// Handles the DoubleClick event of the ErrorRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is standard for event handlers.")]
        public void ErrorRow_DoubleClick(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            var item = row.Item as CompilerError;

            this.FocusError(item);
        }

        /// <summary>
        /// Handles the KeyDown event of the ErrorRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is standard for event handlers.")]
        public void ErrorRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                e.Handled = true;
                this.ErrorRow_DoubleClick(sender, null);
            }
        }

        private void FocusError(CompilerError error)
        {
            TextEditorControl target;
            TabItem tab;
            if (error.FileName == this.ViewModel.GrammarFileName)
            {
                target = this.GrammarEditor;
                tab = this.GrammarTab;
            }
            else if (error.FileName == this.ViewModel.TestFileName)
            {
                target = this.TestEditor;
                tab = this.TestTab;
            }
            else
            {
                return;
            }

            tab.IsSelected = true;
            this.UpdateLayout();

            target.ActiveTextAreaControl.SelectionManager.ClearSelection();
            target.ActiveTextAreaControl.Caret.Position = new TextLocation(error.Column - 1, error.Line - 1);
            target.Focus();

            // BUG: There seems to be a bug in the framework regarding focus with WinForms interop.
            // Repro case:
            // 1. Run the app, switch to both the grammar and the test tabs, to make sure each TextEditor control is shown at least once.
            // 2. Edit until an error is shown.
            // 3. Switch to the tab that the error is NOT on.
            // 4. Double click the error. (Note that the focus did not transfer to the text editor.)
            // 5. Double click the error again. (Note that the focus does transfer.)
            // Note: This does not occur until both TextEditor controls have been shown at least once.
        }

        private void Open(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel.GrammarChanged)
            {
                switch (MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SaveChangesToFile, Path.GetFileName(this.ViewModel.GrammarFileName)), this.Title, MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Cancel:
                        return;

                    case MessageBoxResult.Yes:
                        this.ViewModel.Save.Execute(null);
                        break;
                }
            }

            var dialog = new OpenFileDialog
            {
                FileName = Path.GetFileName(this.ViewModel.GrammarFileName),
                DefaultExt = ".peg",
                Filter = "Pegasus Grammars (*.peg)|*.peg",
                ValidateNames = true,
                InitialDirectory = Path.GetDirectoryName(this.ViewModel.GrammarFileName),
            };

            if (dialog.ShowDialog() == true)
            {
                this.ViewModel.Load.Execute(dialog.FileName);
            }
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel.Save.CanExecute(null))
            {
                this.ViewModel.Save.Execute(null);
            }
            else
            {
                this.SaveAs(sender, e);
            }
        }

        private void SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = Path.GetFileName(this.ViewModel.GrammarFileName),
                AddExtension = true,
                DefaultExt = ".peg",
                Filter = "Pegasus Grammars (*.peg)|*.peg",
                ValidateNames = true,
                InitialDirectory = Path.GetDirectoryName(this.ViewModel.GrammarFileName),
            };

            if (dialog.ShowDialog() == true)
            {
                this.ViewModel.SaveAs.Execute(dialog.FileName);
            }
        }
    }
}
