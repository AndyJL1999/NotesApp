using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.MVVM.ViewModel.Commands
{
    public class EditCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public NotesVM VM { get; set; }

        public EditCommand(NotesVM vM)
        {
            VM = vM;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            VM.StartEditing();
        }
    }
}
