using CopyDep.Models;
using CopyDep.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CopyDep.Services.CopyWorker
{

    // NOTE: Реализация сервиса завязана на модели представления, что не очень красиво 


    /// <summary>
    /// Сервис копирования.
    /// </summary>
    public class CopyWorkerService : ICopyWorkerService
    {
        public CopyWorkerService()
        {
            
        }

        /// <summary>
        /// Подготовка к копированию. Сбор сведений.
        /// </summary>
        public void Prepare(Dispatcher DGUI, ProjectItemIn projectSettings, DirInfoModel infoModel, Status status)
        {
            DGUI.BeginInvoke(() =>
            {
                status.IsError = false;
                status.Message = "Подготовка..";
                infoModel.OnPreparing();
            }, DispatcherPriority.Normal);
            //+ checks
            if (projectSettings == null)
            {
                DGUI.BeginInvoke(() =>
                {
                    status.IsError = true;
                    status.Message = "Укажите проект";
                    infoModel.CanPrepare = true;
                });
                return;
            }
            if (String.IsNullOrWhiteSpace(projectSettings.DirFrom))
            {
                DGUI.BeginInvoke(() =>
                {
                    status.IsError = true;
                    status.Message = "Укажите путь к источникам";
                    infoModel.CanPrepare = true;
                });
                return;
            }
            if (String.IsNullOrWhiteSpace(projectSettings.DirTo))
            {
                DGUI.BeginInvoke(() =>
                {
                    status.IsError = true;
                    status.Message = "Укажите путь назначения";
                    infoModel.CanPrepare = true;
                });
                return;
            }
            var dirUtils = new DirectoryUtils();
            var totalFiles = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var filePrepareIndex = 0;
            Action<String> onCheckFn = (d) =>
            {
                TimeSpan ts = stopWatch.Elapsed; //? а точно ли в этом потоке замер времени останалвивать?
                DGUI.BeginInvoke(() =>
                {
                    status.Message = String.Format("Подготовка (шаг 1).. {0} ({1:00}:{2:00}:{3:00}) -> {4}", ++filePrepareIndex, ts.Hours, ts.Minutes, ts.Seconds, d);
                });
            };
            //+ 1. ищем все папки для работы (исходящие)
            var dirsIgnore = dirUtils.SearchDirs(StringUtils.StringSplitNewLines(projectSettings.DirFromIgnore), null, onCheckFn);//, Conventions.DirSeparatorStr);
            var dirs = dirUtils.SearchDirs(StringUtils.StringSplitNewLines(projectSettings.DirFrom), dirsIgnore, onCheckFn);//, Conventions.DirSeparatorStr);

            //+ 2. собираем все файлы (с их полными путями) для копирования
            var filePrepareTwoIndex = 0;
            Action<String> onCheckFilesFn = (f) =>
            {
                TimeSpan ts = stopWatch.Elapsed; //? а точно ли в этом потоке замер времени останалвивать?
                DGUI.BeginInvoke(() =>
                {
                    status.Message = String.Format("Подготовка (шаг 2).. {0} ({1:00}:{2:00}:{3:00}) -> {4}", ++filePrepareTwoIndex, ts.Hours, ts.Minutes, ts.Seconds, f);
                });
            };
            var files = dirUtils.SearchFiles(dirs, onCheckFilesFn);
            totalFiles = files.Count;

            //+ 3. ищем среди имеющихся по назначению уже точно такие-же (если нет таких файлов или новые, то отдаем в работу) - ради чего все и затеялось
            var dirFromStr = projectSettings.DirFrom;
            if (dirFromStr.EndsWith(Conventions.DirSubdirSymbol)) dirFromStr = dirFromStr.Remove(dirFromStr.Length - 1);
            var fileIndex = 0;
            foreach (var file in files)
            {
                TimeSpan ts = stopWatch.Elapsed; //? а точно ли в этом потоке замер времени останалвивать?
                DGUI.BeginInvoke(() =>
                {
                    status.Message = String.Format("Подготовка (шаг 3).. {0}/{1} ({2:00}:{3:00}:{4:00})", ++fileIndex, totalFiles, ts.Hours, ts.Minutes, ts.Seconds);
                });
                if (String.IsNullOrWhiteSpace(file)) continue;
                if (!file.StartsWith(dirFromStr)) continue; //- странно
                //- приводим к пути назначению
                var fileOut = String.Format("{0}\\{1}", projectSettings.DirTo, file.Remove(0, dirFromStr.Length));
                fileOut = fileOut.Replace("\\\\", "\\"); //? to regexp (учесть не только двойные слэши, но и множественные?)
                // NOTE: метод "infoModel.AddFile()" может не лучший по оптимизации для TreeView
                // NOTE: возможно, для TreeView делать разовую обновку быстрее, чем на каждом файле
                //- если новый, то точно копируем
                if (!dirUtils.ExistsFile(fileOut))
                {
                    DGUI.BeginInvoke(() =>
                    {
                        infoModel.AddFile(file, fileOut, true, DirTreeViewItemModelStatus.Normal, String.Empty);
                    });
                }
                //- если уже есть такой, то смотрим своим алгоритмом - вся суть приложения
                else
                {
                    var isFileInWork = false;
                    try
                    {
                        isFileInWork = IsFileForReplaceAlgo(file, fileOut, projectSettings, dirUtils);
                    }
                    catch(Exception ex)
                    {
                        // некоторая ошибка при сравнении файлов (возможно, не смогли сверить контент)
                        DGUI.BeginInvoke(() =>
                        {
                            infoModel.Errors.Add(ex.Message);
                            infoModel.AddFile(file, fileOut, false, DirTreeViewItemModelStatus.Error, ex.Message);
                        });
                    }

                    if (isFileInWork)
                    {
                        DGUI.BeginInvoke(() =>
                        {
                            infoModel.AddFile(file, fileOut, false, DirTreeViewItemModelStatus.Normal, String.Empty);
                        });
                    }
                }
            }
            stopWatch.Stop();

            //+ end (summary)
            DGUI.BeginInvoke(() =>
            {
                infoModel.TotalFilesFrom = totalFiles;
                infoModel.CanWork = infoModel.Items.Any(); // files.Any(); // TODO: в идеале проверить не отмененные файлы и директории
                infoModel.CanPrepare = true;
                infoModel.ErrorsFilesFrom = infoModel.Errors.Count();
                if (infoModel.ErrorsFilesFrom > 0)
                {
                    status.IsError = true;
                    status.Message = String.Format("Подготовка завершена с ошибками (ошибок: {0})", infoModel.ErrorsFilesFrom);
                }
                else
                {
                    status.Message = "Подготовка завершена";
                }
            });
        }


        /// <summary>
        /// Подлежит ли файл замене.
        /// </summary>
        /// <returns></returns>
        private Boolean IsFileForReplaceAlgo(String fileFrom, String fileTo, ProjectItemIn projectSettings, DirectoryUtils dirUtils)
        {
            var fileFromData = new FileInfo(fileFrom);
            var fileToData = new FileInfo(fileTo);
            // 1. дата создания
            if (projectSettings.Options.ByCreateTime)
            {
                if (fileFromData.CreationTime > fileToData.CreationTime) return true;
            }
            // 2. дата записи
            if (projectSettings.Options.ByLastWriteTime)
            {
                if (fileFromData.LastWriteTime > fileToData.LastWriteTime) return true;
            }
            // 3. размер
            if (projectSettings.Options.ByLength)
            {
                if (fileFromData.Length != fileToData.Length) return true;
            }
            // 4. разница в контенте
            if(projectSettings.Options.ByContent)
            {
                var onlyRead = true; //!projectSettings.Options.AccessReadWrite;
                if (!dirUtils.FileContentCompare(fileFrom, fileTo, onlyRead)) return true;
            }
            //- не нашли разницу
            return false;
        }



        /// <summary>
        /// Копирование файлов.
        /// </summary>
        public void CopyRun(Dispatcher DGUI, DirInfoModel infoModel, Status status)
        {
            DGUI.BeginInvoke(() =>
            {
                status.IsError = false;
                status.Message = "Подготовка к копированию..";
                infoModel.OnWorking();
            }, DispatcherPriority.Normal);

            if (infoModel == null || !infoModel.CanWork)
            {
                DGUI.BeginInvoke(() =>
                {
                    status.IsError = true;
                    status.Message = "Нет данных";
                    if (infoModel != null) infoModel.CanPrepare = true;
                });
                return;
            }
            //+ doing
            DGUI.BeginInvoke(() =>
            {
                status.Message = "Копирование..";
            });
            var currentIndex = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var item in infoModel.Items)
            {
                CopyItemWithInners(DGUI, infoModel, item, status, infoModel.TotalFilesFrom, stopWatch, ref currentIndex);
            }
            stopWatch.Stop();
            //+ stats
            DGUI.BeginInvoke(() =>
            {
                if (infoModel.Errors.Any())
                {
                    status.IsError = true;
                    status.Message = String.Format("Копирование завершено с ошибками (ошибок: {0})", infoModel.Errors.Count());
                    infoModel.CanPrepare = true;
                    infoModel.CanWork = true; //- если была ошибка, то даем сразу еще возможность попробовать (может, исправили что-то с правами доступа или т.п.)
                }
                else
                {
                    status.IsError = false;
                    status.Message = "Копирование завершено";
                    infoModel.CanPrepare = true;
                }
            });
        }

        private void CopyItemWithInners(Dispatcher DGUI, DirInfoModel infoModel, DirTreeViewItemModel item, Status status, Int32 totalCount, Stopwatch stopWatch, ref Int32 currentIndex)
        {
            if (item == null) return;
            if (item.IsExcludeFromWork) return;
            if (!infoModel.IsCopyWithErrorAfterPrepare && !item.IsDir && item.Status == DirTreeViewItemModelStatus.Error) return; // с ошибками не трогаем
            var index = currentIndex++;
            //+ count (loading steps)
            TimeSpan ts = stopWatch.Elapsed;
            DGUI.BeginInvoke(() =>
            {
                status.Message = String.Format("Копирование.. {0}/{1} ({2:00}:{3:00}:{4:00})", index, totalCount, ts.Hours, ts.Minutes, ts.Seconds);
            });
            //+ copy file
            if (!item.IsDir)
            {
                try
                {
                    System.IO.File.Copy(item.SourcePath, item.DistancePath, true);
                    DGUI.BeginInvoke(() =>
                    {
                        item.AsSuccess();
                    });
                }
                catch(Exception ex)
                {
                    var msg = ex.Message; //? to last inner msg?
                    DGUI.BeginInvoke(() =>
                    {
                        item.AsError(msg);
                        infoModel.Errors.Add(msg);
                    });
                }
            }
            //+ create dir
            else
            {
                try
                {
                    if (!System.IO.Directory.Exists(item.DistancePath))
                    {
                        System.IO.Directory.CreateDirectory(item.DistancePath);
                        DGUI.BeginInvoke(() =>
                        {
                            item.AsSuccess();
                        });
                    }
                    else
                    {
                        //- директория уже была, и мы ничего не делали, но мы все-равно ее подсветим успешно
                        DGUI.BeginInvoke(() =>
                        {
                            item.AsSuccess();
                        });
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message; //? to last inner msg?
                    DGUI.BeginInvoke(() =>
                    {
                        item.AsError(msg);
                        infoModel.Errors.Add(msg);
                    });
                }
            }
            //+ childs
            foreach (var itemChild in item.Items)
            {
                CopyItemWithInners(DGUI, infoModel, itemChild, status, totalCount, stopWatch, ref currentIndex);
            }
        }




    }


}
