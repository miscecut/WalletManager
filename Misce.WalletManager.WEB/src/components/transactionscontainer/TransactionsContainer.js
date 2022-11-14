import React, { useState, useEffect } from 'react';
//css
import './TransactionsContainer.css';
//components
import TransactionsSection from './../transactionssection/TransactionsSection.js';
import NoElementsCard from './../commoncomponents/noelementscard/NoElementsCard.js';
//api
import { getApiBaseUrl, getGetCommonSettings, getTransactionsGetQueryParameters } from './../../jsutils/apirequests.js';

function TransactionsContainer(props) {

    //STATE

    //the transactions shown in the various sections
    const [transactions, setTransactions] = useState([]);

    //EFFECTS

    //get the filtered transactions from the api
    useEffect(() => {
        fetch(getApiBaseUrl() + 'transactions' + getTransactionsGetQueryParameters({
            transactionType: props.transactionType,
            fromAccountId: props.fromAccountId,
            toAccountId: props.toAccountId,
            transactionCategoryId: props.transactionCategoryId,
            transactionSubCategoryId: props.transactionSubCategoryId,
            fromDate: props.fromDate
        }), getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => setTransactions(data));
            });
    }, [props.transactionType, props.fromAccountId, props.toAccountId, props.transactionCategoryId, props.transactionSubCategoryId, props.fromDate]);

    //FUNCTIONS

    //draw the content, the transactions divided by day or week or month
    function generateTransactionsSections() {
        //if no transaction were found, return a message
        if (transactions.length == 0)
            return <NoElementsCard message="No transactions found" />;
        else {
            //start by dividing the transactions by day, week or month
            let dividedTransactions = {};
            let transactionSections = []; //this will contain all the jsx sections

            if (props.groupBy === 'DAYS')
                transactions.forEach(transaction => {
                    //this will be the section title
                    let sectionTitle = new Date(transaction.dateTime).toLocaleDateString();
                    //put the transaction in the correct section
                    if (!dividedTransactions[sectionTitle])
                        dividedTransactions[sectionTitle] = [];
                    dividedTransactions[sectionTitle].push(transaction);
                });
            else if (props.groupBy === 'MONTHS')
                transactions.forEach(transaction => {
                    //this will be the section title
                    let sectionTitle = new Date(transaction.dateTime).getMonth() + '/' + new Date(transaction.dateTime).getFullYear();
                    //put the transaction in the correct section
                    if (!dividedTransactions[sectionTitle])
                        dividedTransactions[sectionTitle] = [];
                    dividedTransactions[sectionTitle].push(transaction);
                });

            //at this point, all the transactions are divided, the section jsxs must be created
            transactionSections = Object.keys(dividedTransactions).map(sectionTitle => <TransactionsSection key={sectionTitle} transactions={dividedTransactions[sectionTitle]} title={sectionTitle}/>)
            return transactionSections;
        }
    }

    //RENDERING

    //render component
    return <div className="misce-transactions-container-content">
        {generateTransactionsSections()}
    </div>;
}

export default TransactionsContainer;