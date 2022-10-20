import React from 'react';
//css
import './TransactionsSection.css';
//components
import Transaction from './../transaction/Transaction.js'

function TransactionsSection({ title, transactions }) {
    //component render
    return <div className="misce-transactions-section">
        <div className="misce-transactions-section-header">
            <hr className="misce-transactions-section-header-line"></hr>
            <p className="misce-transactions-section-header-title">{title}</p>
            <hr className="misce-transactions-section-header-line"></hr>
        </div>
        {transactions.map(t => <Transaction transaction={{ ...t, dateTime: new Date(t.dateTime).toLocaleString().replace(',', ' -') }} />)}
    </div>
}

export default TransactionsSection;