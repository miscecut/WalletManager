import React, { useState } from 'react';
//css
import './TransactionCategoryCreationForm.css';
//utils
import { getErrorMap } from "../../../jsutils/errorhandling.js"

function TransactionCategoryCreationForm(props) {

    //STATE

    const [transactionCategoryForm, setTransactionCategoryForm] = useState({
        name: '',
        isExpenseType: false
    });

    //FUNCTIONS

    const submitHandler = e => {
        e.preventDefault();
        props.createTransactionCategory(transactionCategoryForm);
    }

    //RENDERING

    const errorMap = getErrorMap(props.errors);

    //render component
    return <form onSubmit={submitHandler} className="misce-transaction-category-cration-form-container">
        <div className="misce-input-container">
            <label className="misce-input-label" htmlFor="name">Name:</label>
            <input
                className={`misce-input ${errorMap['name'] != null ? 'misce-input-error' : ''}`}
                type="text"
                name="name"
                onChange={e => setTransactionCategoryForm({ ...transactionCategoryForm, name: e.target.value })}
                value={transactionCategoryForm.name} required></input>
            <p className="misce-input-error-message">{errorMap['name']}</p>
        </div>
        <div className="misce-input-container">
            <label className="misce-input-label">Transaction type:</label>
            <select className="misce-select" value={transactionCategoryForm.isExpenseType ? 'EXPENSE' : 'PROFIT'} onChange={e => setTransactionCategoryForm({ ...transactionCategoryForm, isExpenseType: e.target.value === 'EXPENSE' })}>
                <option value="PROFIT">Profit</option>
                <option value="EXPENSE">Expense</option>
            </select>
        </div>
        <button className="misce-btn label-margin-fix" type="submit">add <i className="fa-solid fa-plus"></i></button>
    </form>
}

export default TransactionCategoryCreationForm;