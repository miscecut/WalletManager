import React from 'react';
//css
import './TransactionCategory.css'

function TransactionCategory({ transactionCategory }) {
    return <div className="misce-transaction-category">
        <p className={`misce-transaction-category-name ${transactionCategory.isExpenseType ? 'expense' : 'profit'}`}>{transactionCategory.name}</p>
        <div className="misce-transaction-category-actions">
            <button className="misce-action-button misce-edit-button" type="button"></button>
            <button className="misce-action-button misce-delete-button" type="button"></button>
        </div>
    </div>
}

export default TransactionCategory;