using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.Windows;

namespace ImageOCR.ViewModels
{
    public class CaptureViewModel : BindableBase, INotifyPropertyChanged, IDialogAware
    {
        private string _iconSource;
        public string IconSource
        {
            get { return _iconSource; }
            set { SetProperty(ref _iconSource, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ReactiveCommand<Rect> TranslateCommand { get; set; } = new ReactiveCommand<Rect>();
        public CaptureViewModel(IEventAggregator eventAggregator)
        {
            TranslateCommand.Subscribe((rect) =>
            {
                eventAggregator.GetEvent<PubSubEvent<Rect>>().Publish(rect);
                RaiseRequestClose(null);
            });
        }

        public event Action<IDialogResult> RequestClose;
        public virtual void RaiseRequestClose(IDialogResult dialogResult) => RequestClose?.Invoke(dialogResult);
        public virtual bool CanCloseDialog() => true;
        public virtual void OnDialogClosed() { }
        public virtual void OnDialogOpened(IDialogParameters parameters) { }
    }
}