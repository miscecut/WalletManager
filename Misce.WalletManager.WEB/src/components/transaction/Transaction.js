import React from 'react';
//css
import './Transaction.css';
//images
import BankLogo from '../../images/bank-icon.png'
import CalendarIcon from '../../images/calendar-icon.png'
import ArrowIconWhite from '../../images/arrow-right-icon.png'

function Transaction({ transaction }) {
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
                    <img className="misce-transaction-card-subtitle-icon" src={BankLogo}></img> {transaction.fromAccount != null ? transaction.fromAccount.name : transaction.toAccount.name} {transaction.transactionSubCategory != null ? '- ' + transaction.transactionSubCategory.name : ''}
                </p>
    }

    return <div className="misce-hover-purple misce-transaction-card">
            <div className={`misce-transaction-card-icon ${transactionType}`}></div>
            <div className="misce-transaction-card-content">
                <p className="misce-transaction-card-title">{transaction.title}</p>
                {getTransactionCardSubTitle()}
                <p className="misce-transaction-card-subtitle"><img className="misce-transaction-card-subtitle-icon" src={CalendarIcon}></img> {transaction.dateTime}</p>
                <p className="misce-transaction-card-description">{transaction.description == null || transaction.description == '' ? <i>No description</i> : transaction.description}</p> 
            </div>
            <div className="misce-transaction-card-amount-container">
                <p className={`misce-transaction-card-amount ${transactionType}`}>{transaction.amount} &euro;</p>
            </div>
        </div>
}

export default Transaction;