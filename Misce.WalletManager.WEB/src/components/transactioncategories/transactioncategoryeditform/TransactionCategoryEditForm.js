import React, { useState, useEffect } from 'react';
//css
import './TransactionCategoryEdit.css';
//utils
import { getErrorMap } from "../../../jsutils/errorhandling.js"
import { getGetCommonSettings, getApiBaseUrl } from "../../../jsutils/apirequests.js"

function TransactionCategoryEdit(props) {

    //STATES

    const [transactionCategoryForm, setTransactionCategoryForm] = useState({ name: '' });

    //EFFECTS

    useEffect(() => {
        //get the selected transaction category actual info
        fetch(getApiBaseUrl() + 'transactioncategories/' + props.transactionCategoryId, getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setTransactionCategoryForm({ ...transactionCategoryForm, name: data.name });
                    });
            });
    }, [props.TransactionCategoryId]);

    //FUNCTIONS

    const submitHandler = e => {
        e.preventDefault();
        props.updateTransactionCategoryFunction(transactionCategoryForm);
    }

    //RENDERING

    const errorMap = getErrorMap(props.errors);

    //render component
    return <form onSubmit={submitHandler} className="misce-transaction-category-edit-form-container">
        <div className="misce-input-container misce-tc-edit-input-container">
            <label className="misce-input-label" htmlFor="name">Name:</label>
            <input
                className={`misce-input ${errorMap['name'] != null ? 'misce-input-error' : ''}`}
                type="text"
                name="name"
                onChange={e => setTransactionCategoryForm({ ...transactionCategoryForm, name: e.target.value })}
                value={transactionCategoryForm.name} required></input>
            <p className="misce-input-error-message">{errorMap['name']}</p>
        </div>
        <button className="misce-btn label-margin-fix" type="submit">update <i className="fa-solid fa-pen-to-square"></i></button>
    </form>
}

export default TransactionCategoryEdit;