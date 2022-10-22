import React, { useState, useEffect } from 'react';
//css
import './TransactionsContainer.css';
//components
import TransactionsSection from './../transactionssection/TransactionsSection.js'
//api
import { getApiBaseUrl, getGetCommonSettings, getTransactionsGetQueryParameters } from './../../jsutils/apirequests.js';
//images
import NothingImage from './../../images/nothing-gray.png';

function TransactionsContainer(props) {
    const [transactions, setTransactions] = useState([]);

    //get the filtered transactions from the api
    useEffect(() => {
        fetch(getApiBaseUrl() + 'transactions' + getTransactionsGetQueryParameters({
            transactionType: props.transactionType,
            fromAccountId: props.fromAccountId,
            toAccountId: props.toAccountId,
            transactionCategoryId: props.transactionCategoryId,
            transactionSubCategoryId: props.transactionSubCategoryId
        }), getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => setTransactions(data));
            });
    }, [props.transactionType, props.fromAccountId, props.toAccountId, props.transactionCategoryId, props.transactionSubCategoryId]);

    //TODO: make this a component(elementName)?
    function getNoTransactionsJsx() {
        return <div className="misce-no-elements-card">
            <img className="misce-no-elements-card-image" src={NothingImage}></img>
            <p className="misce-no-elements-card-message">No transactions found.</p>
        </div>
    }

    //draw the content, the transactions divided by day or week or month
    function generateTransactionsSections() {
        //if no transaction were found, return a message
        if (transactions.length == 0)
            return getNoTransactionsJsx();
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