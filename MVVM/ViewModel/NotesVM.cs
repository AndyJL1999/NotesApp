using NotesApp.MVVM.Model;
using NotesApp.MVVM.ViewModel.Commands;
using NotesApp.MVVM.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NotesApp.MVVM.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
		private Notebook _selectedNotebook;
        private Visibility _isVisible;
		private Note _selectedNote;

		public event PropertyChangedEventHandler? PropertyChanged;
		public event EventHandler? SelectedNoteChanged;
		public event EventHandler? SelectedNotebookChanged;

        //Properties
        public Notebook SelectedNotebook
		{
			get { return _selectedNotebook; }
			set 
			{ 
				_selectedNotebook = value;
				OnPropertyChanged(nameof(SelectedNotebook));
				SelectedNotebookChanged?.Invoke(this, new EventArgs());
				//Get notes
				GetNotes();
			}
		}
        public Note SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                _selectedNote = value;
                OnPropertyChanged(nameof(SelectedNote));
				SelectedNoteChanged?.Invoke(this, new EventArgs());
            }
        }
        public Visibility IsVisible
		{
			get { return _isVisible; }
			set 
			{ 
				_isVisible = value; 
				OnPropertyChanged(nameof(IsVisible));
			}
		}

		public ObservableCollection<Notebook> Notebooks { get; set; }
        public ObservableCollection<Note> Notes { get; set; }
		public NewNotebookCommand NewNotebookCommand { get; set; }
		public EditCommand EditCommand { get; set; }
		public EndEditCommand EndEditCommand { get; set; }
		public NewNoteCommand NewNoteCommand { get; set; }
		public DeleteCommand DeleteCommand { get; set; }

		//Constructor
		public NotesVM()
		{
			//Initialize commands
			NewNotebookCommand = new NewNotebookCommand(this);
			NewNoteCommand = new NewNoteCommand(this);
			EditCommand = new EditCommand(this);
			EndEditCommand = new EndEditCommand(this);
			DeleteCommand = new DeleteCommand(this);

			Notebooks = new ObservableCollection<Notebook>();
			Notes = new ObservableCollection<Note>();

			IsVisible = Visibility.Collapsed;
		}

		//Methods
        public async void CreateNotebook()
        {
            Notebook newNotebook = new Notebook
            {
                Name = "New notebook",
				UserId = App.UserId
            };

            await DatabaseHelper.Insert(newNotebook);

			GetNotebooks();
        }

        public async void CreateNote(string notebookId)
		{
			Note newNote = new Note
			{
				NotebookId = notebookId,
				CreatedTime = DateTime.Now,
				UpdatedTime = DateTime.Now,
				Title = $"Note"
			};

			await DatabaseHelper.Insert(newNote);

			GetNotes();
		}

		public async void DeleteNote()
		{
			await DatabaseHelper.Delete(_selectedNote);
			GetNotes();
		}

        public async void DeleteNotebook()
        {
            await DatabaseHelper.Delete(_selectedNotebook);
            GetNotebooks();
        }

        public async void GetNotebooks()
		{
			string appUserId = App.UserId;


			var notebooks = (await DatabaseHelper.Read<Notebook>())?
				.Where(n => n.UserId == App.UserId).ToList();

			Notebooks.Clear();
			if(notebooks != null)
			{
                foreach (var notebook in notebooks)
                {
                    Notebooks.Add(notebook);
                }
            }
		}

        private async void GetNotes()
        {
			if(SelectedNotebook != null)
			{
                var notes = (await DatabaseHelper.Read<Note>())?
                .Where(n => n.NotebookId == SelectedNotebook.Id).ToList();

                Notes.Clear();

				if(notes != null)
				{
                    foreach (var note in notes)
                    {
                        Notes.Add(note);
                    }
                }
            }
        }

        public void StartEditing()
        {
			IsVisible = Visibility.Visible;
        }

        public void StopEditing(Notebook notebook)
        {
            IsVisible = Visibility.Collapsed;
			DatabaseHelper.Update(notebook);
			GetNotebooks();
        }

        private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
