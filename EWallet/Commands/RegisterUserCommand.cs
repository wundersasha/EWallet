﻿using EWallet.Components;
using EWallet.Helpers;
using EWallet.Models;
using EWallet.Services;
using EWallet.Stores;
using EWallet.ViewModels;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EWallet.Commands
{
    /// <summary>
    /// Команда регистрации пользователя.
    /// </summary>
    public sealed class RegisterUserCommand : CommandBase
    {
        #region Fields
        private readonly RegistrationViewModel registrationViewModel;
        private readonly INavigationService accountNavigationService;
        private readonly UserStore userStore;
        #endregion

        #region Constructors
        /// <summary>
        /// Инициализирует команду регистрации пользователя.
        /// </summary>
        /// <param name="registrationViewModel">ViewModel страницы регистрации пользователя.</param>
        /// <param name="accountNavigationService">Сервис навигации, привязанный к AccountViewModel.</param>
        public RegisterUserCommand(RegistrationViewModel registrationViewModel, 
            INavigationService accountNavigationService, 
            UserStore userStore)
        {
            this.registrationViewModel = registrationViewModel;
            this.accountNavigationService = accountNavigationService;
            this.userStore = userStore;
        }
        #endregion

        #region Methods
        ///<inheritdoc cref="CommandBase.Execute(object)"/>
        public override void Execute(object parameter) 
            => Task.Run(RegisterUserInDataBase);

        public async Task RegisterUserInDataBase()
        {
            registrationViewModel.IsUserAuthorizing = true;

            try
            {
                using (var database = new WalletEntities())
                {
                    var user = await FetchUser(database);

                    if (user == null)
                    {
                        int length = 16;
                        if (registrationViewModel.Password.Length < 16)
                            length = registrationViewModel.Password.Length;

                        await AddUserToDataBase(database, length);
                        await AddPassportToDataBase(database);
                    }
                    else
                        throw new Exception("Пользователь уже зарегистрирован в системе!");

                    userStore.CurrentUser = user;
                    accountNavigationService?.Navigate();
                }
            }
            catch (Exception ex) { ErrorMessageBox.Show(ex); }
            finally { registrationViewModel.IsUserAuthorizing = false; }
        }

        private async Task<User> FetchUser(WalletEntities database)
            => await database.User.AsNoTracking().FirstOrDefaultAsync(
                u => u.Login == registrationViewModel.Login);
        private async Task AddUserToDataBase(WalletEntities database, int length)
        {
            var user = new User()
            {
                Login = registrationViewModel.Login,
                Password = HashHelper.GetHash(registrationViewModel.Password, length),
                RoleID = 1,
                Balance = 0
            };
            database.User.Add(user);
            await database.SaveChangesAsync();
        }
        private async Task AddPassportToDataBase(WalletEntities database)
        {
            Passport userPassport = new Passport()
            {
                FirstName = registrationViewModel.FirstName,
                LastName = registrationViewModel.LastName,
                Patronymic = registrationViewModel?.Patronymic,
                SerialNumber = 0,
                DivisionCode = 0,
                Number = 0,
                UserID = FetchUser(database).Result.ID
            };

            database.Passport.Add(userPassport);
            await database.SaveChangesAsync();
        }
        #endregion
    }
}
