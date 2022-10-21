import React, { useState, useEffect } from 'react';
//css
import './TransactionsContainer.css';
//components
import TransactionsSection from './../transactionssection/TransactionsSection.js'
//api
import { getApiBaseUrl, getGetCommonSettings, getTransactionsGetQueryParameters } from './../../jsutils/apirequests.js';

function TransactionsContainer({ token, transactionsFilters }) {
    const [transactions, setTransactions] = useState([]);

    //get the filtered transactions from the api when the filters change
    useEffect(() => {
        fetch(getApiBaseUrl() + 'transactions' + getTransactionsGetQueryParameters(transactionsFilters), getGetCommonSettings(token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => setTransactions(data));
            });
    }, [transactionsFilters]);

    //draw the content, the transactions divided by day or week or month
    function generateTransactionsSections() {
        //if no transaction were found, return a message
        if (transactions.length == 0)
            return <h1>NIENTE</h1>;
        else {
            //start by dividing the transactions by day, week or month
            let dividedTransactions = {};
            let transactionSections = []; //this will contain all the jsx sections
            transactions.forEach(transaction => {
                //this will be the section title
                let sectionTitle = new Date(transaction.dateTime).toLocaleDateString();
                //put the transaction in the correct section
                if (dividedTransactions[sectionTitle] == null)
                    dividedTransactions[sectionTitle] = [];
                dividedTransactions[sectionTitle].push(transaction);
            });
            //at this point, all the transactions are divided, the section jsxs must be created
            transactionSections = Object.keys(dividedTransactions).map(sectionTitle => <TransactionsSection key={sectionTitle} transactions={dividedTransactions[sectionTitle]} title={sectionTitle}/>)
            return transactionSections;
        }
    }

    //render component
    return <div className="misce-transactions-container-content">
        {generateTransactionsSections()}
    </div>;
}

export default TransactionsContainer;