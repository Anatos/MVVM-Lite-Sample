﻿using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Cinegy.ImportTool.FileSystem.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Cinegy.ImportTool.FileSystem.ViewModel
{
    public class TreeViewModel : ViewModelBase
    {
        private ObservableCollection<FolderModel> _itemSource = new ObservableCollection<FolderModel>();
        private RelayCommand<object> _selectedCommand;

        #region Constructors

        public TreeViewModel()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var drive in Directory.GetLogicalDrives())
                {
                    ItemsSource.Add(new FolderModel(new DirectoryInfo(drive)));
                }
            });
        }

        #endregion

        #region Properties

        public ObservableCollection<FolderModel> ItemsSource
        {
            get { return _itemSource; }
            set
            {
                if (_itemSource == value) return;

                _itemSource = value;
                RaisePropertyChanged(nameof(ItemsSource));
            }
        }

        public ICommand SelectedCommand
        {
            get
            {
                return _selectedCommand ?? (_selectedCommand = new RelayCommand<object>(e =>
                {
                    var args = e as RoutedPropertyChangedEventArgs<object>;
                    if (args == null) return;

                    (args.OldValue as FolderModel)?.Unloaded();

                    var @new = args.NewValue as FolderModel;
                    if (@new != null)
                    {
                        @new.Loaded();
                        MessengerInstance.Send($"Tree item {@new} changed!");
                        MessengerInstance.Send(new GenericMessage<DirectoryInfo>(@new.Directory));
                    }
                }));
            }
        }

        #endregion
    }
}