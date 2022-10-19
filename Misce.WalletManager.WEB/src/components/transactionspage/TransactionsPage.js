import React, { useState, useEffect } from 'react';
//css
import './TransactionsPage.css';
//components
import TransactionsContainer from './../transactionscontainer/TransactionsContainer.js';
//api
import { getApiBaseUrl, getGetCommonSettings, getTransactionCategoriesGetQueryParameters } from './../../jsutils/apirequests.js';

function TransactionsPage(props) {
    //the available transaction categories to chose from
    const [transactionCategories, setTransactionCategories] = useState([]);
    //groupBy: 'DAYS', 'WEEKS', 'MONTHS'
    //transactionType: null, '', 'PROFIT', 'EXPENSE', 'TRANSFER'
    const [filters, setFilters] = useState({ groupBy: 'DAYS', transactionType: '', transactionCategoryId: '' });

    //update the available transaction categories when the transaction type selected i updated
    useEffect(() => {
        //if 'Tranfer' was selected, there is no point in showing the transaction category select
        if (filters.transactionType == 'TRANSFER') {
            setTransactionCategories([]);
            setFilters({...filters, transactionCategoryId: '' });
        }
        else {
            fetch(getApiBaseUrl() + 'transactioncategories' + getTransactionCategoriesGetQueryParameters(filters.transactionType), getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => setTransactionCategories(data));
                });
        }
    }, [filters.transactionType]);

    //component render
    return <div className="misce-transactions-page-container">
        <div className="misce-card misce-transactions-content">
            <TransactionsContainer
                token={props.token}
                transactionsFilters={filters}
            />
        </div>
        <div className="misce-card misce-transactions-filters">
            <button className="misce-btn w-100" type="button">add transaction</button>
            <button className="misce-btn w-100" type="button">edit categories</button>
            <div className="misce-input-container">
                <label className="misce-input-label">Group transactions:</label>
                <select className="misce-select" value={filters.groupBy} onChange={e => setFilters({ ...filters, groupBy: e.target.value })}>
                    <option value="DAYS">By day</option>
                    <option value="WEEKS">By Week</option>
                    <option value="MONTHS">By Month</option>
                </select>
            </div>
            <div className="misce-input-container">
                <label className="misce-input-label">Transaction type:</label>
                <select className="misce-select" value={filters.transactionType} onChange={e => setFilters({ ...filters, transactionType: e.target.value })}>
                    <option value="">All</option>
                    <option value="PROFIT">Profit</option>
                    <option value="EXPENSE">Expense</option>
                    <option value="TRANSFER">Transfer</option>
                </select>
            </div>
            {filters.transactionType === 'TRANSFER' ?
                ''
                :
                <div className="misce-input-container">
                    <label className="misce-input-label">Transaction category:</label>
                    <select className="misce-select" value={filters.transactionCategoryId} onChange={e => setFilters({ ...filters, transactionCategoryId: e.target.value })}>
                        <option value="">All</option>
                        {transactionCategories.map(tc => <option className={`misce-option ${tc.isExpenseType ? 'expense-option' : 'profit-option'}`} value={tc.id}>{tc.name}</option>)}
                    </select>
                </div>
            }
        </div>
    </div>
}

export default TransactionsPage;