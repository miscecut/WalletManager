import React, { useState, useEffect } from 'react';
//components
import MutuallyExclusivePills from './../utils/mutuallyexclusivepills/MutuallyExclusivePills.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from './../../jsutils/apirequests.js';

function TransactionCategoriesManagementModal(props) {
    //the user's transaction categories to chose from
    const [transactionCategories, setTransactionCategories] = useState([]);
    //the selected transaction category to edit
    const [selectedTransactionCategory, setSelectedTransactionCategory] = useState({ selectedTransactionCategoryId: '' });

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

    //click on a category
    const selectCategoryClick = tcId => setSelectedTransactionCategory({ ...selectedTransactionCategory, selectedTransactionCategoryId: tcId });

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Edit categories</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-modal-content">
                <MutuallyExclusivePills
                    elements={transactionCategories}
                    selectedElementId={selectedTransactionCategory.selectedTransactionCategoryId}
                    selectElement={selectCategoryClick}
                />
            </div>
        </div>
    </div>
}

export default TransactionCategoriesManagementModal;