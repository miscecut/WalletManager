import React, { useState, useEffect } from 'react';
//css
import './TransactionsContainer.css';
//api
import { getApiBaseUrl, getGetCommonSettings, getTransactionsGetQueryParameters } from './../../jsutils/apirequests.js';

function TransactionsContainer({ token, transactionsFilters }) {
    const [transactions, setTransactions] = useState([]);

    useEffect(() => {
        //get the filtered transactions from the api
        fetch(getApiBaseUrl() + 'transactions' + getTransactionsGetQueryParameters(transactionsFilters), getGetCommonSettings(token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => setTransactions(data));
            });
    }, [transactionsFilters]);

    let dividedTransactions = {};
    transactions.forEach(transaction => {
        let transactionDateAndTime = new Date(transaction.dateTime);

    });

    return <div className="misce-transactions-container-content">

    </div>;
}

export default TransactionsContainer;