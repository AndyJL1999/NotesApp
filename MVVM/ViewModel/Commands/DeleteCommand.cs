using NotesApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.MVVM.ViewModel.Commands
{
    public class DeleteCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public NotesVM VM { get; set; }

        public DeleteCommand(NotesVM vM)
        {
            VM = vM;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {

            if (parameter.GetType() == typeof(Note))
                VM.DeleteNote();

            if (parameter.GetType() == typeof(Notebook))
                VM.DeleteNotebook();
        }
    }
}
