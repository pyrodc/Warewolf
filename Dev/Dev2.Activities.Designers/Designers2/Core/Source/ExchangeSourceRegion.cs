﻿using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Dev2.Common.Common;
using Dev2.Common.Interfaces.ToolBase;
using Dev2.Common.Interfaces.ToolBase.ExchangeEmail;
using Dev2.Runtime.Configuration.ViewModels.Base;
using Dev2.Studio.Core.Activities.Utils;

namespace Dev2.Activities.Designers2.Core.Source
{
    public class ExchangeSourceRegion : ISourceToolRegion<IExchangeSource>
    {
        Guid _sourceId;
        readonly ModelItem _modelItem;
        Action _sourceChangedAction;
        IExchangeSource _selectedSource;
        ICollection<IExchangeSource> _sources;
        public ICommand EditSourceCommand { get; set; }
        public ICommand NewSourceCommand { get; set; }

        public Action SourceChangedAction
        {
            get => _sourceChangedAction ?? (() => { });
            set
            {
                _sourceChangedAction = value;
            }
        }

        public ICollection<IExchangeSource> Sources
        {
            get => _sources;
            set
            {
                _sources = value;
                OnPropertyChanged();
            }
        }

        public event SomethingChanged SomethingChanged;
        public double LabelWidth { get; set; }
        public string NewSourceHelpText { get; set; }
        public string EditSourceHelpText { get; set; }
        public string SourcesHelpText { get; set; }
        public string NewSourceTooltip { get; set; }
        public string EditSourceTooltip { get; set; }
        public string SourcesTooltip { get; set; }

        public ExchangeSourceRegion()
        {
        }

        public ExchangeSourceRegion(IExchangeServiceModel model, ModelItem modelItem, string type)
        {
            LabelWidth = 70;
            ToolRegionName = "ExchangeSourceRegion";
            Dependants = new List<IToolRegion>();
            NewSourceCommand = new DelegateCommand(o=>model.CreateNewSource());
            EditSourceCommand = new DelegateCommand(o => model.EditSource(SelectedSource),o=> CanEditSource());
            var sources = model.RetrieveSources().OrderBy(source => source.ResourceName);
            Sources = sources.Where(source => source != null && source.ResourceType == type).ToObservableCollection();
            IsEnabled = true;
            _modelItem = modelItem;
            SetSourceId(modelItem.GetProperty<Guid>("SourceId"));

            if (SavedSource != null)
            {
                SelectedSource = Sources.FirstOrDefault(source => source.ResourceID == SavedSource.ResourceID);
            }
        }

        public bool CanEditSource() => SelectedSource != null;

        void SetSourceId(Guid value)
        {
            _sourceId = value;
            _modelItem?.SetProperty("SourceId", value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string ToolRegionName { get; set; }
        public bool IsEnabled { get; set; }
        public IList<IToolRegion> Dependants { get; set; }

        public IList<string> Errors { get; set; }

        public IToolRegion CloneRegion() => new ExchangeSourceRegion
        {
            SelectedSource = SelectedSource
        };

        public void RestoreRegion(IToolRegion toRestore)
        {
            if (toRestore is ExchangeSourceRegion region)
            {
                SelectedSource = region.SelectedSource;
            }
        }

        public EventHandler<List<string>> ErrorsHandler { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnSomethingChanged(IToolRegion args)
        {
            var handler = SomethingChanged;
            handler?.Invoke(this, args);
        }

        public IExchangeSource SelectedSource
        {
            get => _selectedSource;
            set
            {
                SetSelectedSource(value);
                SourceChangedAction?.Invoke();
                OnSomethingChanged(this);
                var delegateCommand = EditSourceCommand as DelegateCommand;
                delegateCommand?.RaiseCanExecuteChanged();
            }
        }

        void SetSelectedSource(IExchangeSource value)
        {
            if (value != null)
            {
                _selectedSource = value;
                SavedSource = value;
                SetSourceId(value.ResourceID);
            }

            OnPropertyChanged("SelectedSource");
        }

        public IExchangeSource SavedSource
        {
            get => _modelItem.GetProperty<IExchangeSource>("SavedSource");
            set =>  _modelItem.SetProperty("SavedSource", value);
        }
    }
}
