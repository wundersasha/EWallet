﻿using EWallet.Commands;
using EWallet.NET.Models;
using EWallet.Services;
using EWallet.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace EWallet.ViewModels
{
    /// <summary>
    /// ViewModel аккаунта пользователя.
    /// </summary>
    public class AccountViewModel : ViewModelBase
    {
        #region Fields
        public readonly UserStore userStore;
        private ICollectionView listServices;

        private double balance;
        private double operationBalance;
        private string selectedService;
        private string stringBalance;
        private string cardNumber;

        private bool isConfirmButtonEnabled = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Инициализирует новый объект типа AccounViewModel.
        /// </summary>
        /// <param name="user">Пользователь, прошедший авторизацию в системе.</param>
        /// <param name="navigationStore">Хранилище данных, содержащее данные о текущей ViewModel.</param>
        public AccountViewModel(UserStore userStore, INavigationService authorizationNavigationService)
        {
            this.userStore = userStore;
            Balance = userStore.CurrentUser.Balance;
            StringBalance = string.Format($"Баланс: {Balance}");

            using (var db = new WalletEntities())
            {
                IList<Service> services = new List<Service>(db.Service);
                ListServices = CollectionViewSource.GetDefaultView(services);
                SelectedService = services != null ? services[0].Name : "";
            }

            DoOperationCommand = new DoOperationCommand(this);
            ExitAccountCommand = new ExitAccountCommand(userStore, authorizationNavigationService);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Баланс счёта пользователя.
        /// </summary>
        /// <value>Содержит баланс пользователя 
        /// в виде десятичного числа с двойной точностью.</value>
        public double Balance
        {
            get => balance;
            set
            {
                balance = value;
                StringBalance = string.Format($"Баланс:\n{Balance}");
                OnPropertyChanged(nameof(Balance));
            }
        }

        /// <summary>
        /// Строка отображения баланса пользователя во View.
        /// </summary>
        /// <value>
        /// Строковое представление баланса пользователя.
        /// </value>
        public string StringBalance
        {
            get => stringBalance;
            set
            {
                stringBalance = value;
                OnPropertyChanged(nameof(StringBalance));
            }
        }

        /// <summary>
        /// Список доступных пользователям услуг для отображения во View.
        /// </summary>
        /// <value>
        /// Коллекция данных об услугах.
        /// </value>
        public ICollectionView ListServices
        {
            get => listServices;
            set
            {
                listServices = value;
                OnPropertyChanged(nameof(ListServices));
            }
        }

        /// <summary>
        /// Выбранное значение услуги в ListServices.
        /// </summary>
        /// <value>
        /// Строковое представление названия услуги.
        /// </value>
        public string SelectedService
        {
            get => selectedService;
            set
            {
                selectedService = value;
                OnPropertyChanged(nameof(SelectedService));
                if (selectedService != null && cardNumber != "" && operationBalance != 0) IsConfirmButtonEnabled = true;
            }
        }

        /// <summary>
        /// Номер карты пользователя.
        /// </summary>
        /// <value>
        /// Строковое представление номера карты пользователя.
        /// </value>
        public string CardNumber
        {
            get => cardNumber;
            set
            {
                long.TryParse(value.Replace(" ", ""), out long number);

                if (number != 0 || value != " ")
                {
                    int length = value.Replace(" ", "").Length;
                    if (length <= 4)
                        cardNumber = value;
                    else if (length <= 8)
                        cardNumber = string.Format($"{number:#### ####}");
                    else if (length <= 12)
                        cardNumber = string.Format($"{number:#### #### ####}");
                    else
                        cardNumber = string.Format($"{number:#### #### #### ####}");
                }

                if (cardNumber != null)
                {
                    cardNumber.Trim(' ');
                }

                OnPropertyChanged(nameof(CardNumber));
                if (cardNumber != "" && operationBalance != 0 && selectedService != null)
                    IsConfirmButtonEnabled = true;
            }
        }

        /// <summary>
        /// Баланс операции.
        /// </summary>
        /// <value>
        /// Действительное число с двойной точностью, содержащее баланс операции.
        /// </value>
        public double OperationBalance
        {
            get => operationBalance;
            set
            {
                if (operationBalance != value)
                {
                    operationBalance = value;
                    OnPropertyChanged(nameof(OperationBalance));
                    if (cardNumber != "" && operationBalance != 0 && selectedService != null)
                        IsConfirmButtonEnabled = true;
                }
            }
        }

        /// <summary>
        /// Флаг, отвечающий за включение и отключение кнопки проведения операции.
        /// </summary>
        /// <value>
        /// Булева переменная, содержащая состояние кнопки проведения операции.
        /// </value>
        public bool IsConfirmButtonEnabled
        {
            get => isConfirmButtonEnabled;
            set
            {
                isConfirmButtonEnabled = value;
                OnPropertyChanged(nameof(IsConfirmButtonEnabled));
            }
        }
        #endregion

        #region Commands
        public ICommand OpenCalculatorCommand { get; }
        public ICommand DoOperationCommand { get; }
        public ICommand ExitAccountCommand { get; }
        #endregion

        #region Methods
        public override void Dispose() 
            => base.Dispose();
        #endregion
    }
}