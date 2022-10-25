import React, { useState, useEffect } from 'react';
//css
import './TransactionCategoriesManagementModal.css';
//components
import TransactionCategory from './../transactioncategory/TransactionCategory.js';
import TransactionCategoryCreationForm from './../transactioncategorycreationform/TransactionCategoryCreationForm.js';
import TransactionCategoryEdit from './../transactioncategoryedit/TransactionCategoryEdit.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings,
    getTransactionCategoryCreatePostSettings
} from '../../../jsutils/apirequests.js';

function TransactionCategoriesManagementModal(props) {

    //STATES

    //the user's transaction categories to chose from
    const [transactionCategories, setTransactionCategories] = useState([]);
    //the errors on the creation of a transaction category
    const [transactionCreationErrors, setTransactionCreationErrors] = useState([]);
    //the selected transaction category to edit, if this has a value, the modal changes entirely
    const [transactionCategoryToEdit, setTransactionCategoryToEdit] = useState({ transactionCategoryId: '' });

    //EFFECTS

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

    //FUNCTIONS

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
                        setTransactionCreationErrors(data.errors);
                    });
            });
    }

    //this function is assigned to the pencil edit button, it selects a transaction category to update (by showing a completely different "modal subpage")
    const selectTransactionCategoryToEdit = transactionCategoryId => setTransactionCategoryToEdit({ ...transactionCategoryToEdit, transactionCategoryId: transactionCategoryId });

    //RENDERING

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Edit categories</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            {transactionCategoryToEdit.transactionCategoryId === '' ?
                <div className="misce-modal-content">
                    <div className="misce-transaction-categories-container">
                        {transactionCategories.map(tc => <TransactionCategory key={tc.id}
                            transactionCategory={tc}
                            editClick={selectTransactionCategoryToEdit}
                        />)}
                    </div>
                    <hr className="misce-wide-hr"></hr>
                    <TransactionCategoryCreationForm
                        errors={transactionCreationErrors}
                        createTransactionCategory={createTransactionCategory}
                    />
                </div>
                :
                <TransactionCategoryEdit
                    token={props.token}
                    transactionCategoryId={transactionCategoryToEdit.transactionCategoryId}
                />
            }
        </div>
    </div>
}

export default TransactionCategoriesManagementModal;