using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CopyDep.Models
{
    /// <summary>
    /// Конфигурация проекта (сайта) - входящие параметры.
    /// </summary>
    public class ProjectItemIn : INotifyPropertyChanged
    {
        public Guid Id;


        private String _title;
        public String Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private String _dirFrom;
        public String DirFrom
        {
            get { return _dirFrom; }
            set
            {
                _dirFrom = value;
                OnPropertyChanged("DirFrom");
            }
        }
        private String _dirFromIgnore;
        public String DirFromIgnore
        {
            get { return _dirFromIgnore; }
            set
            {
                _dirFromIgnore = value;
                OnPropertyChanged("DirFromIgnore");
            }
        }
        private String _dirTo;
        public String DirTo
        {
            get { return _dirTo; }
            set
            {
                _dirTo = value;
                OnPropertyChanged("DirTo");
            }
        }

        private ProjectItemInOptions _options;
        public ProjectItemInOptions Options
        {
            get 
            {
                //- кто-бы когда не обнулил, мы не хотим излишние проверки на null по коду
                if (_options == null) _options = new ProjectItemInOptions();
                return _options; 
            }
            set
            {
                _options = value;
                OnPropertyChanged("Options");
            }
        }


        public ProjectItemIn()
        {
            Options = new ProjectItemInOptions();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }


    /// <summary>
    /// Параметры алгоритма поиска необходимых файлов.
    /// </summary>
    public class ProjectItemInOptions : INotifyPropertyChanged
    {
        private Boolean _byCreateTime;
        public Boolean ByCreateTime
        {
            get { return _byCreateTime; }
            set
            {
                _byCreateTime = value;
                OnPropertyChanged("ByCreateTime");
            }
        }
        private Boolean _byLastWriteTime;
        public Boolean ByLastWriteTime
        {
            get { return _byLastWriteTime; }
            set
            {
                _byLastWriteTime = value;
                OnPropertyChanged("ByLastWriteTime");
            }
        }
        private Boolean _byLength;
        public Boolean ByLength
        {
            get { return _byLength; }
            set
            {
                _byLength = value;
                OnPropertyChanged("ByLength");
            }
        }
        private Boolean _byContent;
        public Boolean ByContent
        {
            get { return _byContent; }
            set
            {
                _byContent = value;
                OnPropertyChanged("ByContent");
            }
        }

        //private Boolean _accessReadWrite;
        //public Boolean AccessReadWrite
        //{
        //    get { return _accessReadWrite; }
        //    set
        //    {
        //        _accessReadWrite = value;
        //        OnPropertyChanged("AccessReadWrite");
        //    }
        //}




        public ProjectItemInOptions()
        {
            ByCreateTime = false;
            ByLastWriteTime = true;
            ByLength = true;
            ByContent = false;
            //AccessReadWrite = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }


}
