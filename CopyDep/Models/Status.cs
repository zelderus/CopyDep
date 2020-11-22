using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CopyDep.Models
{
    public class Status : INotifyPropertyChanged
    {
        private String _message;
        public String Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }


        private Boolean _isError;
        public Boolean IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                OnPropertyChanged("IsError");
            }
        }


        private Boolean _isButtonProjectAddQuestion;
        public Boolean IsButtonProjectAddQuestion
        {
            get { return _isButtonProjectAddQuestion; }
            set
            {
                _isButtonProjectAddQuestion = value;
                OnPropertyChanged("IsButtonProjectAddQuestion");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }




        public void OnChangeProject()
        {
            IsError = false;
            Message = String.Empty;
        }


    }
}
