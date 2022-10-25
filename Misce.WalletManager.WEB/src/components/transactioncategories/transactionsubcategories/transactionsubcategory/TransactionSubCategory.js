import React from 'react';
//css
import './TransactionSubCategory.css';

function TransactionSubCategory({ transactionSubCategory }) {
    return <div className="misce-transaction-subcategory">
            <p className="misce-transaction-subcategory-name">{transactionSubCategory.name}</p>
            <div className="misce-transaction-subcategory-actions">
                <button className="misce-action-button misce-edit-button smaller" type="button"></button>
                <button className="misce-action-button misce-delete-button smaller" type="button"></button>
            </div>
        </div>
}

export default TransactionSubCategory;