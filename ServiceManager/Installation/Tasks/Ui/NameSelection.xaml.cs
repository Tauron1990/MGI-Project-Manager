﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceManager.Installation.Tasks.Ui
{
    /// <summary>
    /// Interaktionslogik für NameSelection.xaml
    /// </summary>
    public partial class NameSelection : UserControl
    {
        private readonly NameSelectionModel _model;

        public NameSelection(NameSelectionModel model)
        {
            InitializeComponent();

            _model = model;
            DataContext = model;
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e) => _model.Click();
    }
}
