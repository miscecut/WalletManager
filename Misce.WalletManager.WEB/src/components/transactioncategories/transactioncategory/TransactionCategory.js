import React from 'react';
//css
import './TransactionCategory.css'

function TransactionCategory({ transactionCategory, editClick }) {
    //render component
    return <div className={`misce-transaction-category ${transactionCategory.isExpenseType ? 'expense' : 'profit'}`}>
        <p className="misce-transaction-category-name">{transactionCategory.name}</p>
        <div className="misce-transaction-category-actions">
            <button className="misce-action-button misce-edit-button" type="button" onClick={() => editClick(transactionCategory.id)}></button>
            <button className="misce-action-button misce-delete-button" type="button"></button>
        </div>
    </div>
}

export default TransactionCategory;