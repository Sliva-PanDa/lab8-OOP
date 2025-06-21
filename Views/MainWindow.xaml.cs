using System.Windows;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using InvestigationApp.ViewModels;
using System.Linq;
using System;

namespace InvestigationApp.Views
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Инициализирует компоненты окна.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}