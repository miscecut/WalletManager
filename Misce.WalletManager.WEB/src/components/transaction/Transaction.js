import React from 'react';
//css
import './Transaction.css';
//images
import BankLogo from '../../images/bank-icon.png'
import CalendarIcon from '../../images/calendar-icon.png'
import ArrowIconWhite from '../../images/arrow-right-icon.png'
import TransactionCategoryIcon from '../../images/category-icon.png'
//utils
import { formatMoneyAmount } from './../../jsutils/beautifiers.js';

function Transaction({ transaction, openTransactionUpdateModal }) {
    //determine the transaction type for the correct icon to show
    let transactionType = 'transfer';
    if (transaction.fromAccount == null)
        transactionType = 'profit';
    else if (transaction.toAccount == null)
        transactionType = 'expense';

    function getTransactionCardSubTitle() {
        let isTransfer = transactionType == 'transfer';
        if (isTransfer)
            return <p className="misce-transaction-card-subtitle">
                <img className="misce-transaction-card-subtitle-icon" src={BankLogo}></img> {transaction.fromAccount.name} <img className="misce-transaction-card-subtitle-icon more-margin" src={ArrowIconWhite}></img> <img className="misce-transaction-card-subtitle-icon" src={BankLogo}></img> {transaction.toAccount.name}
                </p>
        else
            return <p className="misce-transaction-card-subtitle">
                    <img className="misce-transaction-card-subtitle-icon" src={BankLogo}></img> {transaction.fromAccount != null ? transaction.fromAccount.name : transaction.toAccount.name}
                </p>
    }

    return <div className="misce-hover-purple misce-transaction-card">
        <div className="misce-transaction-card-content">
            <p className={`misce-transaction-card-title ${transaction.title ? '' : 'untitled'}`}>
                <div className={`misce-transaction-card-icon ${transactionType}`}></div>
                {transaction.title ? transaction.title : 'Untitled'}
                <button className="misce-action-button misce-edit-button" type="button" onClick={() => openTransactionUpdateModal(transaction.id)}></button>
            </p>
            <p className="misce-transaction-card-subtitle">
                <img className="misce-transaction-card-subtitle-icon" src={TransactionCategoryIcon}>
                </img> {transaction.transactionSubCategory != null ? transaction.transactionSubCategory.name : <i>No category</i>}
            </p>
            {getTransactionCardSubTitle()}
            <p className="misce-transaction-card-subtitle"><img className="misce-transaction-card-subtitle-icon" src={CalendarIcon}></img> {transaction.dateTime}</p>
        </div>
        <div className="misce-transaction-card-amount-container">
            <p className={`misce-transaction-card-amount ${transactionType}`}>{formatMoneyAmount(transaction.amount)} &euro;</p>
        </div>
    </div>
}

export default Transaction;