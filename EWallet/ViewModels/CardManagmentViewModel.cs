﻿using EWallet.Commands;
using EWallet.Components;
using EWallet.Helpers;
using EWallet.Models;
using EWallet.Services;
using EWallet.Stores;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace EWallet.ViewModels
{
    public class CardManagmentViewModel : ViewModelBase
    {
        private readonly UserStore userStore;
        private readonly WalletEntities database;

        private ICollectionView cards;
        private bool isThereCards;
        private bool areCardsLoading;

        public CardManagmentViewModel(UserStore userStore, INavigationService accountNavigatonService, INavigationService cardNavigationService)
        {
            this.userStore = userStore;

            try
            {
                database = new WalletEntities();
            }
            catch (Exception e) 
            { 
                ErrorMessageBox.Show(e);
                accountNavigatonService?.Navigate();
            }

            SetCards();

            NavigateAccountCommand = new NavigateCommand(accountNavigatonService);
            NavigateCardCommand = new NavigateCommand(cardNavigationService);
            DeleteCardCommand = new DeleteCardCommand(this);
        }

        public ICollectionView Cards
        {
            get => cards;
            set
            {
                cards = value;
                OnPropertyChanged(nameof(Cards));
            }
        }

        public bool IsThereCards
        {
            get => isThereCards;
            set
            {
                isThereCards = value;
                OnPropertyChanged(nameof(IsThereCards));
            }
        }
        public bool AreCardsLoading
        {
            get => areCardsLoading;
            set
            {
                areCardsLoading = value;
                OnPropertyChanged(nameof(AreCardsLoading));
            }
        }

        public void SetCards()
            => Task.Run(FetchCards);

        private void FetchCards()
        {
            AreCardsLoading = true;
            try
            {
                var cardsList = database
                    .Card
                    .Where(c => c.UserID == userStore.CurrentUser.ID)
                    .ToList()
                    .Select(c =>
                        new
                        {
                            Number = EncryptionHelper.Decrypt(c.Number),
                            ValidThru = c.ValidThru.ToString("D")
                        })
                    .ToList();
                Cards = new CollectionView(cardsList);
                Cards.Refresh();
            }
            catch (Exception e) { ErrorMessageBox.Show(e); }
            finally
            {
                CheckCardsCount();

                AreCardsLoading = false;
            }
        }

        private void CheckCardsCount()
        {
            try
            {
                if (((CollectionView)Cards).Count > 0)
                    IsThereCards = true;
                else
                    IsThereCards = false;
            }
            catch (Exception e) { ErrorMessageBox.Show(e); }
        }

        public override void Dispose()
        {
            database.Dispose();

            base.Dispose();
        }

        public ICommand NavigateAccountCommand { get; }
        public ICommand NavigateCardCommand { get; }
        public ICommand EditCardCommand { get; }
        public ICommand DeleteCardCommand { get; }
    }
}
