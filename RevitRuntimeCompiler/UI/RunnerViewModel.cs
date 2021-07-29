using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RevitRuntimeCompiler.Editor;
using RevitRuntimeCompiler.Executor;
using RevitRuntimeCompiler.CodeProvider;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using System.ComponentModel;

namespace RevitRuntimeCompiler.UI
{
    public class RunnerViewModel : INotifyPropertyChanged
    {
        private Channel _channel = new Channel();
        private IEditor _editor;
        private IExecutor _executor;
        private ICodeProvider _provider;

        private readonly Dictionary<string, IEditor> _editors;
        private readonly Dictionary<string, (IExecutor, ICodeProvider)> _supportedLanguages;

        private ICommand _editCommand;
        public ICommand EditCommand => _editCommand ??= new RelayCommand(Edit);

        private ICommand _runCommand;
        public ICommand RunCommand => _runCommand ??= new AsyncCommand(RunAsync);

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new RelayCommand(Refresh);

        private ICommand _changeEditorCommand;
        public ICommand ChangeEditorCommand => _changeEditorCommand ??= new RelayCommand(ChangeEditor);

        private ICommand _changeLanguageCommand;
        public ICommand ChangeLanguageCommand => _changeLanguageCommand ??= new RelayCommand(ChangeLanguage);

        public ObservableCollection<string> ConsoleLines { get; private set; }
        public readonly List<string> Editors;
        public readonly List<string> Languages;

        public RunnerViewModel(IEnumerable<IEditor> editors, 
            Func<Channel,IEnumerable<(IExecutor,ICodeProvider)>> languagesProvider) 
        {
            _editors = editors.Where(e => e.IsInstalled())
                .ToDictionary(e => e.EditorName);
            _supportedLanguages = languagesProvider(_channel).ToDictionary(l => l.Item1.Language);
            ReadChannel();
        }

        private async void ReadChannel()
        {
            while (true)
            {
                var message = await _channel.ReadAsync();
                ConsoleLines.Add(message);
            }
        }

        private void Edit()
        {
            _editor.Edit(_provider.ExecutableFilePath);
        }

        private async Task RunAsync()
        {
            var code = _provider.GetCode();
            await _executor.ExecuteAsync(code);
        }

        private void Refresh()
        {
            ConsoleLines = new ObservableCollection<string>();
            _provider.Refresh();
        }

        private void ChangeEditor(object newEditor)
        {
            _editor = _editors[newEditor as string];
        }

        private void ChangeLanguage(object newLanguage)
        {
            var (executor, codeProvider) = _supportedLanguages[newLanguage as string];
            _executor = executor;
            _provider = codeProvider;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
