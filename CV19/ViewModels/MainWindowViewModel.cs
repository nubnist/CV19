using CV19.Infrastructure.Commands;
using CV19.Models;
using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CV19.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region TestDataPoint : IEnumerable<DataPoint> - Тестовое набор данных для графика

        /// <summary>Тестовое набор данных для графика</summary>
        private IEnumerable<DataPoint> _TestDataPoint;

        /// <summary>Тестовое набор данных для графика</summary>
        public IEnumerable<DataPoint> TestDataPoint { get => _TestDataPoint; set => Set(ref _TestDataPoint, value); }

        #endregion

        
        #region Title : string - Заголовок окна
        private string _Title;
        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion

        #region Status : string - Статус программы

        /// <summary>Статус программы</summary>
        private string _Status = "Готов";

        /// <summary>Статус программы</summary>
        public string Status { get => _Status; set => Set(ref _Status, value); }

        #endregion

        #region Команды

        #region CloseApplicationCommand

        #endregion
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommand(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion
        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);

            #endregion

            var data_points= new List<DataPoint>((int)(360 / 0.1));
            for (var x = 0d; x < 360; x += 0.1)
            {
                const double to_rad = Math.PI / 180;
                var y = Math.Sin(x * to_rad);

                data_points.Add(new DataPoint {XValue = x, YValue = y});
            }

            TestDataPoint = data_points;
        }
    }
}
