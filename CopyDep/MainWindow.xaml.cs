using CopyDep.Models;
using CopyDep.Services;
using CopyDep.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CopyDep
{
    public partial class MainWindow : Window
    {
        private ICopyWorkerService _copyService;
        private ProjectsIn _projects;
        private DirInfoModel _infoModel;
        private Status _status;
        private Boolean _inited = false;


        public MainWindow(ICopyWorkerService copyService)
        {
            InitializeComponent();
            // services 
            _copyService = copyService;
            // init
            this.Init();
        }

        
        private void Init()
        {
            // mvvm
            _projects = (ProjectsIn)this.TryFindResource("projects");
            _infoModel = (DirInfoModel)this.TryFindResource("infoModel");
            _status = (Status)this.TryFindResource("status");
            // ok
            _inited = true;
            // autostart
            LoadConfigData();
        }



        private void LoadConfigData()
        {
            if (!_inited) return;
            Configurator.LoadConfig(_projects, _status);
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (!_inited) return;
            Configurator.SaveConfig(_projects, _status);
        }





        private void Button_ProjectAdd_Click(object sender, RoutedEventArgs e)
        {
            // TODO: подтверждение действия (кнопка (картинка ее) сменяется на вопрос на некоторое время - повторное нажатие и есть подтверждение)
            _projects.AddNewProject();
            Configurator.SaveConfig(_projects, _status);
        }

        private void Button_ProjectRemove_Click(object sender, RoutedEventArgs e)
        {
            // TODO: подтверждение действия (кнопка (картинка ее) сменяется на вопрос на некоторое время - повторное нажатие и есть подтверждение)
            _projects.RemoveProject(_projects.Current);
            Configurator.SaveConfig(_projects, _status);
        }

        private void ComboBox_Projects_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_inited) return;
            // сброс визуализации
            _infoModel.OnChangeProject();
            _status.OnChangeProject();
            // NOTE: жалко на каждом изменении выборки текущего проекта постоянно сохранять все настройки
            // но, очевидно запомнили бы выбор пользователя (без нажатия кнопки Сохранить)
            //Configurator.SaveConfig(_projects, _status);
        }





        private async void Button_Prepare(object sender, RoutedEventArgs e)
        {
            if (!_inited) return;
            await Task.Factory.StartNew(
                  () => _copyService.Prepare(this.Dispatcher, _projects.Current, _infoModel, _status),
                  TaskCreationOptions.LongRunning);
        }

        private async void Button_Go(object sender, RoutedEventArgs e)
        {
            if (!_inited) return;
            await Task.Factory.StartNew(
                  () => _copyService.CopyRun(this.Dispatcher, _infoModel, _status),
                  TaskCreationOptions.LongRunning);
        }






        private void OpenDirectory(String dir)
        {
            if (String.IsNullOrWhiteSpace(dir)) return;
            if (dir.EndsWith(Conventions.DirSubdirSymbol)) dir = dir.Remove(dir.Length - Conventions.DirSubdirSymbol.Length);
            System.Diagnostics.Process.Start("explorer", dir);
        }
        private void TextBox_MouseDoubleClick_DirFrom(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                OpenDirectory(_projects.Current.DirFrom);
            }
        }
        private void TextBox_MouseDoubleClick_DirTo(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                OpenDirectory(_projects.Current.DirTo);
            }
        }

        
    }
}
