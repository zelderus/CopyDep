using CopyDep.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CopyDep.Utils
{
    public static class Configurator
    {


        class JsonConfigDataFile
        {
            public List<JsonConfigDataItem> items;
            public class JsonConfigDataItem
            {
                public Guid id;
                public String title;
                public Boolean isCurrent;
                public String dirFrom;
                public List<String> dirsFromIgnore;
                public String dirTo;
                public Boolean byCreateTime;
                public Boolean byLastWriteTime;
                public Boolean byLength;
                public Boolean byContent;
            }


            // NOTE: не очень красивое решение с ручным копированием (но очень не хочу обязывать этому модель представления)

            public void FillIn(ProjectsIn projectsIn) // from json
            {
                if (projectsIn == null) return; //? status error
                if (projectsIn.Items == null) projectsIn.Items = new System.Collections.ObjectModel.ObservableCollection<ProjectItemIn>();
                else projectsIn.Items.Clear();
                if (this.items == null) return;
                foreach (var dataEntity in this.items)
                {
                    var dataIn = new ProjectItemIn();
                    dataIn.Id = dataEntity.id;
                    dataIn.Title = dataEntity.title;
                    dataIn.DirFrom = dataEntity.dirFrom;
                    dataIn.DirFromIgnore = StringUtils.StringJoinNewLines(dataEntity.dirsFromIgnore);
                    dataIn.DirTo = dataEntity.dirTo;
                    dataIn.Options.ByCreateTime = dataEntity.byCreateTime;
                    dataIn.Options.ByLastWriteTime = dataEntity.byLastWriteTime;
                    dataIn.Options.ByLength = dataEntity.byLength;
                    dataIn.Options.ByContent = dataEntity.byContent;
                    projectsIn.Items.Add(dataIn);
                    // set current
                    if (dataEntity.isCurrent) projectsIn.Current = dataIn;
                }
            }

            public void Refresh(ProjectsIn projectsIn) // to json
            {
                if (projectsIn == null) return; //? status error
                if (projectsIn.Items == null) return;
                if (this.items == null) this.items = new List<JsonConfigDataItem>();
                else this.items.Clear();
                foreach (var dataIn in projectsIn.Items)
                {
                    var data = new JsonConfigDataItem();
                    data.id = dataIn.Id;
                    data.title = dataIn.Title;
                    data.dirFrom = dataIn.DirFrom;
                    data.dirsFromIgnore = StringUtils.StringSplitNewLines(dataIn.DirFromIgnore);
                    data.dirTo = dataIn.DirTo; // TODO: fix double symbols, errors etc
                    data.byCreateTime = dataIn.Options.ByCreateTime;
                    data.byLastWriteTime = dataIn.Options.ByLastWriteTime;
                    data.byLength = dataIn.Options.ByLength;
                    data.byContent = dataIn.Options.ByContent;
                    this.items.Add(data);
                    // set current
                    if (projectsIn.Current != null && projectsIn.Current.Id.Equals(data.id)) data.isCurrent = true;
                }
            }


        }




        public static void LoadConfig(ProjectsIn projectsIn, Status status)
        {
            //? async dispatcher
            status.IsError = false;
            status.Message = "Загрузка конфигурации..";
            // loading..
            try
            {
                JsonConfigDataFile data = null;
                using (StreamReader r = new StreamReader(Conventions.ConfigFilePath))
                {
                    string json = r.ReadToEnd();
                    data = JsonConvert.DeserializeObject<JsonConfigDataFile>(json);
                }
                if (data != null)
                {
                    data.FillIn(projectsIn);
                    status.Message = "Конфигурация загружена";
                }
            }
            catch(Exception e)
            {
                status.IsError = true;
                status.Message = e.Message;
            }
        }




        public static void SaveConfig(ProjectsIn projectsIn, Status status)
        {
            //? async dispatcher
            status.IsError = false;
            status.Message = "Сохранение конфигурации..";
            // saving..
            try
            {
                JsonConfigDataFile data = new JsonConfigDataFile();
                data.Refresh(projectsIn);
                var jsonSrtr = JsonConvert.SerializeObject(data);
                System.IO.File.WriteAllText(Conventions.ConfigFilePath, jsonSrtr);
                status.Message = "Конфигурация сохранена";
            }
            catch (Exception e)
            {
                status.IsError = true;
                status.Message = e.Message;
            }
        }



    }
}
