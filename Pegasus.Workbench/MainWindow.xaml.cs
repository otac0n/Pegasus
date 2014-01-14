// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ICSharpCode.TextEditor;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.ViewModel = new AppViewModel();
            InitializeComponent();

            this.GrammarEditor.SetHighlighting("Pegasus");
            this.GrammarEditor.Text = ViewModel.GrammarText;
            this.TestEditor.Text = ViewModel.TestText;

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
                if (e.PropertyName == "Text" && !updatingGrammar)
                {
                    updatingGrammar = true;
                    this.GrammarEditor.Text = this.ViewModel.GrammarText;
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
                if (e.PropertyName == "Text" && !updatingTest)
                {
                    updatingTest = true;
                    this.TestEditor.Text = this.ViewModel.TestText;
                    updatingTest = false;
                }
            };
        }

        public AppViewModel ViewModel { get; protected set; }

        public void ErrorRow_DoubleClick(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            var item = row.Item as CompilerError;

            FocusError(item);
        }

        public void ErrorRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
            UpdateLayout();

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

        private void SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = Path.GetFileName(this.ViewModel.GrammarFileName),
                AddExtension = true,
                DefaultExt = ".peg",
                Filter = "Pegasus Grammars (*.peg)|*.peg",
                ValidateNames = true,
                InitialDirectory = Path.GetDirectoryName(this.ViewModel.GrammarFileName),
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.ViewModel.SaveAs.Execute(dialog.FileName);
            }
        }
    }
}
