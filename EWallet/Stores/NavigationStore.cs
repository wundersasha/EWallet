﻿using EWallet.ViewModels;
using System;

namespace EWallet.Stores
{
    /// <summary>
    /// Навигационное хранилище для 
    /// хранения данных о текущей ViewModel.
    /// </summary>
    public sealed class NavigationStore
    {
        #region Fields
        private ViewModelBase currentViewModel;
        #endregion

        #region Events
        /// <summary>
        /// Событие при изменении CurrentViewModel.
        /// </summary>
        public event Action CurrentViewModelChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Текущая ViewModel.
        /// </summary>
        public ViewModelBase CurrentViewModel 
        {
            get => currentViewModel;
            set
            {
                currentViewModel?.Dispose();
                currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Метод, обрабатывающий изменение текущей ViewModel.
        /// </summary>
        private void OnCurrentViewModelChanged() 
            => CurrentViewModelChanged?.Invoke();
        #endregion
    }
}
