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
import NoElementsCard from '../../commoncomponents/noelementscard/NoElementsCard';

function TransactionCategoriesManagementModal(props) {

    //STATES

    //the modal title
    const [modalTitle, setModalTitle] = useState({ title: 'Edit categories' });
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
    const selectTransactionCategoryToEdit = transactionCategoryId => {
        //set the modal title with the name of the transaction category
        transactionCategories.forEach(tc => {
            if (tc.id === transactionCategoryId)
                setModalTitle({ ...modalTitle, title: 'Edit ' + tc.name });
        })
        //show the transaction category edit subpage
        setTransactionCategoryToEdit({ ...transactionCategoryToEdit, transactionCategoryId: transactionCategoryId });
    };

    //this functions removes the selected transaction category id, so the t.c. edit subpage is closed and the modal is back to the first default "page"
    const backToTransactionCategories = () => {
        //set the modal title back to default
        setModalTitle({ ...modalTitle, title: 'Edit categories' });
        //remove the transaction category to edit
        setTransactionCategoryToEdit({ ...transactionCategoryToEdit, transactionCategoryId: '' });
        //show again the transaction categories (maybe they were updated)
        fetch(getApiBaseUrl() + 'transactioncategories', getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setTransactionCategories(data);
                    });
            });
    }

    //RENDERING

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <button className="misce-modal-back-button" type="button" onClick={backToTransactionCategories} hidden={modalTitle.title === 'Edit categories'}></button>
                <p className="misce-modal-title">{modalTitle.title}</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            {transactionCategoryToEdit.transactionCategoryId === '' ?
                <div className="misce-modal-content">
                    {transactionCategories.length !== 0 ?
                        <div className="misce-transaction-categories-container">
                            {transactionCategories.map(tc => <TransactionCategory key={tc.id}
                                transactionCategory={tc}
                                editClick={selectTransactionCategoryToEdit}
                            />)}
                        </div>
                        :
                        <NoElementsCard message="No categories found" />
                    }
                    {transactionCategories.length !== 0 ?
                        <hr className="misce-wide-hr"></hr>
                        :
                        ''
                    }
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