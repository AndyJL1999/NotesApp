using Firebase.Storage;
using NotesApp.MVVM.Model;
using NotesApp.MVVM.ViewModel.Commands;
using NotesApp.MVVM.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace NotesApp.MVVM.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
        #region ----------Fields----------
        private Notebook _selectedNotebook;
        private Visibility _isVisible;
		private Note _selectedNote;
        private FlowDocument _noteDocument;
        private bool _textEditAllowed;
        private ICommand _newNotebookCommand;
        private ICommand _editCommand;
        private ICommand _endEditCommand;
        private ICommand _newNoteCommand;
        private ICommand _deleteCommand;
        private ICommand _saveCommand;
        #endregion

        #region ----------Events----------
        public event PropertyChangedEventHandler? PropertyChanged;
		public event EventHandler? SelectedNoteChanged;
		public event EventHandler? SelectedNotebookChanged;
        #endregion

        //Constructor
        public NotesVM()
        {
            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();

            NoteDocument = new FlowDocument();
            TextEditAllowed = false;

            IsVisible = Visibility.Collapsed;

            SelectedNoteChanged += NotesVM_SelectedNoteChanged;
        }

        #region ----------Properties----------
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

        public FlowDocument NoteDocument 
        { 
            get { return _noteDocument; }
            set
            {
                _noteDocument = value;
                OnPropertyChanged(nameof(NoteDocument));
            } 
        }

        public bool TextEditAllowed 
        { 
            get { return _textEditAllowed; }
            set
            {
                _textEditAllowed = value;
                OnPropertyChanged(nameof(TextEditAllowed));
            }
        }
        public ICommand NewNotebookCommand 
        {
            get
            {
                if (_newNotebookCommand is null)
                {
                    _newNotebookCommand = new RelayCommand(p => CreateNotebook(), p => true);
                }

                return _newNotebookCommand;
            } 
        }
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand is null)
                {
                    _editCommand = new RelayCommand(p => StartEditing(), p => true);
                }

                return _editCommand;
            }
        }
        public ICommand EndEditCommand
        {
            get
            {
                if (_endEditCommand is null)
                {
                    _endEditCommand = new RelayCommand(p => StopEditing((Notebook)p), p => p != null);
                }

                return _endEditCommand;
            }
        }
        public ICommand NewNoteCommand
        {
            get
            {
                if (_newNoteCommand is null)
                {
                    _newNoteCommand = new RelayCommand(p => CreateNote(SelectedNotebook.Id), p => p != null);
                }

                return _newNoteCommand;
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand is null)
                {
                    _deleteCommand = new RelayCommand(p => Delete(p), p => true);
                }

                return _deleteCommand;
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand is null)
                {
                    _saveCommand = new RelayCommand(p => SaveChanges(), p => true);
                }

                return _saveCommand;
            }
        }
        public ObservableCollection<Notebook> Notebooks { get; set; }
        public ObservableCollection<Note> Notes { get; set; }
        #endregion

        #region ----------Methods----------
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

        public void Delete(object parameter)
        {
            if (parameter.GetType() == typeof(Note))
            {
                DeleteNote();
            }

            if (parameter.GetType() == typeof(Notebook))
            {
                DeleteNotebook();
            }
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

        public async void SaveChanges()
        {
            string fileName = $"{SelectedNote.Id}.rtf";
            string rtf_File = Path.Combine(Environment.CurrentDirectory, fileName);

            using (FileStream fileStream = new FileStream(rtf_File, FileMode.Create))
            {
                var contents = new TextRange(NoteDocument.ContentStart, NoteDocument.ContentEnd);

                contents.Save(fileStream, DataFormats.Rtf);
            }

            SelectedNote.FileLocation = await DatabaseHelper.UpdateFile(rtf_File, fileName);
            await DatabaseHelper.Update(SelectedNote);
        }

        private async void DeleteNote()
        {
            await DatabaseHelper.Delete(_selectedNote);
            GetNotes();
        }

        private async void DeleteNotebook()
        {
            await DatabaseHelper.Delete(_selectedNotebook);
            GetNotebooks();
        }

        private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

        //Executes when a note is selected
        private async void NotesVM_SelectedNoteChanged(object? sender, EventArgs e)
        {
            NoteDocument.Blocks.Clear();

            if(SelectedNote is null)
            {
                TextEditAllowed = false;
                return;
            }

            TextEditAllowed = true;

            if (!string.IsNullOrEmpty(SelectedNote.FileLocation))
            {
                //Get note from cloud storage | WARNINING: The call to Firebase storage causes delay
                string downloadPath = await new FirebaseStorage(DatabaseHelper.bucket).Child(SelectedNote.Id + ".rtf").GetDownloadUrlAsync();

                //Update the local storage note with its corresponding cloud storage note
                using (HttpResponseMessage response = await DatabaseHelper.httpClient.GetAsync(downloadPath))
                {
                    using (Stream fileStream = await response.Content.ReadAsStreamAsync())
                    {
                        var contents = new TextRange(NoteDocument.ContentStart, NoteDocument.ContentEnd);
                        contents.Load(fileStream, DataFormats.Rtf);
                    }
                }
            }
        }
        #endregion
    }
}
