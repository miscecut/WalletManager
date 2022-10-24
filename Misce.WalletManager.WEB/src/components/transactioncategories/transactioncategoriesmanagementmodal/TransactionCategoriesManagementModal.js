import React, { useState, useEffect } from 'react';
//components
import TransactionCategory from './../transactioncategory/TransactionCategory.js';
import TransactionCategoryCreationForm from './../transactioncategorycreationform/TransactionCategoryCreationForm.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings,
    getTransactionCategoryCreatePostSettings
} from '../../../jsutils/apirequests.js';

function TransactionCategoriesManagementModal(props) {
    //the user's transaction categories to chose from
    const [transactionCategories, setTransactionCategories] = useState([]);
    //the errors on the creation of a transaction category
    const [transactionCreationErrors, setTransactionCreationErrors] = useState([]);

    //get the user's transaction categories
    useEffect(() => {
        fetch(getApiBaseUrl() + 'transactioncategories', getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setTransactionCategories(data);
                    });
            });
    }, []);

    //this function creates a transaction category with specified name and type (expense or not expense)
    const createTransactionCategory = transactionCategoryToCreate => {
        //the transaction category gets created
        fetch(getApiBaseUrl() + 'transactioncategories', getTransactionCategoryCreatePostSettings(transactionCategoryToCreate, props.token))
            .then(res => {
                //if the operation was succesfull...
                if (res.ok)
                    res.json().then(() => {
                        //...fetch the transaction categories again to see the new transaction category
                        fetch(getApiBaseUrl() + 'transactioncategories', getGetCommonSettings(props.token))
                            .then(res => {
                                if (res.ok)
                                    res.json().then(data => {
                                        setTransactionCategories(data);
                                        //clean errors, if there are
                                        if (transactionCreationErrors.length != 0)
                                            setTransactionCreationErrors([]);
                                    });
                            });
                    });
                else if (res.status == 422)
                    res.json().then(data => {
                        setTransactionCreationErrors(data);
                    });
            });
    }

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Edit categories</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-modal-content">
                {transactionCategories.map(tc => <TransactionCategory
                    key={tc.id}
                    transactionCategory={tc}
                />)}
                <hr className="misce-wide-hr"></hr>
                <TransactionCategoryCreationForm
                    errors={transactionCreationErrors}
                    createTransactionCategory={createTransactionCategory}
                />
            </div>
        </div>
    </div>
}

export default TransactionCategoriesManagementModal;