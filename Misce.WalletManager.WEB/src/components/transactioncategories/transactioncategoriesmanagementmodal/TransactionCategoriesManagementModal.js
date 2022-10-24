import React, { useState, useEffect } from 'react';
//components
import TransactionCategory from './../transactioncategory/TransactionCategory.js';
import TransactionCategoryCreationForm from './../transactioncategorycreationform/TransactionCategoryCreationForm.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from '../../../jsutils/apirequests.js';

function TransactionCategoriesManagementModal(props) {
    //the user's transaction categories to chose from
    const [transactionCategories, setTransactionCategories] = useState([]);
    //the selected transaction category to edit
    const [selectedTransactionCategory, setSelectedTransactionCategory] = useState({ selectedTransactionCategoryId: '' });
    //the user's transaction categories to chose from
    const [transactionSubCategories, setTransactionSubCategories] = useState([]);
    //the selected transaction category to edit
    const [selectedTransactionSubCategory, setSelectedTransactionSubCategory] = useState({ selectedTransactionSubCategoryId: '' });

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

    //get the selected transaction category subcategories
    useEffect(() => {
        if (selectedTransactionCategory.selectedTransactionCategoryId != '')
            fetch(getApiBaseUrl() + 'transactionsubcategories?transactionCategoryId=' + selectedTransactionCategory.selectedTransactionCategoryId, getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => {
                            setTransactionSubCategories(data);
                        });
                });
        //else
        //    setTransactionSubCategories([]);
    }, [selectedTransactionCategory]);

    //click on a category
    const selectCategoryClick = tcId => setSelectedTransactionCategory({ ...selectedTransactionCategory, selectedTransactionCategoryId: tcId });

    //click on a subcategory
    const selectSubCategoryClick = tcId => setSelectedTransactionSubCategory({ ...selectedTransactionSubCategory, selectedTransactionSubCategoryId: tcId });

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
                <TransactionCategoryCreationForm />
            </div>
        </div>
    </div>
}

export default TransactionCategoriesManagementModal;