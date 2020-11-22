using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CopyDep.Models
{
    /// <summary>
    /// Проекты.
    /// </summary>
    public class ProjectsIn : INotifyPropertyChanged
    {
        private ObservableCollection<ProjectItemIn> _items = new ObservableCollection<ProjectItemIn>();
        public ObservableCollection<ProjectItemIn> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        private ProjectItemIn _current;
        public ProjectItemIn Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged("Current");
            }
            //? так не подхидит в MVVM
            //get 
            //{
            //    if (_items == null || !_items.Any()) return new ProjectItemIn();
            //    var current = _items.FirstOrDefault(f => f.IsCurrent);
            //    return current ?? _items.FirstOrDefault();
            //}
        }



        public ProjectsIn()
        {
            AddNewProject();
        }

        public void AddNewProject()
        {
            var item = new ProjectItemIn() { Id = Guid.NewGuid(), Title = "Test site", DirFrom = @"c:\test\dir1\*", DirFromIgnore = @"c:\test\dir1\src\*", DirTo = @"c:\test\dir2" };
            Items.Add(item);
            Current = item;
        }


        public void RemoveProject(ProjectItemIn forRemove)
        {
            if (forRemove == null) return;
            Items.Remove(forRemove);
            Current = _items.FirstOrDefault();
        }






        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
