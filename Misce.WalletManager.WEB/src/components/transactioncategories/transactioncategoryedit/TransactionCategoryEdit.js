import React, { useState, useEffect } from 'react';
//css
import './TransactionCategoryEdit.css';
//components
import TransactionCategoryEditForm from './../transactioncategoryeditform/TransactionCategoryEditForm.js';
import TransactionSubCategory from './../transactionsubcategories/transactionsubcategory/TransactionSubCategory.js';
//utils
import {
    getTransactionCategoryUpdateSettings,
    getTransactionSubCategoriesGetQueryParameters,
    getGetCommonSettings,
    getApiBaseUrl
} from "../../../jsutils/apirequests.js";

function TransactionCategoryEdit({ token, transactionCategoryId }) {

    //STATES

    //the transaction subcategories under the selected transaction category to edit
    const [transactionSubCategories, setTransactionSubCategories] = useState([]);
    //the transaction category update errors
    const [updateErrors, setUpdateErrors] = useState([]);

    //EFFECTS

    //retrieve the transaction subcategories under the transaction category
    useEffect(() => {
        fetch(getApiBaseUrl() + 'transactionsubcategories' + getTransactionSubCategoriesGetQueryParameters({
            transactionCategoryId: transactionCategoryId
        }), getGetCommonSettings(token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => setTransactionSubCategories(data));
            });
    }, [transactionCategoryId])

    //FUNCTIONS

    //the function that effectively update the transaction category
    const updateTransactionCategory = updatedTransactionCategory => {
        fetch(getApiBaseUrl() + 'transactioncategories/' + transactionCategoryId, getTransactionCategoryUpdateSettings(updatedTransactionCategory, token))
            .then(res => {
                //if the update wasn't succesfull
                if (res.status === 422) {
                    res.json().then(data => setUpdateErrors(data.errors));
                }
            });
    }

    //RENDERING

    //component render
    return <div className="misce-modal-content">
        <TransactionCategoryEditForm
            token={token} //the token needed to make api calls
            errors={updateErrors} //the errors from the api when trying to update
            transactionCategoryId={transactionCategoryId} //the transaction category to update id
            updateTransactionCategoryFunction={updateTransactionCategory} //the function that effectively update the transaction category
        />
        <div className="misce-transaction-subcategories-container">
            <p className="misce-transaction-subcategories-container-title">Edit subcategories</p>
            {transactionSubCategories.map(tsc => <TransactionSubCategory transactionSubCategory={tsc} />)}
        </div>
    </div>
}

export default TransactionCategoryEdit;