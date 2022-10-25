import React, { useState } from 'react';
//css
import './TransactionSubCategoryCreationForm.css';
//utils
import { getErrorMap } from "../../../../jsutils/errorhandling.js"

function TransactionSubCategoryCreationForm(props) {

    //STATE

    const [transactionSubCategoryForm, setTransactionSubCategoryForm] = useState({ name: '' });

    //FUNCTIONS

    const submitHandler = e => {
        e.preventDefault();
        props.createTransactionSubCategory(transactionSubCategoryForm);
    }

    //RENDERING

    const errorMap = getErrorMap(props.errors);

    //render component
    return <form onSubmit={submitHandler} className="misce-transaction-subcategory-cration-form-container">
        <div className="misce-input-container">
            <label className="misce-input-label" htmlFor="name">Name:</label>
            <input
                className={`misce-input ${errorMap['name'] != null ? 'misce-input-error' : ''}`}
                type="text"
                name="name"
                onChange={e => setTransactionSubCategoryForm({ ...transactionSubCategoryForm, name: e.target.value })}
                value={transactionSubCategoryForm.name} required></input>
            <p className="misce-input-error-message">{errorMap['name']}</p>
        </div>
        <button className="misce-btn label-margin-fix" type="submit">add <i className="fa-solid fa-plus"></i></button>
    </form>
}

export default TransactionSubCategoryCreationForm;