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
    using System.Windows;
    using System.Windows.Controls;
    using ICSharpCode.TextEditor;

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
        }
    }
}
