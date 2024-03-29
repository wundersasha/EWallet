﻿using EWallet.Models;

namespace EWallet.Stores
{
    public sealed class CardStore
    {
        #region Fields
        private Card currentCard;
        #endregion

        #region Properties
        /// <summary>
        /// Текущая карта.
        /// </summary>
        public Card CurrentCard
        {
            get => currentCard;
            set => currentCard = value;
        }
        #endregion
    }
}