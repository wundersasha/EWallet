﻿using EWallet.Components;
using EWallet.Helpers;
using EWallet.Models;
using EWallet.Stores;
using EWallet.ViewModels;
using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace EWallet.Commands
{
    public class TransferCommand : CommandBase
    {
        #region Fields
        private readonly UserStore userStore;
        private readonly TransferViewModel transferViewModel;
        #endregion

        #region Constructors
        public TransferCommand(UserStore userStore, TransferViewModel transferViewModel)
        {
            this.userStore = userStore;
            this.transferViewModel = transferViewModel;
        }
        #endregion

        #region Methods
        public override void Execute(object parameter) 
            => Task.Run(ProvideTransfer);

        private async Task ProvideTransfer()
        {
            transferViewModel.IsOperationBeingProvided = true;

            try
            {
                using (var database = new WalletEntities())
                {
                    string cardNumber = HashHelper.GetHash(transferViewModel.CardNumber, 16);
                    Card card = await OperationsHelper.FetchCard(database, cardNumber);
                    OperationsHelper.CheckCard(card, this.userStore);

                    User user = await OperationsHelper.FetchUser(database, this.userStore);
                    double sum = SetSum();
                    OperationsHelper.TryUpdateBalance(user, this.userStore, sum);

                    Service service = await OperationsHelper.FetchService(database, 1);
                    Operation operation = OperationsHelper.GenerateOperation(database, card, user, sum, service);

                    database.User.AddOrUpdate(user);
                    database.Operation.Add(operation);
                    await database.SaveChangesAsync();
                }
            }
            catch (Exception e) { ErrorMessageBox.Show(e); }
            finally
            {
                transferViewModel.IsOperationBeingProvided = false;
            }
        }

        private double SetSum()
        {
            double.TryParse(transferViewModel.OperationSum, out double sum);
            double.TryParse(transferViewModel.Comission, out double comission);
            sum += comission;

            return sum;
        }
        #endregion
    }
}
