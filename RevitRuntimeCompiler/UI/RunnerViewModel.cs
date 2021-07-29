using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RevitRuntimeCompiler.Editor;
using RevitRuntimeCompiler.Executor;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;

namespace RevitRuntimeCompiler.UI
{
    public class RunnerViewModel
    {
        public ObservableCollection<string> ConsoleLines { get; private set; }
        private Channel _channel = new Channel();
        private IEditor _editor;
        private IExecutor _executor;

        private ICommand _editCommand;
        public ICommand EditCommand => _editCommand ??= new AsyncCommand(Edit);

        private ICommand _runCommand;
        public ICommand RunCommand => _runCommand ??= new AsyncCommand(RunAsync);

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(Refresh);

        public RunnerViewModel() 
        {
            ReadChannel();
            _editor = new VSSolutionEditor();
            _executor = new CSharpExecutor(_channel, new CSharpCompiler(_channel));
            ConsoleLines = new ObservableCollection<string>();
        }

        private async void ReadChannel()
        {
            while (true)
            {
                var message = await _channel.ReadAsync();
                ConsoleLines.Add(message);
            }
        }

        private Task Edit()
        {
            _editor.Edit();
            return Task.CompletedTask;
        }

        private async Task RunAsync()
        {
            var code = _editor.GetCode();
            await _executor.ExecuteAsync(code);
        }

        private Task Refresh()
        {
            _editor.Refresh();
            return Task.CompletedTask;
        }
    }
}
