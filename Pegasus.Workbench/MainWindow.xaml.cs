// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.Windows;

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

            var updating = false;
            this.GrammarEditor.TextChanged += (s, e) =>
            {
                if (!updating)
                {
                    updating = true;
                    this.ViewModel.Text = this.GrammarEditor.Text;
                    updating = false;
                }
            };

            this.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Text" && !updating)
                {
                    updating = true;
                    this.GrammarEditor.Text = this.ViewModel.Text;
                    updating = false;
                }
            };

            this.ViewModel.Text = "start" + Environment.NewLine + "  = \"Hello, world!\" EOF" + Environment.NewLine + "EOF" + Environment.NewLine + "  = !.";
        }

        public AppViewModel ViewModel { get; protected set; }
    }
}
