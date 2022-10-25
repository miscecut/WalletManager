import React, { useState } from 'react';
//components
import TransactionCategoryEditForm from './../transactioncategoryeditform/TransactionCategoryEditForm.js';
//utils
import { getTransactionCategoryUpdateSettings, getApiBaseUrl } from "../../../jsutils/apirequests.js";

function TransactionCategoryEdit({ token, transactionCategoryId }) {

    //STATE

    //the transaction subcategories under the selected transaction category to edit
    const [transactionSubCategories, setTransactionSubCategories] = useState([]);
    //the transaction category update errors
    const [updateErrors, setUpdateErrors] = useState([]);

    //FUNCTIONS

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
        </div>
}

export default TransactionCategoryEdit;