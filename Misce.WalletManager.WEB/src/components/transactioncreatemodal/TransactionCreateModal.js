import React, { useState, useEffect } from 'react';
//css
import './TransactionCreateModal.css';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings,
    getTransactionCategoriesGetQueryParameters,
    getTransactionSubCategoriesGetQueryParameters
} from '../../jsutils/apirequests.js';
//error handling
import { getErrorMap } from "../../jsutils/errorhandling.js";

//the transaction createion/update form
//inputs in the props:
//-show: true if the modal is shown
//-transactionId: not null and populated if a transaction was selected to be updated, null if a new transaction must be created
function TransactionCreateModal(props) {

    //FUNCTIONS

    //returns an empty transaction form
    const getEmptyTransactionForm = () => {
        return {
            id: '', //uneusefull if a transaction must be created
            title: '',
            accountFromId: '',
            accountToId: '',
            transactionTypeId: '0', //expense
            transactionCategoryId: '',
            transactionSubCategoryId: '',
            description: '',
            dateTime: new Date().toJSON().slice(0, 19)
        };
    }

    //STATE

    //the transaction form to be passed to the api
    const [transaction, setTransaction] = useState(getEmptyTransactionForm());
    //the user's transaction categories
    const [transactionCategories, setTransactionCategories] = useState([]);
    //the user's transaction subcategories
    const [transactionSubCategories, setTransactionSubCategories] = useState([]);

    //EFFECTS

    //load transaction's data at modal startup (or empty the form)
    useEffect(() => {
        //update
        if (props.transactionId != null) {
            //retrieve the transaction data
            fetch(getApiBaseUrl() + 'transactions/' + props.transactionId, getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => {
                            console.log(data);
                        });
                });
        }
        //create
        else
            setTransaction(getEmptyTransactionForm());
    }, [props.transactionId]);

    //load the user's transaction categories depending on the transaction type
    useEffect(() => {
        //if 'Tranfer' was selected, there is no point in showing the transaction category select
        if (transaction.transactionTypeId === '2') {
            setTransactionCategories([]);
            setTransaction({ ...transaction, transactionSubCategoryId: '' });
        }
        else {
            fetch(getApiBaseUrl() + 'transactioncategories' + getTransactionCategoriesGetQueryParameters(transaction.transactionTypeId), getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => {
                            setTransactionCategories(data);
                            setTransaction({
                                ...transaction,
                                transactionCategoryId: '',
                                transactionSubCategoryId: '',
                                accountFromId: transaction.transactionTypeId === '1' ? '' : transaction.accountFromId,
                                accountToId: transaction.transactionTypeId === '0' ? '' : transaction.accountToId
                            });
                        });
                });
        }
    }, [transaction.transactionTypeId]);

    //load the user's transaction sub categories under the selected transaction category
    useEffect(() => {
        //if no transaction category was selected, there is no point in showing the transaction sub category select
        if (transaction.transactionCategoryId === '') {
            setTransactionSubCategories([]);
            setTransaction({ ...transaction, transactionSubCategoryId: '' });
        }
        else {
            fetch(getApiBaseUrl() + 'transactionsubcategories' + getTransactionSubCategoriesGetQueryParameters({
                transactionCategoryId: transaction.transactionCategoryId
            }), getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => setTransactionSubCategories(data));
                });
        }
    }, [transaction.transactionCategoryId]);

    //RENDERING

    const errorMap = getErrorMap(props.errors);

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Transaction creation</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-modal-content">
                <div className="misce-transaction-type-buttons-container">
                    <button type="button" className={`misce-btn misce-btn-profit ${transaction.transactionTypeId === '1' ? 'active' : ''}`} onClick={e => setTransaction({ ...transaction, transactionTypeId: '1' })}>profit</button>
                    <button type="button" className={`misce-btn misce-btn-expense ${transaction.transactionTypeId === '0' ? 'active' : ''}`} onClick={e => setTransaction({ ...transaction, transactionTypeId: '0' })}>expense</button>
                    <button type="button" className={`misce-btn ${transaction.transactionTypeId === '2' ? 'active' : ''}`} onClick={e => setTransaction({ ...transaction, transactionTypeId: '2' })}>transfer</button>
                </div>
                <div className="misce-transaction-form-container">
                    <div className="misce-input-container">
                        <label className="misce-input-label" htmlFor="dateandtime">Datetime:</label>
                        <input className={`misce-input ${errorMap['datetime'] != null ? 'misce-input-error' : ''}`} type="datetime-local" name="dateandtime" id="dateandtime" onChange={e => setTransaction({ ...transaction, dateTime: e.target.value })} value={transaction.dateTime} required></input>
                        <p className="misce-input-error-message">{errorMap['password']}</p>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label" htmlFor="title">Title:</label>
                        <input className={`misce-input ${errorMap['title'] != null ? 'misce-input-error' : ''}`} type="text" name="title" id="title" onChange={e => setTransaction({ ...transaction, title: e.target.value })} value={transaction.title}></input>
                        <p className="misce-input-error-message">{errorMap['password']}</p>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label">From account:</label>
                        <select className="misce-select" value={transaction.accountFromId} onChange={e => setTransaction({ ...transaction, accountFromId: e.target.value })} disabled={transaction.transactionTypeId === '1'}>
                            <option value=""></option>
                            {props.accounts.map(account => <option key={account.id} className="misce-option" value={account.id}>{account.name}</option>)}
                        </select>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label">To account:</label>
                        <select className="misce-select" value={transaction.accountToId} onChange={e => setTransaction({ ...transaction, accountToId: e.target.value })} disabled={transaction.transactionTypeId === '0'}>
                            <option value=""></option>
                            {props.accounts.map(account => <option key={account.id} className="misce-option" value={account.id}>{account.name}</option>)}
                        </select>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label">Transaction category:</label>
                        <select className="misce-select" value={transaction.transactionCategoryId} onChange={e => setTransaction({ ...transaction, transactionCategoryId: e.target.value })} disabled={transaction.transactionTypeId === '2'}>
                            <option value=""></option>
                            {transactionCategories.map(tc => <option key={tc.id} className="misce-option" value={tc.id}>{tc.name}</option>)}
                        </select>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label">Transaction subcategory:</label>
                        <select className="misce-select" value={transaction.transactionSubCategoryId} onChange={e => setTransaction({ ...transaction, transactionSubCategoryId: e.target.value })} disabled={transaction.transactionTypeId === '2'}>
                            <option value=""></option>
                            {transactionSubCategories.map(tsc => <option key={tsc.id} className="misce-option" value={tsc.id}>{tsc.name}</option>)}
                        </select>
                    </div>
                </div>
            </div>
        </div>
    </div>
}

export default TransactionCreateModal;