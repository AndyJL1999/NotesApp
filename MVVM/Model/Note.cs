using NotesApp.MVVM.ViewModel.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.MVVM.Model
{
    public class Note : INotifyPropertyChanged, IHasId
    {
        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string notebookId;
        public string NotebookId
        {
            get { return notebookId; }
            set
            {
                notebookId = value;
                OnPropertyChanged(nameof(NotebookId));
            }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private DateTime createdTime;

        public DateTime CreatedTime
        {
            get { return createdTime; }
            set
            {
                createdTime = value;
                OnPropertyChanged(nameof(CreatedTime));
            }
        }

        private DateTime updatedTime;

        public DateTime UpdatedTime
        {
            get { return updatedTime; }
            set
            {
                updatedTime = value;
                OnPropertyChanged(nameof(UpdatedTime));
            }
        }

        private string fileLocation;

        public string FileLocation
        {
            get { return fileLocation; }
            set
            {
                fileLocation = value;
                OnPropertyChanged(nameof(FileLocation));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
