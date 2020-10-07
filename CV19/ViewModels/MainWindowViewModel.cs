using CV19.Infrastructure.Commands;
using CV19.Models;
using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CV19.Models.Decanat;

namespace CV19.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        /*-------------------------------------------------------------------------------------------------------------------------------*/


        public ObservableCollection<Group> Groups { get; set; }
        

        #region SelectedPageIndex : int - Индекс выбранной вкладки

        /// <summary>Индекс выбранной вкладки</summary>
        private int _SelectedPageIndex;

        /// <summary>Индекс выбранной вкладки</summary>
        public int SelectedPageIndex { get => _SelectedPageIndex; set => Set(ref _SelectedPageIndex, value); }

        #endregion

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

        /*-------------------------------------------------------------------------------------------------------------------------------*/

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommand(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion


        public ICommand ChangeTabIndex { get; set; }

        private bool CanChangeTabIndexCommandExecute(object p) => SelectedPageIndex >= 0;

        private void OnChangeTabIndexCommandExecuted(object p)
        {
            if (p is null) return;
            SelectedPageIndex += Convert.ToInt32(p);
        }

        #endregion

        /*-------------------------------------------------------------------------------------------------------------------------------*/

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);
            ChangeTabIndex = new LambdaCommand(OnChangeTabIndexCommandExecuted, CanChangeTabIndexCommandExecute);

            #endregion

            var data_points= new List<DataPoint>((int)(360 / 0.1));
            for (var x = 0d; x < 360; x += 0.1)
            {
                const double to_rad = Math.PI / 180;
                var y = Math.Sin(x * to_rad);

                data_points.Add(new DataPoint {XValue = x, YValue = y});
            }

            TestDataPoint = data_points;

            var student_index = 1;
            var students = Enumerable.Range(1, 10).Select(i => new Student()
            {
                Name = $"Name {student_index}",
                Surname = $"Surname {student_index}",
                Patronymic = $"Patronymic {student_index++}",
                Birthday = DateTime.Now,
                Rating = 0
            });

            var groups = new ObservableCollection<Group>(Enumerable.Range(1, 20).Select(i => new Group()
            {
                Name = $"Группа {i}",
                Students = new ObservableCollection<Student>(students)
            }));

            Groups = new ObservableCollection<Group>(groups);
        }

        /*-------------------------------------------------------------------------------------------------------------------------------*/
    }
}
