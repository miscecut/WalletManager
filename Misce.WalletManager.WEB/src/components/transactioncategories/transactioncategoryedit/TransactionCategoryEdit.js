import React, { useState, useEffect } from 'react';
//css
import './TransactionCategoryEdit.css';
//components
import TransactionCategoryEditForm from './../transactioncategoryeditform/TransactionCategoryEditForm.js';
import TransactionSubCategory from './../transactionsubcategories/transactionsubcategory/TransactionSubCategory.js';
import TransactionSubCategoryCreationForm from './../transactionsubcategories/transactionsubcategorycreationform/TransactionSubCategoryCreationForm.js';
import NoElementsCard from './../../commoncomponents/noelementscard/NoElementsCard.js';
//utils
import {
    getTransactionCategoryUpdateSettings,
    getTransactionSubCategoriesGetQueryParameters,
    getTransactionSubCategoryCreatePostSettings,
    getGetCommonSettings,
    getApiBaseUrl
} from "../../../jsutils/apirequests.js";

function TransactionCategoryEdit({ token, transactionCategoryId }) {

    //STATES

    //the transaction subcategories under the selected transaction category to edit
    const [transactionSubCategories, setTransactionSubCategories] = useState([]);
    //the transaction category update errors
    const [updateErrors, setUpdateErrors] = useState([]);
    //the transaction subcategory creation errors
    const [transactionSubCategoryCreationErrors, setTransactionSubCategoryCreationErrors] = useState([]);

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

    //this function creates a subcategory under the category to edit
    const createTransactionSubCategory = transactionSubCategory => {
        //the transaction category id is added to the form
        transactionSubCategory.transactionCategoryId = transactionCategoryId;
        //the transaction subcategory gets created
        fetch(getApiBaseUrl() + 'transactionsubcategories', getTransactionSubCategoryCreatePostSettings(transactionSubCategory, token))
            .then(res => {
                //if the operation was succesfull...
                if (res.ok)
                    res.json().then(() => {
                        //...fetch the transaction subcategories again to see the new transaction subcategory
                        fetch(getApiBaseUrl() + 'transactionsubcategories' + getTransactionSubCategoriesGetQueryParameters({
                            transactionCategoryId: transactionCategoryId
                        }), getGetCommonSettings(token))
                            .then(res => {
                                if (res.ok) {
                                    res.json().then(data => setTransactionSubCategories(data));
                                    if (transactionSubCategoryCreationErrors.length !== 0)
                                        setTransactionSubCategoryCreationErrors([]);
                                }
                            });
                    });
                else if (res.status == 422)
                    res.json().then(data => {
                        setTransactionSubCategoryCreationErrors(data.errors);
                    });
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
            {transactionSubCategories.length !== 0 ?
                transactionSubCategories.map(tsc => <TransactionSubCategory transactionSubCategory={tsc} />)
                :
                <NoElementsCard message="No subcategories found" />
            }
        </div>
        {transactionSubCategories.length !== 0 ?
            <hr className="wide-hr"></hr>
            :
            ''
        }
        <TransactionSubCategoryCreationForm
            createTransactionSubCategory={createTransactionSubCategory}
            errors={transactionSubCategoryCreationErrors}
        />
    </div>
}

export default TransactionCategoryEdit;