import React, { useState, useEffect } from 'react';
//css
import './TransactionsPage.css';
//components
import TransactionsContainer from './../transactionscontainer/TransactionsContainer.js';
//modals
import TransactionCreateModal from './../transactioncreatemodal/TransactionCreateModal.js';
import TransactionCategoriesManagementModal from './../transactioncategories/transactioncategoriesmanagementmodal/TransactionCategoriesManagementModal.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings,
    getTransactionCategoriesGetQueryParameters,
    getTransactionSubCategoriesGetQueryParameters
} from './../../jsutils/apirequests.js';

function TransactionsPage(props) {
    //set one month ago as default fromDate filter value
    let now = new Date();
    let theFirstOfTheMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    theFirstOfTheMonth.setHours(2, 0, 0, 0); //2 because gtm +2

    //the user's accounts to chose from
    const [accounts, setAccounts] = useState([]);
    //the available transaction categories to chose from
    const [transactionCategories, setTransactionCategories] = useState([]);
    //the available transaction subcategories to chose from
    const [transactionSubCategories, setTransactionSubCategories] = useState([]);
    //groupBy: 'DAYS', 'WEEKS', 'MONTHS'
    //transactionType: '', 'PROFIT', 'EXPENSE', 'TRANSFER'
    const [filters, setFilters] = useState({
        groupBy: 'DAYS',
        fromDate: theFirstOfTheMonth.toISOString().substring(0, 10),
        transactionType: '',
        transactionCategoryId: '',
        transactionSubCategoryId: '',
        fromAccountId: '',
        toAccountId: ''
    });
    //modals state
    const [modals, setModals] = useState({
        transactionCreateModalIsOpen: false,
        editCategoriesModalIsOpen: false
    });

    //this function closes the transaction create modal
    const closeTransactionCreateModal = () => setModals({ ...modals, transactionCreateModalIsOpen: false });

    //this function closes the transaction categories management modal
    const closeEditCategoriesModal = () => setModals({ ...modals, editCategoriesModalIsOpen: false });

    //get the user's account, this function is called only at the page startup
    useEffect(() => {
        fetch(getApiBaseUrl() + 'accounts', getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setAccounts(data);
                    });
            });
    }, []);

    //update the available transaction categories when the transaction type selected is updated
    useEffect(() => {
        //if 'Tranfer' was selected, there is no point in showing the transaction category select
        if (filters.transactionType == '2') {
            setTransactionCategories([]);
            setFilters({...filters, transactionCategoryId: '' });
        }
        else {
            fetch(getApiBaseUrl() + 'transactioncategories' + getTransactionCategoriesGetQueryParameters(filters.transactionType), getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => {
                            setTransactionCategories(data);
                            setFilters({
                                ...filters,
                                transactionCategoryId: '',
                                fromAccountId: filters.transactionType == '1' ? '' : filters.fromAccountId,
                                toAccountId: filters.transactionType == '0' ? '' : filters.toAccountId
                            });
                        });
                });
        }
    }, [filters.transactionType]);

    //update the available transaction subcategories when the transaction category id selected is updated
    useEffect(() => {
        //if 'Tranfer' was selected or All the transaction categories are shown, there is no point in showing the transaction category select
        if (filters.transactionType == 2 || filters.transactionCategoryId == '') {
            setTransactionSubCategories([]);
            setFilters({ ...filters, transactionSubCategoryId: '' });
        }
        else {
            fetch(getApiBaseUrl() + 'transactionsubcategories' + getTransactionSubCategoriesGetQueryParameters({
                transactionCategoryId: filters.transactionCategoryId
            }), getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => setTransactionSubCategories(data));
                });
        }
    }, [filters.transactionCategoryId]);

    //component render
    return <div className="misce-transactions-page-container">
        <div className="misce-card misce-transactions-content">
            <TransactionsContainer
                token={props.token}
                transactionType={filters.transactionType}
                fromDate={filters.fromDate}
                fromAccountId={filters.fromAccountId}
                toAccountId={filters.toAccountId}
                transactionCategoryId={filters.transactionCategoryId}
                transactionSubCategoryId={filters.transactionSubCategoryId}
            />
        </div>
        <div className="misce-card misce-transactions-filters">
            <button className="misce-btn w-100" type="button" onClick={() => setModals({ ...modals, transactionCreateModalIsOpen: true })}>add transaction</button>
            <button className="misce-btn w-100" type="button" onClick={() => setModals({ ...modals, editCategoriesModalIsOpen: true })}>edit categories</button>
            <div className="misce-input-container">
                <label className="misce-input-label">Group transactions:</label>
                <select className="misce-select" value={filters.groupBy} onChange={e => setFilters({ ...filters, groupBy: e.target.value })}>
                    <option value="DAYS">By day</option>
                </select>
            </div>
            <div className="misce-input-container">
                <label className="misce-input-label">From date:</label>
                <input className="misce-input" type="date" value={filters.fromDate} onChange={e => setFilters({ ...filters, fromDate: e.target.value })}></input>
            </div>
            <div className="misce-input-container">
                <label className="misce-input-label">Transaction type:</label>
                <select className="misce-select" value={filters.transactionType} onChange={e => setFilters({ ...filters, transactionType: e.target.value })}>
                    <option value="">All</option>
                    <option value="1">Profit</option>
                    <option value="0">Expense</option>
                    <option value="2">Transfer</option>
                </select>
            </div>
            {filters.transactionType != '1' ?
                <div className="misce-input-container">
                    <label className="misce-input-label">From account:</label>
                    <select className="misce-select" value={filters.fromAccountId} onChange={e => setFilters({ ...filters, fromAccountId: e.target.value })}>
                        <option value="">All</option>
                        {accounts.map(account => <option key={account.id} className="misce-option" value={account.id}>{account.name}</option>)}
                    </select>
                </div>
                :
                ''
            }
            {filters.transactionType != '0' ?
                <div className="misce-input-container">
                    <label className="misce-input-label">To account:</label>
                    <select className="misce-select" value={filters.toAccountId} onChange={e => setFilters({ ...filters, toAccountId: e.target.value })}>
                        <option value="">All</option>
                        {accounts.map(account => <option key={account.id} className="misce-option" value={account.id}>{account.name}</option>)}
                    </select>
                </div>
                :
                ''
            }
            {filters.transactionType != '2' ?
                <div className="misce-input-container">
                    <label className="misce-input-label">Transaction category:</label>
                    <select className="misce-select" value={filters.transactionCategoryId} onChange={e => setFilters({ ...filters, transactionCategoryId: e.target.value })}>
                        <option value="">All</option>
                        {transactionCategories.map(tc => <option key={tc.id} className={`misce-option ${tc.isExpenseType ? 'expense-option' : 'profit-option'}`} value={tc.id}>{tc.name}</option>)}
                    </select>
                </div>
                :
                ''
            }
            {filters.transactionType != '2' && filters.transactionCategoryId != '' ?
                <div className="misce-input-container">
                    <label className="misce-input-label">Transaction subcategory:</label>
                    <select className="misce-select" value={filters.transactionSubCategoryId} onChange={e => setFilters({ ...filters, transactionSubCategoryId: e.target.value })}>
                        <option value="">All</option>
                        {transactionSubCategories.map(tsc => <option key={tsc.id} className="misce-option" value={tsc.id}>{tsc.name}</option>)}
                    </select>
                </div>
                :
                ''
            }
        </div>
        <TransactionCreateModal
            show={modals.transactionCreateModalIsOpen}
            closeButtonFunction={closeTransactionCreateModal}
        />
        <TransactionCategoriesManagementModal
            token={props.token}
            show={modals.editCategoriesModalIsOpen}
            closeButtonFunction={closeEditCategoriesModal}
        />
    </div>
}

export default TransactionsPage;