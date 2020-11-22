using CopyDep.Models;
using CopyDep.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CopyDep.Services
{

    public interface ICopyWorkerService
    {
        void Prepare(Dispatcher DGUI, ProjectItemIn dirInfoDataIn, DirInfoModel infoModel, Status status);
        void CopyRun(Dispatcher DGUI, DirInfoModel infoModel, Status status);
    }

    public class CopyWorkerPrepareOptions
    {

    }


}
