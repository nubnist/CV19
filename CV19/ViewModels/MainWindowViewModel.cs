using CV19.Infrastructure.Commands;
using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CV19.Models;
using CV19.Models.Decanat;


namespace CV19.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        /*-------------------------------------------------------------------------------------------------------------------------------*/

        public object[] CompositeCollection { get; }
        
        public ObservableCollection<Group> Groups { get; }

        #region SelectedCompositeValue : object - Выбранный непонятный элемент

        /// <summary>Выбранный непонятный элемент</summary>
        private object _SelectedCompositeValue;

        /// <summary>Выбранный непонятный элемент</summary>
        public object SelectedCompositeValue { get => _SelectedCompositeValue; set => Set(ref _SelectedCompositeValue, value); }

        #endregion

        #region SelectedGroup : Group - Выбранная группа

        /// <summary>Выбранная группа</summary>
        private Group _SelectedGroup;

        /// <summary>Выбранная группа</summary>
        public Group SelectedGroup 
        { 
            get => _SelectedGroup;
            set
            {
                if (!Set(ref _SelectedGroup, value)) return;
                _SelectedGroupStudents.Source = value?.Students;
                _SelectedGroupStudents.View.Refresh();
                OnPropertyChanged(nameof(SelectedGroupStudents));
            }
        }

        #endregion

        #region StudentFilterText : string - Текст фильтров студентов

        /// <summary>Текст фильтров студентов</summary>
        private string _StudentFilterText;

        /// <summary>Текст фильтров студентов</summary>
        public string StudentFilterText
        {
            get => _StudentFilterText;
            set
            {
                if (!Set(ref _StudentFilterText, value)) return; 
                _SelectedGroupStudents.View.Refresh();
            }
        }

        #endregion

        #region SelectedGroupStudents

        private readonly CollectionViewSource _SelectedGroupStudents = new CollectionViewSource();

        private void OnStudentFiltred(object Sender, FilterEventArgs E)
        {
            if(!(E.Item is Student student)) return;
            var filter_text = _StudentFilterText;
            if(string.IsNullOrEmpty(filter_text)) return;
            if (student.Name is null || student.Surname is null || student.Patronymic is null) return;

            if(student.Name.Contains(filter_text, StringComparison.OrdinalIgnoreCase)) return;
            if (student.Surname.Contains(filter_text, StringComparison.OrdinalIgnoreCase)) return;
            if (student.Patronymic.Contains(filter_text, StringComparison.OrdinalIgnoreCase)) return;

            E.Accepted = false;
        }

        public ICollectionView SelectedGroupStudents => _SelectedGroupStudents?.View;

        #endregion

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

        #region ChangeTabIndex
        public ICommand ChangeTabIndex { get; set; }

        private bool CanChangeTabIndexCommandExecute(object p) => SelectedPageIndex >= 0;

        private void OnChangeTabIndexCommandExecuted(object p)
        {
            if (p is null) return;
            SelectedPageIndex += Convert.ToInt32(p);
        }

        #endregion

        #region CreateNewGroupComand

        public ICommand CreateNewGroupComand { get; }

        private bool CanCreateNewGroupComandExecute(object p) => true;

        private void OnCreateNewGroupComandExecuted(object p)
        {
            var group_max_index = Groups.Count + 1;
            var new_group = new Group()
            {
                Name = $"Группа {group_max_index}",
                Students = new ObservableCollection<Student>()
            };

            Groups.Add(new_group);
            SelectedGroup = new_group;
        }

        #endregion

        #region DeleteGroupCommand

        public ICommand DeleteGroupCommand { get; }
        private bool CanDeleteGroupCommandComandExecute(object p) => p is Group group && Groups.Contains(group);

        private void OnDeleteGroupComandExecuted(object p)
        {
            if (!(p is Group group)) return;
            int index = Groups.IndexOf(SelectedGroup);
            Groups.Remove(group);
            if (Groups.Count != 0)
                SelectedGroup = index - 1 >= 0 ? Groups[index - 1] : Groups[index];
        }

        #endregion

        #endregion

        /*-------------------------------------------------------------------------------------------------------------------------------*/

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);
            ChangeTabIndex = new LambdaCommand(OnChangeTabIndexCommandExecuted, CanChangeTabIndexCommandExecute);
            CreateNewGroupComand = new LambdaCommand(OnCreateNewGroupComandExecuted, CanCreateNewGroupComandExecute);
            DeleteGroupCommand = new LambdaCommand(OnDeleteGroupComandExecuted, CanDeleteGroupCommandComandExecute);

            #endregion

            #region Тестовый регион

            var data_points = new List<DataPoint>((int)(360 / 0.1));
            for (var x = 0d; x < 360; x += 0.1)
            {
                const double to_rad = Math.PI / 180;
                var y = Math.Sin(x * to_rad);

                data_points.Add(new DataPoint { XValue = x, YValue = y });
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

            var data_list = new List<object> {"Hello world", 42};

            var group = Groups[1];
            data_list.Add(group);
            data_list.Add(group.Students[0]);

            CompositeCollection = data_list.ToArray();

            #endregion

            _SelectedGroupStudents.Filter += OnStudentFiltred;
        }

        /*-------------------------------------------------------------------------------------------------------------------------------*/
    }
}
