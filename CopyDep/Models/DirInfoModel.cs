using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CopyDep.Models
{
    /// <summary>
    /// Подготовленная модель данных для копирования (дерево файлов).
    /// </summary>
    public class DirInfoModel : INotifyPropertyChanged
    {
        private Boolean _canPrepare;
        public Boolean CanPrepare
        {
            get { return _canPrepare; }
            set
            {
                _canPrepare = value;
                OnPropertyChanged("CanPrepare");
            }
        }


        private Boolean _canWork;
        public Boolean CanWork
        {
            get { return _canWork; }
            set
            {
                _canWork = value;
                OnPropertyChanged("CanWork");
            }
        }


        private Int32 _totalFilesFrom;
        public Int32 TotalFilesFrom
        {
            get { return _totalFilesFrom; }
            set
            {
                _totalFilesFrom = value;
                OnPropertyChanged("TotalFilesFrom");
            }
        }


        private Int32 _newFilesFrom;
        public Int32 NewFilesFrom
        {
            get { return _newFilesFrom; }
            set
            {
                _newFilesFrom = value;
                OnPropertyChanged("NewFilesFrom");
            }
        }


        private Int32 _replaceFilesFrom;
        public Int32 ReplaceFilesFrom
        {
            get { return _replaceFilesFrom; }
            set
            {
                _replaceFilesFrom = value;
                OnPropertyChanged("ReplaceFilesFrom");
            }
        }

        private Int32 _errorsFilesFrom;
        public Int32 ErrorsFilesFrom
        {
            get { return _errorsFilesFrom; }
            set
            {
                _errorsFilesFrom = value;
                OnPropertyChanged("ErrorsFilesFrom");
            }
        }

        private ObservableCollection<String> _errors = new ObservableCollection<String>();
        public ObservableCollection<String> Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
                OnPropertyChanged("Errors");
            }
        }


        private ObservableCollection<DirTreeViewItemModel> _items = new ObservableCollection<DirTreeViewItemModel>();
        public ObservableCollection<DirTreeViewItemModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }



        private Boolean _isCopyWithErrorAfterPrepare;
        public Boolean IsCopyWithErrorAfterPrepare
        {
            get { return _isCopyWithErrorAfterPrepare; }
            set
            {
                _isCopyWithErrorAfterPrepare = value;
                OnPropertyChanged("IsCopyWithErrorAfterPrepare");
            }
        }






        public DirInfoModel()
        {
            CanPrepare = true;
        }




        public void AddFile(String fileFrom, String filePath, bool isNew, DirTreeViewItemModelStatus statusFile, String messageView)
        {
            if (statusFile != DirTreeViewItemModelStatus.Error) //? но теперь мы умеем копировать и с ошибками файлы - значит, неверный счетчик будет? подумать
            {
                if (isNew) NewFilesFrom++;
                else ReplaceFilesFrom++;
            }
            //+ dir
            var dirName = Path.GetDirectoryName(filePath);
            var dirItem = new DirTreeViewItemModel()
            {
                SourcePath = fileFrom,
                DistancePath = dirName, // важно, у директории тут путь к папке, а не файлу
                Title = dirName,
                IsDir = true,
                IsExcludeFromWork = false,
                Status = DirTreeViewItemModelStatus.Normal, // директории при подготовке всегда без ошибок (мы пока их не пытались создавать и не знаем их фактических ошибок)
                MessageView = String.Empty,
                IsNew = false // не нагружаем в этот момент - не проверяем наличие директории в дистанции
            };
            //- в своего родителя
            bool isFoundSelf;
            var dirParent = SearchItemDirParent(dirName, out isFoundSelf);
            if (dirParent == null) Items.Add(dirItem);
            else 
            {
                if (!isFoundSelf) dirParent.Items.Add(dirItem);
                else dirItem = dirParent;
            }

            //+ file
            var fileName = Path.GetFileName(filePath);
            var fileItem = new DirTreeViewItemModel()
            {
                SourcePath = fileFrom,
                DistancePath = filePath,
                Title = fileName,
                IsDir = false,
                IsExcludeFromWork = false,
                Status = statusFile,
                MessageView = messageView,
                IsNew = isNew
            };
            dirItem.Items.Add(fileItem);
        }

        private DirTreeViewItemModel SearchItemDirParent(String dirPath, out Boolean isSelf)
        {
            isSelf = false;
            if (Items == null || !Items.Any()) return null;
            var dirUri = CreateFolderUri(dirPath);
            return SearchDirParent(dirUri, Items, ref isSelf);
        }
        private DirTreeViewItemModel SearchDirParent(Uri dirUri, IEnumerable<DirTreeViewItemModel> items, ref Boolean isSelf)
        {
            if (items == null || !items.Any()) return null;
            DirTreeViewItemModel parent = null;
            foreach (var item in items)
            {
                if (!item.IsDir) continue;
                var dirOtherUri = CreateFolderUri(item.DistancePath);
                //- если это мы и есть
                if (dirOtherUri.Equals(dirUri))
                {
                    parent = item;
                    isSelf = true; //- нашли сами себя - ставим флаг, что не надо добавляться
                    break;
                }
                //- часть этого
                if (dirOtherUri.IsBaseOf(dirUri))
                {
                    parent = item;
                    var innerParent = SearchDirParent(dirUri, item.Items, ref isSelf);
                    if (innerParent != null) parent = innerParent;
                    break;
                }
            }
            return parent;
        }


        private Uri CreateFolderUri(String dirPath)
        {
            if (!dirPath.EndsWith("\\")) dirPath = dirPath + "\\"; // для функции IsBaseOf важно, чтобы отличать от файла - наличие слэша на конце
            return new Uri(dirPath);
        }



        public void OnChangeProject()
        {
            CanPrepare = true;
            CanWork = false;
            TotalFilesFrom = 0;
            NewFilesFrom = 0;
            ReplaceFilesFrom = 0;
            ErrorsFilesFrom = 0;
            Errors.Clear();
            Items.Clear();
        }


        public void OnPreparing()
        {
            CanPrepare = false;
            CanWork = false;
            TotalFilesFrom = 0;
            NewFilesFrom = 0;
            ReplaceFilesFrom = 0;
            ErrorsFilesFrom = 0;
            Errors.Clear();
            Items.Clear();
        }


        public void OnWorking()
        {
            CanPrepare = false;
            CanWork = false;
            Errors.Clear(); // будем новые при копировании считать
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }



    public enum DirTreeViewItemModelStatus
    {
        Normal = 0,
        Success = 1,
        Error = 2
    }



    public class DirTreeViewItemModel : INotifyPropertyChanged
    {
        public String SourcePath { get; set; }
        public String DistancePath { get; set; }
        public String Title { get; set; }
        public Boolean IsExcludeFromWork { get; set; }
        public Boolean IsNew { get; set; }
        public Boolean IsDir { get; set; }

        private DirTreeViewItemModelStatus _status;
        public DirTreeViewItemModelStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private String _messageView;
        public String MessageView
        {
            get { return _messageView; }
            set
            {
                _messageView = value;
                OnPropertyChanged("MessageView");
            }
        }


        public ObservableCollection<DirTreeViewItemModel> Items { get; set; }


        public DirTreeViewItemModel()
        {
            this.Items = new ObservableCollection<DirTreeViewItemModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }



        public void AsSuccess()
        {
            Status = DirTreeViewItemModelStatus.Success;
            MessageView = String.Empty;
        }
        public void AsError(String messageView)
        {
            Status = DirTreeViewItemModelStatus.Error;
            MessageView = messageView;
        }


    }






}
