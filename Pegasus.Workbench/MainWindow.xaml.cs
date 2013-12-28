// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
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

            var updating = false;
            this.TextEditor.TextChanged += (s, e) =>
            {
                if (!updating)
                {
                    updating = true;
                    this.ViewModel.Text = this.TextEditor.Text;
                    updating = false;
                }
            };

            this.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Text" && !updating)
                {
                    updating = true;
                    this.TextEditor.Text = this.ViewModel.Text;
                    updating = false;
                }
            };
        }

        public AppViewModel ViewModel { get; protected set; }
    }
}
